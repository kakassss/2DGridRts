using UnityEngine;

namespace Rts2D.Health.View
{
    public class HealthBarView : MonoBehaviour
    {
        [SerializeField] private SpriteRenderer background;
        [SerializeField] private SpriteRenderer fill;

        [Header("Look")]
        [SerializeField] private Color backgroundColor = new Color(0.1f, 0.1f, 0.1f, 1f);
        [SerializeField] private Color fillColor = Color.green;
        [SerializeField] private int sortingOrder = 100;
        [SerializeField, Range(0.1f, 1f)] private float widthRatio = 0.8f;

        // Bar thickness in world units
        private const float Height = 0.1f;

        // Gap between the top of the owner and the bar, in world units
        private const float Padding = 0.1f;

        private Transform _fillPivot;
        private float _width;
        private bool _built;

        /// <summary>Sizes the bar to its owner and places it above.</summary>
        public void Setup(Vector2 ownerSize)
        {
            _width = ownerSize.x * widthRatio;

            if (!_built)
            {
                _fillPivot = new GameObject("FillPivot").transform;
                _fillPivot.SetParent(transform, false);
                fill.transform.SetParent(_fillPivot, false);
                _built = true;
            }

            transform.localPosition = new Vector3(0f, ownerSize.y / 2f + Padding, 0f);

            background.color = backgroundColor;
            background.sortingOrder = sortingOrder;
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = new Vector3(_width, Height, 1f);

            fill.color = fillColor;
            fill.sortingOrder = sortingOrder + 1;

            _fillPivot.localPosition = new Vector3(-_width / 2f, 0f, 0f);
            fill.transform.localPosition = new Vector3(_width / 2f, 0f, 0f);
            fill.transform.localScale = new Vector3(_width, Height, 1f);
        }

        public void SetHealth(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

            _fillPivot.localScale = new Vector3(ratio, 1f, 1f);

            gameObject.SetActive(ratio < 1f);
        }
    }
}
