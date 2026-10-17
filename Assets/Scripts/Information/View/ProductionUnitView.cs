using System;
using Rts2D.Shared.Contracts;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rts2D.Information.View
{
    public class ProductionUnitView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;

        private IProduct _data;
        private Action<IProduct> _onClicked;

        private void Awake() => _button.onClick.AddListener(HandleClick);
        private void OnDestroy() => _button.onClick.RemoveListener(HandleClick);

        public void Bind(IProduct data, Action<IProduct> onClicked)
        {
            _data = data;
            _onClicked = onClicked;
            _iconImage.sprite = data.Icon;
            _nameText.text = data.DisplayName;
        }

        private void HandleClick() => _onClicked?.Invoke(_data);
    }
}
