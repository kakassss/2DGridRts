using UnityEngine;
using Rts2D.GameConfig.DataSO;
using Rts2D.Shared.Contracts;

namespace Rts2D.Grid.Manager
{
    public class GridManager
    {
        private class GridCell
        {
            public IOccupant Occupant;
            public bool IsOccupied => Occupant != null;
        }

        private readonly GridCell[,] _cells;
        private readonly GameConfigSO _gameConfigSo;

        private float CellSize => _gameConfigSo.CellSize;

        public GridManager(GameConfigSO gameConfigSo)
        {
            _gameConfigSo = gameConfigSo;
            _cells = new GridCell[gameConfigSo.GridWidth, gameConfigSo.GridHeight];

            for (int x = 0; x < gameConfigSo.GridWidth; x++)
                for (int y = 0; y < gameConfigSo.GridHeight; y++)
                    _cells[x, y] = new GridCell();
        }

        // ---------- Registration (place / remove / move) ----------

        /// <summary>Writes the occupant into every cell it covers.</summary>
        public void PlaceOccupant(IOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
                for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                    SetOccupant(x, y, occupant);
        }

        /// <summary>Frees the cells the occupant covers. Only removes its own entries.</summary>
        public void RemoveOccupant(IOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
                for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                    ClearOccupant(x, y, occupant);
        }

        /// <summary>Returns false and changes nothing if the target cell is occupied.</summary>
        public bool TryMoveOccupant(Vector2Int from, Vector2Int to, IOccupant occupant)
        {
            if (IsOccupied(to.x, to.y))
                return false;

            ClearOccupant(from.x, from.y, occupant);
            SetOccupant(to.x, to.y, occupant);
            return true;
        }

        // ---------- Queries ----------

        public IOccupant GetOccupant(int x, int y)
            => InBounds(x, y) ? _cells[x, y].Occupant : null;

        public bool InBounds(int x, int y)
            => x >= 0 && x < _gameConfigSo.GridWidth && y >= 0 && y < _gameConfigSo.GridHeight;

        /// <summary>Cells outside the grid count as occupied.</summary>
        public bool IsOccupied(int x, int y)
            => !InBounds(x, y) || _cells[x, y].IsOccupied;

        /// <summary>Are all cells of the given rectangle free and inside the grid?</summary>
        public bool IsAreaFree(Vector2Int origin, Vector2Int size)
        {
            for (int x = origin.x; x < origin.x + size.x; x++)
                for (int y = origin.y; y < origin.y + size.y; y++)
                    if (IsOccupied(x, y))
                        return false;

            return true;
        }

        // ---------- Coordinate conversions ----------

        public void GetXYPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / CellSize);
            y = Mathf.FloorToInt(worldPosition.y / CellSize);
        }

        /// <summary>World-space center of the cell.</summary>
        public Vector3 GetCellCenter(Vector2Int cell)
            => new Vector3((cell.x + 0.5f) * CellSize, (cell.y + 0.5f) * CellSize, 0f);

        /// <summary>
        /// Returns the bottom-left cell so that the mouse sits at the middle of the building.
        /// The center snaps to a cell middle (odd size) or to a cell corner (even size).
        /// </summary>
        public Vector2Int GetBuildingOrigin(Vector3 worldPosition, Vector2Int size)
        {
            int x = Mathf.FloorToInt(worldPosition.x / CellSize - size.x / 2f + 0.5f);
            int y = Mathf.FloorToInt(worldPosition.y / CellSize - size.y / 2f + 0.5f);
            return new Vector2Int(x, y);
        }

        /// <summary>World-space center of the footprint (assuming a Center sprite pivot).</summary>
        public Vector3 GetBuildingCenter(Vector2Int origin, Vector2Int size)
            => new Vector3(
                (origin.x + size.x / 2f) * CellSize,
                (origin.y + size.y / 2f) * CellSize,
                0f);

        // ---------- Internal writes (not exposed) ----------

        private void SetOccupant(int x, int y, IOccupant occupant)
        {
            if (!InBounds(x, y)) return;
            _cells[x, y].Occupant = occupant;
        }

        private void ClearOccupant(int x, int y, IOccupant occupant)
        {
            if (!InBounds(x, y)) return;
            if (!ReferenceEquals(_cells[x, y].Occupant, occupant)) return;

            _cells[x, y].Occupant = null;
        }
    }
}