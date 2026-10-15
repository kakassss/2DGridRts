using UnityEngine;
using Rts2D.GameConfig.DataSO;
using Rts2D.Shared.Contracts;

namespace Rts2D.Grid.Manager
{
    public class GridNode
    {
        public bool IsOccupied;
        public ISelectable Occupant;

        public GridNode(bool isOccupied)
        {
            IsOccupied = isOccupied;
        }
    }

    public class GridManager : IGrid
    {
        private GridNode[,] _grid;
        private readonly GameConfigSO _gameConfigSo;

        public float CellSize => _gameConfigSo.CellSize;

        public GridManager(GameConfigSO gameConfigSo)
        {
            _gameConfigSo = gameConfigSo;
            CreateGrid();
        }

        private void CreateGrid()
        {
            _grid = new GridNode[_gameConfigSo.GridWidth, _gameConfigSo.GridHeight];

            for (int i = 0; i < _gameConfigSo.GridWidth; i++)
                for (int j = 0; j < _gameConfigSo.GridHeight; j++)
                    _grid[i, j] = new GridNode(false);
        }

        public void SetOccupant(int x, int y, ISelectable occupant)
        {
            if (!InBounds(x, y)) return;
            _grid[x, y].Occupant = occupant;
            _grid[x, y].IsOccupied = occupant != null;
        }

        /// <summary>Hücreyi sadece occupant hâlâ o ise boşaltır. Başkasının kaydını asla silmez.</summary>
        public void ClearOccupant(int x, int y, ISelectable occupant)
        {
            if (!InBounds(x, y)) return;

            if (!ReferenceEquals(_grid[x, y].Occupant, occupant))
            {
                if (_grid[x, y].Occupant != null) return;
            }

            _grid[x, y].Occupant = null;
            _grid[x, y].IsOccupied = false;
        }

        public ISelectable GetOccupant(int x, int y)
        {
            return InBounds(x, y) ? _grid[x, y].Occupant : null;
        }

        /// <summary>Hedef doluysa false döner ve hiçbir şey değişmez.</summary>
        public bool TryMoveOccupant(Vector2Int from, Vector2Int to, ISelectable occupant)
        {
            if (!InBounds(to.x, to.y) || GetPositionIsOccupied(to.x, to.y))
                return false;

            ClearOccupant(from.x, from.y, occupant);
            SetOccupant(to.x, to.y, occupant);
            return true;
        }

        public Vector3 GetWorldPosition(int x, int y)
        {
            return new Vector3(x, y, 0) * _gameConfigSo.CellSize;
        }

        public void GetXYPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / _gameConfigSo.CellSize);
            y = Mathf.FloorToInt(worldPosition.y / _gameConfigSo.CellSize);
        }

        public bool InBounds(int x, int y)
        {
            return x >= 0 && x < _gameConfigSo.GridWidth && y >= 0 && y < _gameConfigSo.GridHeight;
        }

        public bool GetPositionIsOccupied(int x, int y)
        {
            return !InBounds(x, y) || _grid[x, y].IsOccupied;
        }
    }
}
