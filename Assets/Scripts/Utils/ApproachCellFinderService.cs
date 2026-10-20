using System.Collections.Generic;
using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Utils
{
    /// <summary>Lists the free cells within attack range of a target, so a unit can walk there.</summary>
    public class ApproachCellFinderService
    {
        private readonly GridManager _grid;

        public ApproachCellFinderService(GridManager grid) => _grid = grid;

        /// <summary>
        /// Fills result with every free cell at most "range" cells from the target, nearest to "from" first.
        /// Range 1 means the cells right next to the target.
        /// The list is cleared first so the caller can reuse it without allocating.
        /// </summary>
        public void FindCells(Vector2Int from, IOccupant target, int range, List<Vector2Int> result)
        {
            result.Clear();

            for (int x = target.MinCell.x - range; x <= target.MaxCell.x + range; x++)
            {
                for (int y = target.MinCell.y - range; y <= target.MaxCell.y + range; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (_grid.IsOccupied(x, y)) continue; // out-of-bounds counts as occupied
                    if (OccupantGeometry.DistanceTo(cell, target) > range) continue;

                    InsertByDistance(result, cell, from);
                }
            }
        }

        // Keeps the list sorted by distance; equal distances stay in the order they were found
        private void InsertByDistance(List<Vector2Int> cells, Vector2Int cell, Vector2Int from)
        {
            int distance = ManhattanDistance(cell, from);
            int index = cells.Count;
            while (index > 0 && ManhattanDistance(cells[index - 1], from) > distance)
                index--;

            cells.Insert(index, cell);
        }

        private int ManhattanDistance(Vector2Int a, Vector2Int b)
            => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);
    }
}