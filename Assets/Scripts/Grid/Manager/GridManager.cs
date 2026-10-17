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

        // ---------- Kayıt (yaz / sil / taşı) ----------

        /// <summary>Varlığın kapladığı tüm hücrelere kendini yazar.</summary>
        public void PlaceOccupant(IOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
                for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                    SetOccupant(x, y, occupant);
        }

        /// <summary>Varlığın kapladığı hücreleri boşaltır. Sadece kendi kaydını siler.</summary>
        public void RemoveOccupant(IOccupant occupant)
        {
            for (int x = occupant.MinCell.x; x <= occupant.MaxCell.x; x++)
                for (int y = occupant.MinCell.y; y <= occupant.MaxCell.y; y++)
                    ClearOccupant(x, y, occupant);
        }

        /// <summary>Hedef doluysa false döner ve hiçbir şey değişmez.</summary>
        public bool TryMoveOccupant(Vector2Int from, Vector2Int to, IOccupant occupant)
        {
            if (IsOccupied(to.x, to.y))
                return false;

            ClearOccupant(from.x, from.y, occupant);
            SetOccupant(to.x, to.y, occupant);
            return true;
        }

        // ---------- Sorgular ----------

        public IOccupant GetOccupant(int x, int y)
            => InBounds(x, y) ? _cells[x, y].Occupant : null;

        public bool InBounds(int x, int y)
            => x >= 0 && x < _gameConfigSo.GridWidth && y >= 0 && y < _gameConfigSo.GridHeight;

        /// <summary>Sınır dışı hücreler dolu sayılır.</summary>
        public bool IsOccupied(int x, int y)
            => !InBounds(x, y) || _cells[x, y].IsOccupied;

        /// <summary>Verilen dikdörtgen alandaki tüm hücreler boş ve sınır içinde mi?</summary>
        public bool IsAreaFree(Vector2Int origin, Vector2Int size)
        {
            for (int x = origin.x; x < origin.x + size.x; x++)
                for (int y = origin.y; y < origin.y + size.y; y++)
                    if (IsOccupied(x, y))
                        return false;

            return true;
        }

        // ---------- Koordinat dönüşümleri ----------

        public void GetXYPosition(Vector3 worldPosition, out int x, out int y)
        {
            x = Mathf.FloorToInt(worldPosition.x / CellSize);
            y = Mathf.FloorToInt(worldPosition.y / CellSize);
        }

        /// <summary>Hücrenin dünya merkezi.</summary>
        public Vector3 GetCellCenter(Vector2Int cell)
            => new Vector3((cell.x + 0.5f) * CellSize, (cell.y + 0.5f) * CellSize, 0f);

        /// <summary>
        /// Fare binanın ortasına gelecek şekilde sol alt hücreyi döndürür.
        /// Merkez, boyuta göre hücre ortasına (tek) ya da hücre köşesine (çift) en yakın noktaya oturur.
        /// </summary>
        public Vector2Int GetBuildingOrigin(Vector3 worldPosition, Vector2Int size)
        {
            int x = Mathf.FloorToInt(worldPosition.x / CellSize - size.x / 2f + 0.5f);
            int y = Mathf.FloorToInt(worldPosition.y / CellSize - size.y / 2f + 0.5f);
            return new Vector2Int(x, y);
        }

        /// <summary>Footprint'in dünya merkezi (sprite pivot'u Center ise).</summary>
        public Vector3 GetBuildingCenter(Vector2Int origin, Vector2Int size)
            => new Vector3(
                (origin.x + size.x / 2f) * CellSize,
                (origin.y + size.y / 2f) * CellSize,
                0f);

        // ---------- İç yazma (dışarıya kapalı) ----------

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