using UnityEngine;
using UnityEngine.UI;

namespace Rts2D.Selection.View
{
    /// <summary>Drag-selection box. Put it on a UI Image directly under the root canvas.</summary>
    [RequireComponent(typeof(RectTransform), typeof(Image))]
    public class SelectionBoxView : MonoBehaviour
    {
        private RectTransform _rect;
        private RectTransform _parentRect;
        private Image _image;
        private Canvas _rootCanvas;

        private void Awake()
        {
            _rect = (RectTransform)transform;
            _parentRect = (RectTransform)_rect.parent;
            _image = GetComponent<Image>();
            _rootCanvas = GetComponentInParent<Canvas>().rootCanvas;

            // Anchor at the parent origin, pivot bottom-left: anchoredPosition = box corner
            _rect.anchorMin = _rect.anchorMax = _parentRect.pivot;
            _rect.pivot = Vector2.zero;

            _image.raycastTarget = false; // never block clicks
            Hide();
        }

        /// <summary>Stretches the box between two screen positions.</summary>
        public void Show(Vector2 screenStart, Vector2 screenEnd)
        {
            Vector2 start = ScreenToLocal(screenStart);
            Vector2 end = ScreenToLocal(screenEnd);

            _rect.anchoredPosition = Vector2.Min(start, end);
            _rect.sizeDelta = new Vector2(Mathf.Abs(end.x - start.x), Mathf.Abs(end.y - start.y));
            _image.enabled = true;
        }

        public void Hide() => _image.enabled = false;

        private Vector2 ScreenToLocal(Vector2 screenPosition)
        {
            // Overlay canvases need no camera
            Camera canvasCamera = _rootCanvas.renderMode == RenderMode.ScreenSpaceOverlay ? null : _rootCanvas.worldCamera;

            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                _parentRect, screenPosition, canvasCamera, out Vector2 localPosition);
            return localPosition;
        }
    }
}
