using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Grid.Manager
{
    /// <summary>A* pathfinding on the grid. Units move only in 4 directions, so the heuristic is Manhattan distance.</summary>
    public class GridPathfinderManager
    {
        // Cost of moving one cell; every step costs the same on this grid
        private const int StepCost = 1;

        // Right, left, up, down (no diagonal movement)
        private static readonly Vector2Int[] NeighborOffsets =
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        private readonly GridManager _grid;

        public GridPathfinderManager(GridManager grid) => _grid = grid;

        /// <summary>Cells from start (exclusive) to target (inclusive). Null if there is no path.</summary>
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
        {
            if (start == target) return new List<Vector2Int>();
            if (!IsWalkable(target)) return null;

            // Cells found but not yet expanded
            var cellsToVisit = new List<Vector2Int> { start };
            // For each reached cell, the cell we came from (used to rebuild the path)
            var previousCell = new Dictionary<Vector2Int, Vector2Int>();
            // Cheapest known number of steps from start to each cell (A* "g" cost)
            var costFromStart = new Dictionary<Vector2Int, int> { [start] = 0 };

            while (cellsToVisit.Count > 0)
            {
                Vector2Int current = FindMostPromisingCell(cellsToVisit, costFromStart, target);

                if (current == target)
                    return BuildPath(previousCell, start, target);

                cellsToVisit.Remove(current);

                foreach (Vector2Int offset in NeighborOffsets)
                {
                    Vector2Int neighbor = current + offset;
                    if (!IsWalkable(neighbor)) continue;

                    int newCost = costFromStart[current] + StepCost;
                    if (costFromStart.TryGetValue(neighbor, out int knownCost) && newCost >= knownCost)
                        continue; // already reached this cell by an equal or shorter route

                    costFromStart[neighbor] = newCost;
                    previousCell[neighbor] = current;
                    if (!cellsToVisit.Contains(neighbor)) cellsToVisit.Add(neighbor);
                }
            }
            return null;
        }

        /// <summary>Picks the cell with the lowest estimated total cost (steps so far + remaining estimate).</summary>
        private Vector2Int FindMostPromisingCell(
            List<Vector2Int> cells, Dictionary<Vector2Int, int> costFromStart, Vector2Int target)
        {
            Vector2Int best = cells[0];
            int bestTotalCost = costFromStart[best] + EstimateRemainingCost(best, target);

            foreach (Vector2Int cell in cells)
            {
                int totalCost = costFromStart[cell] + EstimateRemainingCost(cell, target);
                if (totalCost < bestTotalCost)
                {
                    best = cell;
                    bestTotalCost = totalCost;
                }
            }
            return best;
        }

        private bool IsWalkable(Vector2Int cell)
            => _grid.InBounds(cell.x, cell.y) && !_grid.IsOccupied(cell.x, cell.y);

        /// <summary>Manhattan distance: the minimum number of steps when only 4-direction moves are allowed.</summary>
        private int EstimateRemainingCost(Vector2Int from, Vector2Int to)
            => (Mathf.Abs(from.x - to.x) + Mathf.Abs(from.y - to.y)) * StepCost;

        /// <summary>Walks back from target to start through previousCell, then reverses to get start→target order.</summary>
        private List<Vector2Int> BuildPath(
            Dictionary<Vector2Int, Vector2Int> previousCell, Vector2Int start, Vector2Int target)
        {
            var path = new List<Vector2Int>();
            for (Vector2Int cell = target; cell != start; cell = previousCell[cell])
                path.Add(cell);

            path.Reverse();
            return path;
        }
    }
}
