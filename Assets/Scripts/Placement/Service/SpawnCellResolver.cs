using UnityEngine;
using Rts2D.Building.SO;
using Rts2D.Grid.Manager;

namespace Rts2D.Placement.Service
{
    /// <summary>Decides which cell a building's units come out of.</summary>
    public class SpawnCellResolver
    {
        private readonly GridManager _grid;

        public SpawnCellResolver(GridManager grid) => _grid = grid;

        /// <summary>
        /// The designated spawn cell (origin + SpawnOffset). If it falls outside the grid, it is moved
        /// to the middle of another side of the building so the spawn point is always on the board.
        /// </summary>
        public Vector2Int Resolve(Vector2Int origin, BuildingSO data)
        {
            Vector2Int preferred = origin + data.SpawnOffset;
            if (_grid.InBounds(preferred.x, preferred.y)) return preferred;

            Vector2Int size = data.Size;
            int midX = origin.x + size.x / 2;
            int midY = origin.y + size.y / 2;

            Vector2Int[] fallbacks =
            {
                new Vector2Int(midX, origin.y + size.y), // above
                new Vector2Int(midX, origin.y - 1),      // below
                new Vector2Int(origin.x - 1, midY),      // left
                new Vector2Int(origin.x + size.x, midY)  // right
            };

            foreach (Vector2Int cell in fallbacks)
            {
                if (_grid.InBounds(cell.x, cell.y))
                    return cell;
            }

            return preferred; // no side is on the board; the spawn search still finds the nearest free cell
        }
    }
}
