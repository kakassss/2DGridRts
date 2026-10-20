using System.Collections.Generic;
using Rts2D.Pooling;
using UnityEngine;
using UnityEngine.EventSystems;
using Zenject;
using Rts2D.Building.DataSO;

namespace Rts2D.ProductionMenu.View
{
    [RequireComponent(typeof(RectTransform))]
    public class ProductionInfiniteScrollView : MonoBehaviour, IScrollHandler, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        #region Fields

        [Header("Settings")]
        [SerializeField, Min(1)] private int _columns = 2;
        [SerializeField] private float _spacing = 10f;
        [SerializeField] private float _itemAspect = 1f;
        [SerializeField] private float _wheelSpeed = 600f;
        [SerializeField] private float _damping = 5f;

        [Header("References")]
        [SerializeField] private RectTransform rectTransform;
        [SerializeField] private Canvas canvas;

        private ObjectPool<ProductItemView> _pool;
        private BuildingCatalogSO _productList;

        private Vector2 _lastSize;
        private float _offset;
        private float _velocity;
        private bool _dragging;

        private readonly List<ProductItemView> _views = new List<ProductItemView>();
        private readonly List<RectTransform> _rects = new List<RectTransform>();

        #endregion

        #region Lifecycle

        [Inject]
        private void Construct(ObjectPool<ProductItemView> pool, BuildingCatalogSO productList)
        {
            _pool = pool;
            _productList = productList;
        }

        private void Start()
        {
            rectTransform = (RectTransform)transform;
            canvas = canvas.rootCanvas;

            RebuildItems();
            Layout();
        }

        private void LateUpdate()
        {
            bool dirty = false;

            if (rectTransform.rect.size != _lastSize)
            {
                RebuildItems();
                dirty = true;
            }

            if (!_dragging && Mathf.Abs(_velocity) > 0.5f)
            {
                _offset += _velocity * Time.unscaledDeltaTime;
                _velocity = Mathf.Lerp(_velocity, 0f, 1f - Mathf.Exp(-_damping * Time.unscaledDeltaTime));
                dirty = true;
            }

            if (dirty)
                Layout();
        }

        private void OnDestroy()
        {
            foreach (ProductItemView view in _views)
                if (view != null) _pool.Despawn(view);
        }

        #endregion
        
        public void OnScroll(PointerEventData eventData)
        {
            _velocity -= eventData.scrollDelta.y * _wheelSpeed;
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            _dragging = true;
            _velocity = 0f;
        }

        public void OnDrag(PointerEventData eventData)
        {
            float delta = eventData.delta.y / canvas.scaleFactor;
            _offset += delta;

            float instant = delta / Mathf.Max(Time.unscaledDeltaTime, 0.0001f);
            _velocity = Mathf.Lerp(_velocity, instant, 0.5f);

            Layout();
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            _dragging = false;
        }
        
        #region Internals

        /// <summary>
        /// Spawns enough items (from the pool) to fill the area. Data comes from the ScriptableObject list
        /// and is bound cyclically: with 3 products, 12 slots show A B C A B C ...
        /// </summary>
        private void RebuildItems()
        {
            var products = _productList.Buildings;
            if (products.Count == 0) return;

            Vector2 area = rectTransform.rect.size;
            float itemWidth = (area.x - _spacing * (_columns + 1)) / _columns;
            float rowHeight = itemWidth * _itemAspect + _spacing;

            int neededRows = Mathf.CeilToInt(area.y / rowHeight) + 2;
            int total = neededRows * _columns;

            // Keep the cycle order intact: round up to a multiple of the product count
            total = Mathf.CeilToInt(total / (float)products.Count) * products.Count;

            while (_views.Count < total)
            {
                ProductItemView view = _pool.Spawn(transform);
                RectTransform rt = (RectTransform)view.transform;
                rt.anchorMin = rt.anchorMax = rt.pivot = new Vector2(0f, 1f);

                _views.Add(view);
                _rects.Add(rt);
            }

            // Fazlaysa havuza geri ver
            while (_views.Count > total)
            {
                int last = _views.Count - 1;
                _pool.Despawn(_views[last]);
                _views.RemoveAt(last);
                _rects.RemoveAt(last);
            }

            for (int i = 0; i < _views.Count; i++)
                _views[i].Bind(products[i % products.Count]);
        }

        private void Layout()
        {
            if (_rects.Count == 0) return;

            Vector2 area = rectTransform.rect.size;
            _lastSize = area;

            float itemWidth = (area.x - _spacing * (_columns + 1)) / _columns;
            float itemHeight = itemWidth * _itemAspect;
            float rowHeight = itemHeight + _spacing;

            int rows = Mathf.CeilToInt(_rects.Count / (float)_columns);
            float loopHeight = Mathf.Max(rows * rowHeight, area.y + rowHeight);

            for (int i = 0; i < _rects.Count; i++)
            {
                int row = i / _columns;
                int col = i % _columns;

                float y = Mod(row * rowHeight - _offset + rowHeight, loopHeight) - rowHeight;

                _rects[i].sizeDelta = new Vector2(itemWidth, itemHeight);
                _rects[i].anchoredPosition = new Vector2(_spacing + col * (itemWidth + _spacing), -y - _spacing);
            }
        }

        private static float Mod(float value, float modulus)
        {
            return ((value % modulus) + modulus) % modulus;
        }

        #endregion
    }
}
