using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Information.View
{
    public class InformationPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject _root;
        [SerializeField] private Image _iconImage;
        [SerializeField] private TextMeshProUGUI _nameText;
        [SerializeField] private Transform _productionContainer;
        [SerializeField] private ProductionUnitView productionUnitViewPrefab;

        private SelectionService _selectionService;
        private SoldierProductionService _soldierService;
        private IInstantiator _instantiator;

        private ISelectable _current;
        private readonly List<ProductionUnitView> _buttons = new List<ProductionUnitView>();

        [Inject]
        private void Construct(SelectionService selectionService,
            SoldierProductionService soldierService,
            IInstantiator instantiator)
        {
            _selectionService = selectionService;
            _soldierService = soldierService;
            _instantiator = instantiator;
        }

        private void OnEnable()
        {
            _selectionService.Selected += Show;
            _selectionService.Cleared += Hide;
            Hide();
        }

        private void OnDisable()
        {
            _selectionService.Selected -= Show;
            _selectionService.Cleared -= Hide;
        }

        private void Show(ISelectable selectable)
        {
            _current = selectable; // Request'e gönderilecek üretici bina

            _root.SetActive(true);
            _iconImage.sprite = selectable.Icon;
            _nameText.text = selectable.DisplayName;

            IReadOnlyList<IProduct> producibles = selectable.Producibles;

            // Gereken kadar buton oluştur
            while (_buttons.Count < producibles.Count)
            {
                ProductionUnitView unitView = _instantiator
                    .InstantiatePrefabForComponent<ProductionUnitView>(
                        productionUnitViewPrefab.gameObject, _productionContainer);
                _buttons.Add(unitView);
            }

            // Kullanılanları aç ve bağla, fazlasını gizle
            for (int i = 0; i < _buttons.Count; i++)
            {
                bool active = i < producibles.Count;
                _buttons[i].gameObject.SetActive(active);

                if (active)
                    _buttons[i].Bind(producibles[i], OnProduceClicked);
            }
        }

        private void Hide()
        {
            _current = null;
            _root.SetActive(false);
        }

        // Butona basılınca burası çalışır, Request'i çağıran yer burası
        private void OnProduceClicked(IProduct soldier)
        {
            if (_current == null) return;
            _soldierService.Request(soldier, _current);
        }
    }
}
