using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Rts2D.Building.SO;
using Rts2D.Pooling.Manager;

namespace Rts2D.ProductionMenu.View
{
    /// <summary>Endless building list; items that scroll off one end come back at the other.</summary>
    [RequireComponent(typeof(RectTransform))]
    public class ProductionInfiniteScrollView : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        // Spare rows outside the viewport, so wrapping shows no gap
        private const int SpareRowCount = 2;

        // Inertia stops below this speed (pixels per second)
        private const float StopVelocity = 0.5f;

        // How much a new drag sample replaces the velocity (0-1)
        private const float DragVelocitySmoothing = 0.5f;

        // Avoids dividing by zero on a zero-length frame
        private const float MinDeltaTime = 0.0001f;

        // Items are anchored and pivoted at their top-left corner
        private static readonly Vector2 TopLeft = new Vector2(0f, 1f);

        [Header("Layout")]
        [SerializeField, Min(1)] private int columnCount = 2;
        [SerializeField] private float itemSpacing = 10f;
        [Tooltip("Item height as a multiple of its width (1 = square).")]
        [SerializeField] private float itemHeightRatio = 1f;

        [Header("Scrolling")]
        [SerializeField] private float wheelScrollSpeed = 600f;
        [SerializeField] private float inertiaDamping = 5f;

        private readonly List<ProductItemView> _spawnedItems = new List<ProductItemView>();

        private ObjectPool<ProductItemView> _itemPool;
        private BuildingCatalogSO _buildingCatalog;
        private RectTransform _viewport;
        private Canvas _rootCanvas;

        private float _scrollOffset;        // in pixels
        private float _scrollVelocity;      // pixels per second, for inertia
        private bool _isDragging;

        private float _itemWidth;
        private float _itemHeight;
        private float _rowStep;             // item height + spacing
        private float _contentHeight;       // all rows; scrolling wraps after this
        private Vector2 _lastViewportSize;

        [Inject]
        private void Construct(ObjectPool<ProductItemView> itemPool, BuildingCatalogSO buildingCatalog)
        {
            _itemPool = itemPool;
            _buildingCatalog = buildingCatalog;
        }

        private void Awake()
        {
            _viewport = (RectTransform)transform;
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;
        }

        private void LateUpdate()
        {
            // First frame and on every resize
            if (_viewport.rect.size != _lastViewportSize)
            {
                FillViewportWithItems();
                PositionItems();
            }

            if (!_isDragging && Mathf.Abs(_scrollVelocity) > StopVelocity)
                ApplyInertia();
        }

        private void OnDestroy()
        {
            foreach (ProductItemView item in _spawnedItems)
                if (item != null) _itemPool.Despawn(item);
        }

        public void OnScroll(PointerEventData eventData)
        {
            _scrollVelocity -= eventData.scrollDelta.y * wheelScrollSpeed;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _isDragging = true;
            _scrollVelocity = 0f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float dragPixels = eventData.delta.y / _rootCanvas.scaleFactor;
            ScrollBy(dragPixels);

            // Track pointer speed so the list keeps sliding after release
            float pointerVelocity = dragPixels / Mathf.Max(Time.unscaledDeltaTime, MinDeltaTime);
            _scrollVelocity = Mathf.Lerp(_scrollVelocity, pointerVelocity, DragVelocitySmoothing);
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _isDragging = false;
        }

        /// <summary>Keeps sliding after a flick and slows down.</summary>
        private void ApplyInertia()
        {
            float deltaTime = Time.unscaledDeltaTime;
            ScrollBy(_scrollVelocity * deltaTime);

            // Exponential decay: same feel at any frame rate
            float decay = 1f - Mathf.Exp(-inertiaDamping * deltaTime);
            _scrollVelocity = Mathf.Lerp(_scrollVelocity, 0f, decay);
        }

        private void ScrollBy(float pixels)
        {
            _scrollOffset += pixels;
            PositionItems();
        }

        /// <summary>Takes just enough pooled items to fill the viewport and binds them.</summary>
        private void FillViewportWithItems()
        {
            IReadOnlyList<BuildingSO> buildings = _buildingCatalog.Buildings;
            if (buildings.Count == 0) return;

            Vector2 viewportSize = _viewport.rect.size;
            _lastViewportSize = viewportSize;

            _itemWidth = (viewportSize.x - itemSpacing * (columnCount + 1)) / columnCount;
            _itemHeight = _itemWidth * itemHeightRatio;
            _rowStep = _itemHeight + itemSpacing;

            int visibleRowCount = Mathf.CeilToInt(viewportSize.y / _rowStep);
            int itemCount = (visibleRowCount + SpareRowCount) * columnCount;

            // Round up so rows stay full and the order repeats cleanly
            int cycleLength = LeastCommonMultiple(buildings.Count, columnCount);
            itemCount = Mathf.CeilToInt(itemCount / (float)cycleLength) * cycleLength;

            while (_spawnedItems.Count < itemCount)
                _spawnedItems.Add(SpawnItem());

            while (_spawnedItems.Count > itemCount)
            {
                int lastIndex = _spawnedItems.Count - 1;
                _itemPool.Despawn(_spawnedItems[lastIndex]);
                _spawnedItems.RemoveAt(lastIndex);
            }

            for (int i = 0; i < _spawnedItems.Count; i++)
                _spawnedItems[i].Bind(buildings[i % buildings.Count]);

            int rowCount = _spawnedItems.Count / columnCount;
            _contentHeight = rowCount * _rowStep;
        }

        /// <summary>Places every item, shifted by the scroll offset.</summary>
        private void PositionItems()
        {
            if (_spawnedItems.Count == 0) return;

            // Keep the offset within one content height
            _scrollOffset = Mathf.Repeat(_scrollOffset, _contentHeight);

            for (int i = 0; i < _spawnedItems.Count; i++)
            {
                int row = i / columnCount;
                int column = i % columnCount;

                // Top of this row, measured down from the viewport top
                float rowTop = row * _rowStep - _scrollOffset;
                if (rowTop < -_rowStep)
                    rowTop += _contentHeight; // scrolled off the top: wrap to the bottom

                float left = itemSpacing + column * (_itemWidth + itemSpacing);
                float top = rowTop + itemSpacing;

                var itemRect = (RectTransform)_spawnedItems[i].transform;
                itemRect.sizeDelta = new Vector2(_itemWidth, _itemHeight);
                itemRect.anchoredPosition = new Vector2(left, -top); // UI y grows upwards
            }
        }

        private ProductItemView SpawnItem()
        {
            ProductItemView item = _itemPool.Spawn(transform);
            var itemRect = (RectTransform)item.transform;
            itemRect.anchorMin = itemRect.anchorMax = itemRect.pivot = TopLeft;
            return item;
        }

        private int LeastCommonMultiple(int a, int b)
        {
            return a / GreatestCommonDivisor(a, b) * b;
        }

        private int GreatestCommonDivisor(int a, int b)
        {
            while (b != 0)
                (a, b) = (b, a % b);
            return a;
        }
    }
}
