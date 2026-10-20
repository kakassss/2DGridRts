using System;
using UnityEngine;
using Rts2D.Building.SO;
using Rts2D.Building.Manager;
using Rts2D.Building.View;
using Rts2D.GameConfig.SO;
using Rts2D.Grid.Manager;
using Rts2D.InputHandling.Manager;

namespace Rts2D.Placement.Manager
{
    /// <summary>
    /// Manages the see-through preview building that follows the cursor while placing.
    /// Knows where it snaps on the grid and whether that area is free, but not when to place it.
    /// </summary>
    public class GhostBuildingManager : IDisposable
    {
        private readonly GridManager _grid;
        private readonly MouseInputService _mouseInput;
        private readonly BuildingPoolManager _pool;
        private readonly GameConfigSO _gameConfig;

        private BuildingView _view;

        public bool IsActive => _view != null;
        public BuildingSO Data { get; private set; }
        public Vector2Int OriginCell { get; private set; }
        public bool CanPlace { get; private set; }

        public GhostBuildingManager(GridManager grid, MouseInputService mouseInput,
            BuildingPoolManager pool, GameConfigSO gameConfig)
        {
            _grid = grid;
            _mouseInput = mouseInput;
            _pool = pool;
            _gameConfig = gameConfig;
        }

        /// <summary>Takes a building of the given type from the pool and snaps it to the cursor.</summary>
        public void Show(BuildingSO data)
        {
            Hide();

            Data = data;
            _view = _pool.Spawn(data);
            FollowCursor(); // snap to the cursor right away, avoids a frame at the old position
        }

        /// <summary>Returns the ghost to the pool (placement cancelled).</summary>
        public void Hide()
        {
            if (_view == null) return;

            _pool.Despawn(_view);
            _view = null;
        }

        /// <summary>Snaps the ghost to the grid under the cursor and tints it by whether the area is free.</summary>
        public void FollowCursor()
        {
            Vector2Int size = Data.Size;

            OriginCell = _grid.GetBuildingOrigin(_mouseInput.GetWorldPosition(), size);
            CanPlace = _grid.IsAreaFree(OriginCell, size);

            _view.transform.position = _grid.GetBuildingCenter(OriginCell, size);
            _view.SetTint(CanPlace ? _gameConfig.ValidPlacementColor : _gameConfig.InvalidPlacementColor);
        }

        /// <summary>Hands the ghost over as a real building: clears the tint and stops tracking it.</summary>
        public BuildingView Release()
        {
            BuildingView view = _view;
            view.SetTint(Color.white);
            _view = null;
            return view;
        }

        public void Dispose()
        {
            _view = null; // the scene is closing, nothing to return to the pool
        }
    }
}
