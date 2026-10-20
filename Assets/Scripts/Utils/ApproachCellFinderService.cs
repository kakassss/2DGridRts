using System.Collections.Generic;
using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Utils
{
    /// <summary>Lists the free cells next to a target, so a unit can walk up to it.</summary>
    public class ApproachCellFinderService
    {
        private readonly GridManager _grid;

        public ApproachCellFinderService(GridManager grid) => _grid = grid;

        /// <summary>
        /// Fills result with every free cell next to the target, nearest to "from" first.
        /// The list is cleared first so the caller can reuse it without allocating.
        /// </summary>
        public void FindCells(Vector2Int from, IOccupant target, List<Vector2Int> result)
        {
            result.Clear();

            for (int x = target.MinCell.x - 1; x <= target.MaxCell.x + 1; x++)
            {
                for (int y = target.MinCell.y - 1; y <= target.MaxCell.y + 1; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (_grid.IsOccupied(x, y)) continue; // out-of-bounds counts as occupied
                    if (!OccupantGeometry.IsAdjacent(cell, target)) continue;

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