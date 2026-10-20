using System;
using UnityEngine;
using UnityEngine.InputSystem;
using Zenject;
using Rts2D.Building.SO;
using Rts2D.Building.View;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;
using Rts2D.Placement.Service;
using Rts2D.ProductionMenu.Manager;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Placement.Manager
{
    /// <summary>
    /// Runs the placement flow: starts it when a building is picked from the menu,
    /// reads cancel / place input, and turns the ghost into a real building.
    /// </summary>
    public class PlacementManager : ITickable, IInitializable, IDisposable
    {
        private readonly ProductSelectionService _productSelection;
        private readonly GhostBuildingManager _ghost;
        private readonly SpawnCellResolver _spawnCellResolver;
        private readonly GridManager _grid;
        private readonly IDamageableTracker _tracker;
        private readonly InteractionService _interaction;
        private readonly SelectionService _selection;

        public PlacementManager(ProductSelectionService productSelection, GhostBuildingManager ghost,
            SpawnCellResolver spawnCellResolver, GridManager grid, IDamageableTracker tracker,
            InteractionService interaction, SelectionService selection)
        {
            _productSelection = productSelection;
            _ghost = ghost;
            _spawnCellResolver = spawnCellResolver;
            _grid = grid;
            _tracker = tracker;
            _interaction = interaction;
            _selection = selection;
        }

        public void Initialize() => _productSelection.ProductSelected += StartPlacement;

        public void Dispose() => _productSelection.ProductSelected -= StartPlacement;

        public void Tick()
        {
            if (!_ghost.IsActive || Keyboard.current == null || Mouse.current == null) return;

            if (WasCancelPressed())
            {
                _interaction.ConsumeClick(); // the cancelling right click must not become a move order
                CancelPlacement();
                return;
            }

            _ghost.FollowCursor();

            if (Mouse.current.leftButton.wasPressedThisFrame && !_interaction.IsPointerOverUI())
            {
                _interaction.ConsumeClick(); // the placing click must not also select
                TryPlace();
            }
        }

        private void StartPlacement(BuildingSO data)
        {
            _ghost.Show(data);
            _interaction.SetMode(InteractionMode.Placing);
        }

        private void CancelPlacement()
        {
            _ghost.Hide();
            _interaction.SetMode(InteractionMode.Idle);
        }

        private void TryPlace()
        {
            if (!_ghost.CanPlace) return;

            Vector2Int originCell = _ghost.OriginCell;
            Vector2Int spawnCell = _spawnCellResolver.Resolve(originCell, _ghost.Data);
            BuildingView building = _ghost.Release();

            building.Initialize(originCell, spawnCell, _grid.GetCellCenter(spawnCell));
            _grid.PlaceOccupant(building);
            _tracker.Track(building);

            _interaction.SetMode(InteractionMode.Idle);
            _selection.Select(building); // show the new building right away (e.g. Barracks production buttons)
        }

        private static bool WasCancelPressed()
            => Keyboard.current.escapeKey.wasPressedThisFrame
            || Mouse.current.rightButton.wasPressedThisFrame;
    }
}
