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

        private const float Height = 0.1f;
        private const float Padding = 0.1f;

        private Transform _fillPivot;
        private float _width;
        private bool _built;

        public void Setup(float width, float ownerHeight)
        {
            _width = width;

            if (!_built)
            {
                _fillPivot = new GameObject("FillPivot").transform;
                _fillPivot.SetParent(transform, false);
                fill.transform.SetParent(_fillPivot, false);
                _built = true;
            }

            transform.localPosition = new Vector3(0f, ownerHeight / 2f + Padding, 0f);

            background.color = backgroundColor;
            background.sortingOrder = sortingOrder;
            background.transform.localPosition = Vector3.zero;
            background.transform.localScale = new Vector3(width, Height, 1f);

            fill.color = fillColor;
            fill.sortingOrder = sortingOrder + 1;

            _fillPivot.localPosition = new Vector3(-width / 2f, 0f, 0f);
            fill.transform.localPosition = new Vector3(width / 2f, 0f, 0f);
            fill.transform.localScale = new Vector3(width, Height, 1f);
        }

        public void SetHealth(int current, int max)
        {
            float ratio = max > 0 ? Mathf.Clamp01((float)current / max) : 0f;

            _fillPivot.localScale = new Vector3(ratio, 1f, 1f);

            gameObject.SetActive(ratio < 1f);
        }
    }
}
