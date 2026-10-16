using Rts2D.GameConfig.DataSO;
using UnityEngine;
using Rts2D.Shared.Contracts;

namespace Rts2D.Grid.Manager
{
    public class GridNode
    {
        public bool IsOccupied;
        public ISelectable Occupant;
        
        public GridNode(bool isOccupied) => IsOccupied = isOccupied;
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

        /// <summary>Hücreyi sadece occupant hâlâ o ise boşaltır. Başkasının kaydını silmez.</summary>
        public void ClearOccupant(int x, int y, ISelectable occupant)
        {
            if (!InBounds(x, y)) return;
            if (!ReferenceEquals(_grid[x, y].Occupant, occupant)) return;

            _grid[x, y].Occupant = null;
            _grid[x, y].IsOccupied = false;
        }

        /// <summary>Varlığın kapladığı tüm hücrelere kendini yazar.</summary>
        public void PlaceOccupant(IGridOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
            for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                SetOccupant(x, y, occupant);
        }

        /// <summary>Varlığın kapladığı hücreleri boşaltır. Sadece kendi kaydını siler.</summary>
        public void RemoveOccupant(IGridOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
            for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                ClearOccupant(x, y, occupant);
        }

        public ISelectable GetOccupant(int x, int y)
            => InBounds(x, y) ? _grid[x, y].Occupant : null;

        public bool TryMoveOccupant(Vector2Int from, Vector2Int to, ISelectable occupant)
        {
            if (GetPositionIsOccupied(to.x, to.y))
                return false;

            ClearOccupant(from.x, from.y, occupant);
            SetOccupant(to.x, to.y, occupant);
            return true;
        }

        public Vector3 GetWorldPosition(int x, int y)
            => new Vector3(x, y, 0) * CellSize;

        public void GetXYPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / CellSize);
            y = Mathf.FloorToInt(worldPosition.y / CellSize);
        }

        public bool InBounds(int x, int y)
            => x >= 0 && x < _gameConfigSo.GridWidth && y >= 0 && y < _gameConfigSo.GridHeight;

        /// <summary>Sınır dışı hücreler dolu sayılır.</summary>
        public bool GetPositionIsOccupied(int x, int y)
            => !InBounds(x, y) || _grid[x, y].IsOccupied;

        /// <summary>Fare binanın ortasına gelecek şekilde sol alt hücreyi döndürür.</summary>
        public Vector2Int GetBuildingOrigin(Vector3 worldPosition, Vector2Int size)
        {
            int x = Mathf.FloorToInt(worldPosition.x / CellSize - size.x / 2f + 0.5f);
            int y = Mathf.FloorToInt(worldPosition.y / CellSize - size.y / 2f + 0.5f);
            return new Vector2Int(x, y);
        }

        /// <summary>Footprint'in dünya merkezi (sprite pivot'u Center ise).</summary>
        public Vector3 GetBuildingCenter(Vector2Int origin, Vector2Int size)
        {
            return new Vector3(
                (origin.x + size.x / 2f) * CellSize,
                (origin.y + size.y / 2f) * CellSize,
                0f);
        }
    }
}