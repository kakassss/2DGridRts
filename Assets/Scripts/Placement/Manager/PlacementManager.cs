using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Building.DataSO;
using Rts2D.Building.Manager;
using Rts2D.Building.View;
using Rts2D.GameConfig.DataSO;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Service;
using Rts2D.ProductionMenu.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Shared.Selection;

namespace Rts2D.Placement.Manager
{
    public class PlacementManager : ITickable, IInitializable, IDisposable
    {
        private readonly ProductSelectionService _service;
        private readonly GameConfigSO _gameConfigSo;
        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly IDamageableTracker _tracker;
        private readonly BuildingPoolManager _poolManager;
        private readonly InteractionService _interaction;
        private readonly SelectionService _selection;
        
        private BuildingSO _current;
        private BuildingView _ghostBuilding;
        private Vector2Int _currentOriginCell;
        private bool _canPlace;

        public bool IsPlacing => _ghostBuilding != null;

        public PlacementManager(GridManager gridManager, MouseInputService mouseInputService,
            GameConfigSO gameConfigSo, ProductSelectionService service,
            IDamageableTracker tracker, BuildingPoolManager poolManager, InteractionService interaction,
            SelectionService selection)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _gameConfigSo = gameConfigSo;
            _service = service;
            _tracker = tracker;
            _poolManager = poolManager;
            _interaction = interaction;
            _selection = selection;
        }

        public void Initialize()
        {
            _service.ProductSelected += StartPlacement;
        }

        public void Dispose()
        {
            _service.ProductSelected -= StartPlacement;
            _ghostBuilding = null; // the scene is closing, nothing to return to the pool
        }

        public void Tick()
        {
            if (!IsPlacing || Keyboard.current == null || Mouse.current == null) return;

            if (WasCancelPressed())
            {
                _interaction.ConsumeClick(); // the cancelling right click must not become a move order
                DestroyGhost();
                return;
            }

            UpdateGhost();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
            {
                _interaction.ConsumeClick(); // the placing click must not also select
                TryPlace();
            }
        }

        private void StartPlacement(BuildingSO data)
        {
            DestroyGhost();

            _current = data;
            _ghostBuilding = _poolManager.Spawn(data);
            _interaction.SetMode(InteractionMode.Placing);
            UpdateGhost(); // snap to the cursor right away, avoids a frame at the old position
        }

        private void DestroyGhost()
        {
            if (_ghostBuilding == null) return;

            _poolManager.Despawn(_ghostBuilding);
            _ghostBuilding = null;
            _interaction.SetMode(InteractionMode.Idle);
        }

        private void UpdateGhost()
        {
            Vector2Int size = _current.Size;

            _currentOriginCell = _gridManager.GetBuildingOrigin(_mouseInputService.GetWorldPosition(), size);
            _canPlace = _gridManager.IsAreaFree(_currentOriginCell, size);

            _ghostBuilding.transform.position = _gridManager.GetBuildingCenter(_currentOriginCell, size);
            _ghostBuilding.SetTint(_canPlace ? _gameConfigSo.ValidPlacementColor : _gameConfigSo.InvalidPlacementColor);
        }

        private void TryPlace()
        {
            if (!_canPlace) return;

            BuildingView view = _ghostBuilding;
            view.Initialize(_currentOriginCell);
            view.SetTint(Color.white);

            _gridManager.PlaceOccupant(view);
            _tracker.Track(view);

            _ghostBuilding = null;
            _interaction.SetMode(InteractionMode.Idle);

            _selection.Select(view); // show the new building right away (e.g. Barracks production buttons)
        }
        
        private bool WasCancelPressed()
            => Keyboard.current.escapeKey.wasPressedThisFrame
            || Mouse.current.rightButton.wasPressedThisFrame;

        private bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}