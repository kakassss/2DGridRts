using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Building.DataSO;
using Rts2D.Building.View;
using Rts2D.Combat.Manager;
using Rts2D.GameConfig.DataSO;
using Rts2D.Grid.Manager;
using Rts2D.Grid.Utils;
using Rts2D.ProductionMenu.Manager;
using Rts2D.Test;

namespace Rts2D.Placement.Manager
{
    public class PlacementManager : ITickable, IInitializable, IDisposable
    {
        private readonly ProductSelectionService _service;
        private readonly GameConfigSO _gameConfigSo;
        private readonly GridManager _gridManager;
        private readonly MouseInput _mouseInput;
        private readonly DestroyManager _destroyManager;
        private readonly IInstantiator _instantiator;

        private readonly List<Vector2Int> _footprintCells = new List<Vector2Int>();

        private BuildingDataSO _currentData;
        private BuildingView _ghostBuilding;
        private SpriteRenderer _ghostRenderer;
        private Vector2Int _currentOriginCell;
        private bool _canPlace;

        public bool IsPlacing => _ghostBuilding != null;

        public PlacementManager(GridManager gridManager, MouseInput mouseInput,
            GameConfigSO gameConfigSo, ProductSelectionService service,
            DestroyManager destroyManager, IInstantiator instantiator)
        {
            _gridManager = gridManager;
            _mouseInput = mouseInput;
            _gameConfigSo = gameConfigSo;
            _service = service;
            _destroyManager = destroyManager;
            _instantiator = instantiator;
        }

        public void Initialize()
        {
            _service.ProductSelected += StartPlacement;
        }

        public void Dispose()
        {
            _service.ProductSelected -= StartPlacement;
            DestroyGhost();
        }

        public void Tick()
        {
            if (_ghostBuilding == null) return;
            if (Keyboard.current == null || Mouse.current == null) return;

            if (Keyboard.current.escapeKey.wasPressedThisFrame ||
                Mouse.current.rightButton.wasPressedThisFrame)
            {
                DestroyGhost();
                return;
            }

            UpdateGhost();

            if (Mouse.current.leftButton.wasPressedThisFrame && !IsPointerOverUI())
                TryPlace();
        }

        private void StartPlacement(BuildingDataSO data)
        {
            DestroyGhost();

            _currentData = data;
            _ghostBuilding = _instantiator.InstantiatePrefabForComponent<BuildingView>(data.BuildingPrefab);
            _ghostRenderer = _ghostBuilding.GetComponent<SpriteRenderer>();
        }

        private void DestroyGhost()
        {
            if (_ghostBuilding != null)
                UnityEngine.Object.Destroy(_ghostBuilding.gameObject);

            _ghostBuilding = null;
            _ghostRenderer = null;
        }

        private void UpdateGhost()
        {
            Vector2Int size = _currentData.Size;

            _currentOriginCell = _gridManager.GetBuildingOrigin(_mouseInput.GetMouseInput(), size);
            _canPlace = CanPlace(_currentOriginCell, size);

            _ghostBuilding.transform.position = _gridManager.GetBuildingCenter(_currentOriginCell, size);
            _ghostRenderer.color = _canPlace
                ? _gameConfigSo.ValidPlacementColor
                : _gameConfigSo.InvalidPlacementColor;
        }

        private void TryPlace()
        {
            if (!_canPlace) return;

            BuildingView view = _ghostBuilding;
            view.Initialize(_currentData, _currentOriginCell);

            GridCellUtils.FillRect(_currentOriginCell, _currentData.Size, _footprintCells);
            foreach (Vector2Int cell in _footprintCells)
                _gridManager.SetOccupant(cell.x, cell.y, view);

            _ghostRenderer.color = Color.white;
            _destroyManager.Track(view);

            // Ghost artık gerçek bina, referansı bırak (yok etme)
            _ghostBuilding = null;
            _ghostRenderer = null;
        }

        /// <summary>Sınır dışı hücreler GetPositionIsOccupied ile dolu sayılır, ayrı kontrol gerekmez.</summary>
        private bool CanPlace(Vector2Int origin, Vector2Int size)
        {
            GridCellUtils.FillRect(origin, size, _footprintCells);

            foreach (Vector2Int cell in _footprintCells)
                if (_gridManager.GetPositionIsOccupied(cell.x, cell.y))
                    return false;

            return true;
        }

        private bool IsPointerOverUI()
        {
            return EventSystem.current != null && EventSystem.current.IsPointerOverGameObject();
        }
    }
}