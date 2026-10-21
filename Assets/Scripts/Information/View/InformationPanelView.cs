using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Information.View
{
    public class InformationPanelView : MonoBehaviour
    {
        [SerializeField] private GameObject root;
        [SerializeField] private Image iconImage;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Transform productionContainer;
        [SerializeField] private ProductionUnitView productionUnitViewPrefab;

        private SelectionService _selectionService;
        private SoldierProductionService _soldierService;
        private IInstantiator _instantiator;

        private static readonly IReadOnlyList<IProduct> NoProducibles = new List<IProduct>();

        private IProducer _currentProducer;
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
            // Only producers get production buttons
            _currentProducer = selectable as IProducer;

            root.SetActive(true);
            iconImage.sprite = selectable.Icon;
            // Several selected: show the first and the count
            int selectedCount = _selectionService.SelectedItems.Count;
            nameText.text = selectedCount > 1
                ? $"{selectable.DisplayName} (+{selectedCount - 1})"
                : selectable.DisplayName;

            // Non-producers get an empty list
            IReadOnlyList<IProduct> producibles = NoProducibles;
            if (_currentProducer != null)
                producibles = _currentProducer.Producibles;

            // Create as many buttons as needed
            while (_buttons.Count < producibles.Count)
            {
                ProductionUnitView unitView = _instantiator
                    .InstantiatePrefabForComponent<ProductionUnitView>(
                        productionUnitViewPrefab.gameObject, productionContainer);
                _buttons.Add(unitView);
            }

            // Enable and bind the used ones, hide the rest
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
            _currentProducer = null;
            root.SetActive(false);
        }

        // Sends the production request for the clicked soldier
        private void OnProduceClicked(IProduct soldier)
        {
            if (_currentProducer == null) return;
            _soldierService.Request(soldier, _currentProducer);
        }
    }
}
