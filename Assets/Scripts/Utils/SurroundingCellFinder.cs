using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Grid.Util
{
    /// <summary>Finds the nearest free cell around an occupant or around a single cell.</summary>
    public class SurroundingCellFinder
    {
        private readonly GridManager _grid;

        public SurroundingCellFinder(GridManager grid) => _grid = grid;

        /// <summary>Nearest free cell in the rings around the occupant's footprint.</summary>
        public bool TryFind(IOccupant target, out Vector2Int result)
            => TryFindRing(target.MinCell, target.MaxCell, 1, out result);

        /// <summary>The cell itself if free, otherwise the nearest free cell around it.</summary>
        public bool TryFindNear(Vector2Int cell, out Vector2Int result)
            => TryFindRing(cell, cell, 0, out result);

        private bool TryFindRing(Vector2Int min, Vector2Int max, int startDistance, out Vector2Int result)
        {
            // No fixed radius: rings grow until they cover the whole grid
            int maxDistance = Mathf.Max(_grid.Width, _grid.Height);

            for (int d = startDistance; d <= maxDistance; d++)
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
