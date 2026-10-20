using UnityEngine;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Utils
{
    /// <summary>Finds the nearest free cell adjacent to a target.</summary>
    public class ApproachCellFinderService
    {
        private readonly GridManager _grid;

        public ApproachCellFinderService(GridManager grid) => _grid = grid;

        public bool TryFind(Vector2Int from, IOccupant target, out Vector2Int result)
        {
            int best = int.MaxValue;
            result = default;
            bool found = false;

            for (int x = target.MinCell.x - 1; x <= target.MaxCell.x + 1; x++)
            {
                for (int y = target.MinCell.y - 1; y <= target.MaxCell.y + 1; y++)
                {
                    var cell = new Vector2Int(x, y);
                    if (_grid.IsOccupied(x, y)) continue; // out-of-bounds counts as occupied
                    if (!OccupantGeometry.IsAdjacent(cell, target)) continue;

                    int dist = Mathf.Abs(x - from.x) + Mathf.Abs(y - from.y);
                    if (dist < best) { best = dist; result = cell; found = true; }
                }
            }
            return found;
        }
    }
}