using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Grid.Manager
{
    public class GridPathfinderManager
    {
        private static readonly Vector2Int[] Directions =
        {
            new Vector2Int(1, 0), new Vector2Int(-1, 0),
            new Vector2Int(0, 1), new Vector2Int(0, -1)
        };

        private readonly GridManager _grid;

        public GridPathfinderManager(GridManager grid) => _grid = grid;

        /// <summary>Başlangıç hariç, hedef dahil hücre listesi. Yol yoksa null.</summary>
        public List<Vector2Int> FindPath(Vector2Int start, Vector2Int target)
        {
            if (start == target) return new List<Vector2Int>();
            if (!_grid.InBounds(target.x, target.y) || _grid.GetPositionIsOccupied(target.x, target.y))
                return null;

            var open = new List<Vector2Int> { start };
            var cameFrom = new Dictionary<Vector2Int, Vector2Int>();
            var g = new Dictionary<Vector2Int, int> { [start] = 0 };

            while (open.Count > 0)
            {
                Vector2Int current = open[0];
                foreach (Vector2Int cell in open)
                    if (g[cell] + H(cell, target) < g[current] + H(current, target))
                        current = cell;

                if (current == target)
                    return Reconstruct(cameFrom, start, target);

                open.Remove(current);

                foreach (Vector2Int dir in Directions)
                {
                    Vector2Int next = current + dir;
                    if (!_grid.InBounds(next.x, next.y) || _grid.GetPositionIsOccupied(next.x, next.y))
                        continue;

                    int newG = g[current] + 1;
                    if (g.TryGetValue(next, out int old) && newG >= old) continue;

                    g[next] = newG;
                    cameFrom[next] = current;
                    if (!open.Contains(next)) open.Add(next);
                }
            }
            return null;
        }

        private static int H(Vector2Int a, Vector2Int b) => Mathf.Abs(a.x - b.x) + Mathf.Abs(a.y - b.y);

        private static List<Vector2Int> Reconstruct(Dictionary<Vector2Int, Vector2Int> cameFrom, Vector2Int start, Vector2Int end)
        {
            var path = new List<Vector2Int>();
            for (Vector2Int c = end; c != start; c = cameFrom[c]) path.Add(c);
            path.Reverse();
            return path;
        }
    }
}
