using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Rts2D.Building.DataSO;
using Rts2D.ProductionMenu.Manager;

namespace Rts2D.ProductionMenu.View
{
    public class ProductItemView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;

        private ProductSelectionService _service;
        private BuildingDataSO _data;

        [Inject]
        private void Construct(ProductSelectionService service)
        {
            _service = service;
        }

        private void Awake()
        {
            button.onClick.AddListener(HandleClick);
        }

        private void OnDestroy()
        {
            button.onClick.RemoveListener(HandleClick);
        }

        public void Bind(BuildingDataSO data)
        {
            _data = data;
            iconImage.sprite = data.Icon;
            nameText.text = data.DisplayName;
        }

        private void HandleClick()
        {
            if (_data != null)
                _service.Select(_data);
        }
    }
}
