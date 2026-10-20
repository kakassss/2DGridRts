using UnityEngine;
using Rts2D.Shared.Contracts;

namespace Rts2D.Utils
{
    public static class OccupantGeometry
    {
        /// <summary>Manhattan distance from the cell to the footprint (0 if inside).</summary>
        public static int DistanceTo(Vector2Int cell, IOccupant target)
        {
            int dx = Mathf.Max(target.MinCell.x - cell.x, 0, cell.x - target.MaxCell.x);
            int dy = Mathf.Max(target.MinCell.y - cell.y, 0, cell.y - target.MaxCell.y);
            return dx + dy;
        }
    }
}