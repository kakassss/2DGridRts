using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Rts2D.Unit.Soldier.DataSO;

namespace Rts2D.Information.View
{
    public class ProductionUnitView : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;

        private SoldierDataSo _data;
        private Action<SoldierDataSo> _onClicked;

        private void Awake() => _button.onClick.AddListener(HandleClick);
        private void OnDestroy() => _button.onClick.RemoveListener(HandleClick);

        public void Bind(SoldierDataSo data, Action<SoldierDataSo> onClicked)
        {
            _data = data;
            _onClicked = onClicked;
            _iconImage.sprite = data.Icon;
            _nameText.text = data.DisplayName;
        }

        private void HandleClick() => _onClicked?.Invoke(_data);
    }
}
