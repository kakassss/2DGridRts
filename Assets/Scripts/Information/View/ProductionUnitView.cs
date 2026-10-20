using System;
using Rts2D.Shared.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rts2D.Information.View
{
    public class ProductionUnitView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;

        private IProduct _data;
        private Action<IProduct> _onClicked;

        private void Awake() => button.onClick.AddListener(HandleClick);
        private void OnDestroy() => button.onClick.RemoveListener(HandleClick);

        public void Bind(IProduct data, Action<IProduct> onClicked)
        {
            _data = data;
            _onClicked = onClicked;
            iconImage.sprite = data.Icon;
            nameText.text = data.DisplayName;
        }

        private void HandleClick() => _onClicked?.Invoke(_data);
    }
}
