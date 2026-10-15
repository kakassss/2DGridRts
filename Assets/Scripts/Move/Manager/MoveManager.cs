using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.GameConfig.DataSO;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Move.Manager
{
    public class MoveManager : ITickable
    {
        private const float ArriveSqrDistance = 0.0001f;

        private readonly GridManager _grid;
        private readonly GridPathfinderManager _gridPathfinderManager;
        private readonly GameConfigSO _config;
        private readonly List<IMovable> _moving = new List<IMovable>();

        public MoveManager(GridManager grid, GridPathfinderManager gridPathfinderManager, GameConfigSO config)
        {
            _grid = grid;
            _gridPathfinderManager = gridPathfinderManager;
            _config = config;
        }

        public void MoveTo(IMovable unit, Vector2Int target)
        {
            List<Vector2Int> path = _gridPathfinderManager.FindPath(unit.Cell, target);
            if (path == null || path.Count == 0) return;

            unit.Path.Clear();
            EnqueueCurrentCellIfOffCenter(unit); // yarım kalan adımı bitir
            foreach (Vector2Int c in path) unit.Path.Enqueue(c);
            unit.Destination = target;

            if (!_moving.Contains(unit)) _moving.Add(unit);
        }

        public void Stop(IMovable unit)
        {
            unit.Path.Clear();
            EnqueueCurrentCellIfOffCenter(unit); // merkeze oturana kadar hareket etsin

            if (unit.Path.Count == 0)
                _moving.Remove(unit);
            else if (!_moving.Contains(unit))
                _moving.Add(unit);
        }

        public void Tick()
        {
            for (int i = _moving.Count - 1; i >= 0; i--)
            {
                IMovable u = _moving[i];

                if (!u.GameObject.activeSelf || u.Path.Count == 0)
                {
                    _moving.RemoveAt(i);
                    continue;
                }

                Vector2Int nextCell = u.Path.Peek();

                // Yeni hücreyi devral (kendi hücresi zaten devralınmış)
                if (nextCell != u.Cell)
                {
                    if (!_grid.TryMoveOccupant(u.Cell, nextCell, u))
                    {
                        Repath(u, i);
                        continue;
                    }
                    u.SetCell(nextCell);
                }

                Transform t = u.GameObject.transform;
                Vector3 targetPos = CellCenter(nextCell);
                t.position = Vector3.MoveTowards(t.position, targetPos, u.MoveSpeed * Time.deltaTime);

                if ((t.position - targetPos).sqrMagnitude < ArriveSqrDistance)
                    u.Path.Dequeue();
            }
        }

        private void Repath(IMovable u, int index)
        {
            List<Vector2Int> path = _gridPathfinderManager.FindPath(u.Cell, u.Destination);
            u.Path.Clear();
            EnqueueCurrentCellIfOffCenter(u);

            if (path == null || path.Count == 0)
            {
                if (u.Path.Count == 0) _moving.RemoveAt(index);
                return;
            }

            foreach (Vector2Int c in path)
                u.Path.Enqueue(c);
        }

        /// <summary>Birim kendi hücresinin merkezinde değilse, önce oraya varması için hücresini kuyruğa koyar.</summary>
        private void EnqueueCurrentCellIfOffCenter(IMovable u)
        {
            Vector3 center = CellCenter(u.Cell);
            if ((u.GameObject.transform.position - center).sqrMagnitude > ArriveSqrDistance)
                u.Path.Enqueue(u.Cell);
        }

        private Vector3 CellCenter(Vector2Int c)
        {
            float s = _config.CellSize;
            return new Vector3((c.x + 0.5f) * s, (c.y + 0.5f) * s, 0f);
        }
    }
}
