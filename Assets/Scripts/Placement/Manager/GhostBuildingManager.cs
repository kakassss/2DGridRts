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
    /// <summary>The see-through preview building that follows the cursor while placing.</summary>
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

        /// <summary>Takes the building from the pool and snaps it to the cursor.</summary>
        public void Show(BuildingSO data)
        {
            Hide();

            Data = data;
            _view = _pool.Spawn(data);
            FollowCursor(); // avoids one frame at the old position
        }

        /// <summary>Returns the ghost to the pool (placement cancelled).</summary>
        public void Hide()
        {
            if (_view == null) return;

            _pool.Despawn(_view);
            _view = null;
        }

        /// <summary>Snaps to the grid under the cursor; green if free, red if not.</summary>
        public void FollowCursor()
        {
            Vector2Int size = Data.Size;

            OriginCell = _grid.GetBuildingOrigin(_mouseInput.GetWorldPosition(), size);
            CanPlace = _grid.IsAreaFree(OriginCell, size);

            _view.transform.position = _grid.GetBuildingCenter(OriginCell, size);
            _view.SetTint(CanPlace ? _gameConfig.ValidPlacementColor : _gameConfig.InvalidPlacementColor);
        }

        /// <summary>Hands the ghost over as a real building.</summary>
        public BuildingView Release()
        {
            BuildingView view = _view;
            view.SetTint(Color.white);
            _view = null;
            return view;
        }

        public void Dispose()
        {
            _view = null; // scene is closing, nothing to pool
        }
    }
}
