using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Building.DataSO;
using Rts2D.Building.Manager;
using Rts2D.Building.View;
using Rts2D.Combat.Manager;
using Rts2D.GameConfig.DataSO;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Service;
using Rts2D.ProductionMenu.Manager;

namespace Rts2D.Placement.Manager
{
    public class PlacementManager : ITickable, IInitializable, IDisposable
    {
        private readonly ProductSelectionService _service;
        private readonly GameConfigSO _gameConfigSo;
        private readonly GridManager _gridManager;
        private readonly MouseInputService _mouseInputService;
        private readonly DestroyManager _destroyManager;
        private readonly BuildingPoolManager _poolManager;
        
        private BuildingSO _current;
        private BuildingView _ghostBuilding;
        private Vector2Int _currentOriginCell;
        private bool _canPlace;

        public bool IsPlacing => _ghostBuilding != null;

        public PlacementManager(GridManager gridManager, MouseInputService mouseInputService,
            GameConfigSO gameConfigSo, ProductSelectionService service,
            DestroyManager destroyManager, BuildingPoolManager poolManager)
        {
            _gridManager = gridManager;
            _mouseInputService = mouseInputService;
            _gameConfigSo = gameConfigSo;
            _service = service;
            _destroyManager = destroyManager;
            _poolManager = poolManager;
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
                DestroyGhost();
                return;
            }

            UpdateGhost();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
                TryPlace();
        }

        private void StartPlacement(BuildingSO data)
        {
            DestroyGhost();

            _current = data;
            _ghostBuilding = _poolManager.Spawn(data);
            UpdateGhost(); // snap to the cursor right away, avoids a frame at the old position
        }

        private void DestroyGhost()
        {
            if (_ghostBuilding == null) return;

            _poolManager.Despawn(_ghostBuilding);
            _ghostBuilding = null;
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
            _destroyManager.Track(view);

            _ghostBuilding = null;
        }
        
        private bool WasCancelPressed()
            => Keyboard.current.escapeKey.wasPressedThisFrame
            || Mouse.current.rightButton.wasPressedThisFrame;

        private bool IsPointerOverUI()
            => EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
    }
}