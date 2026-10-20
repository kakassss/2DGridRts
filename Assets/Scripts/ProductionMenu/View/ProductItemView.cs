using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Rts2D.Building.SO;
using Rts2D.ProductionMenu.Manager;

namespace Rts2D.ProductionMenu.View
{
    public class ProductItemView : MonoBehaviour
    {
        [SerializeField] private Button button;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;

        private ProductSelectionService _service;
        private BuildingSO _data;

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

        public void Bind(BuildingSO data)
        {
            _data = data;
            iconImage.sprite = data.Icon;
            nameText.text = $"{data.DisplayName} ({data.Size.x}x{data.Size.y})"; // e.g. "Barrack (4x4)", size in cells
        }

        private void HandleClick()
        {
            if (_data != null)
                _service.Select(_data);
        }
    }
}
