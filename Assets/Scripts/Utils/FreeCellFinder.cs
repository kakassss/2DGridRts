using System.Collections.Generic;
using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Utils
{
    /// <summary>Finds the nearest free cells around an occupant or a cell.</summary>
    public class FreeCellFinder
    {
        private readonly GridManager _grid;

        public FreeCellFinder(GridManager grid) => _grid = grid;

        /// <summary>Nearest free cell around the occupant's footprint.</summary>
        public bool TryFind(IOccupant target, out Vector2Int result)
            => TryFindRing(target.MinCell, target.MaxCell, 1, out result);

        /// <summary>The cell itself if free, otherwise the nearest free cell around it.</summary>
        public bool TryFindNear(Vector2Int cell, out Vector2Int result)
            => TryFindRing(cell, cell, 0, out result);

        /// <summary>Up to "count" distinct free cells, nearest first (one per unit of a group).</summary>
        public void FindFreeCellsNear(Vector2Int cell, int count, List<Vector2Int> result)
        {
            result.Clear();

            // Rings grow until enough cells are found or the grid is covered
            int maxDistance = Mathf.Max(_grid.Width, _grid.Height);

            for (int d = 0; d <= maxDistance; d++)
            {
                for (int x = cell.x - d; x <= cell.x + d; x++)
                {
                    for (int y = cell.y - d; y <= cell.y + d; y++)
                    {
                        bool onRing = x == cell.x - d || x == cell.x + d || y == cell.y - d || y == cell.y + d;
                        if (!onRing || _grid.IsOccupied(x, y)) continue;

                        result.Add(new Vector2Int(x, y));
                        if (result.Count == count) return;
                    }
                }
            }
        }

        private bool TryFindRing(Vector2Int min, Vector2Int max, int startDistance, out Vector2Int result)
        {
            // Rings grow until they cover the whole grid
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
