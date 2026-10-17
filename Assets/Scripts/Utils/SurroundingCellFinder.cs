using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Grid.Util
{
    /// <summary>Bir varlığın etrafındaki halkada, en yakın boş hücreyi bulur.</summary>
    public class SurroundingCellFinder
    {
        private const int MaxDistance = 5;

        private readonly GridManager _grid;

        public SurroundingCellFinder(GridManager grid) => _grid = grid;

        public bool TryFind(IOccupant target, out Vector2Int result)
        {
            Vector2Int min = target.MinCell;
            Vector2Int max = target.MaxCell;

            for (int d = 1; d <= MaxDistance; d++)
            {
                for (int x = min.x - d; x <= max.x + d; x++)
                {
                    for (int y = min.y - d; y <= max.y + d; y++)
                    {
                        bool onRing = x == min.x - d || x == max.x + d
                                                     || y == min.y - d || y == max.y + d;

                        if (!onRing || _grid.IsOccupied(x, y)) continue;

                        result = new Vector2Int(x, y);
                        return true;
                    }
                }
            }

            result = default;
            return false;
        }
    }
}