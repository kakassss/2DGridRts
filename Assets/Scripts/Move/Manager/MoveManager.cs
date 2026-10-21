using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Move.Manager
{
    /// <summary>Moves units cell by cell along A* paths and keeps their routes.</summary>
    public class MoveManager : ITickable
    {
        // Squared distance (0.01 units) at which a unit is on the cell center
        private const float ArrivalSqrDistance = 0.0001f;

        /// <summary>Destination and the cells left to walk.</summary>
        private class Route
        {
            public readonly Queue<Vector2Int> Cells = new Queue<Vector2Int>();
            public Vector2Int Destination;
        }

        private readonly GridManager _grid;
        private readonly GridPathfinderManager _pathfinder;
        private readonly List<IMovable> _movingUnits = new List<IMovable>();

        // One route per unit, reused (soldiers are pooled)
        private readonly Dictionary<IMovable, Route> _routes = new Dictionary<IMovable, Route>();

        public MoveManager(GridManager grid, GridPathfinderManager pathfinder)
        {
            _grid = grid;
            _pathfinder = pathfinder;
        }

        /// <summary>Starts moving to the cell. Returns false if it cannot be reached.</summary>
        public bool MoveTo(IMovable unit, Vector2Int targetCell)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, targetCell);
            if (path == null || path.Count == 0) return false;

            Route route = GetRoute(unit);
            route.Cells.Clear();
            EnqueueCurrentCellIfOffCenter(unit, route); // finish the half step first
            EnqueuePath(route, path);
            route.Destination = targetCell;

            StartTracking(unit);
            return true;
        }

        /// <summary>Drops the path but lets the unit reach its cell center.</summary>
        public void Stop(IMovable unit)
        {
            Route route = GetRoute(unit);
            route.Cells.Clear();
            EnqueueCurrentCellIfOffCenter(unit, route);

            if (route.Cells.Count == 0)
                _movingUnits.Remove(unit);
            else
                StartTracking(unit);
        }

        /// <summary>Removes the unit right away (on despawn).</summary>
        public void Remove(IMovable unit)
        {
            if (_routes.TryGetValue(unit, out Route route))
                route.Cells.Clear();

            _movingUnits.Remove(unit);
        }

        /// <summary>True while the unit still has cells to walk.</summary>
        public bool IsMoving(IMovable unit)
            => _routes.TryGetValue(unit, out Route route) && route.Cells.Count > 0;

        /// <summary>True when the unit has no path and stands on its cell center.</summary>
        public bool IsSettled(IMovable unit)
            => !IsMoving(unit) && IsAtCellCenter(unit);

        public void Tick()
        {
            // Backwards, so units can be removed while looping
            for (int i = _movingUnits.Count - 1; i >= 0; i--)
            {
                IMovable unit = _movingUnits[i];
                Route route = GetRoute(unit);

                if (!unit.GameObject.activeSelf || route.Cells.Count == 0)
                {
                    _movingUnits.RemoveAt(i);
                    continue;
                }

                Vector2Int nextCell = route.Cells.Peek();

                if (!TryReserveCell(unit, nextCell))
                {
                    RecalculatePath(unit, route, i);
                    continue;
                }

                StepTowards(unit, route, nextCell);
            }
        }

        /// <summary>Reserves the next cell before walking there. False if it is taken.</summary>
        private bool TryReserveCell(IMovable unit, Vector2Int nextCell)
        {
            if (nextCell == unit.Cell) return true; // already owns it

            if (!_grid.TryMoveOccupant(unit.Cell, nextCell, unit)) return false;

            unit.SetCell(nextCell);
            return true;
        }

        /// <summary>Steps toward the cell center; pops the cell once reached.</summary>
        private void StepTowards(IMovable unit, Route route, Vector2Int cell)
        {
            Transform unitTransform = unit.GameObject.transform;
            Vector3 targetPosition = _grid.GetCellCenter(cell);

            unitTransform.position = Vector3.MoveTowards(
                unitTransform.position, targetPosition, unit.MoveSpeed * Time.deltaTime);

            if ((unitTransform.position - targetPosition).sqrMagnitude < ArrivalSqrDistance)
                route.Cells.Dequeue();
        }

        /// <summary>Next cell got blocked: searches a new path to the destination.</summary>
        private void RecalculatePath(IMovable unit, Route route, int movingIndex)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, route.Destination);
            route.Cells.Clear();
            EnqueueCurrentCellIfOffCenter(unit, route);

            if (path == null || path.Count == 0)
            {
                // No way through: stop once on the cell center
                if (route.Cells.Count == 0) _movingUnits.RemoveAt(movingIndex);
                return;
            }

            EnqueuePath(route, path);
        }

        /// <summary>Queues the unit's own cell first if it is off its center.</summary>
        private void EnqueueCurrentCellIfOffCenter(IMovable unit, Route route)
        {
            if (!IsAtCellCenter(unit))
                route.Cells.Enqueue(unit.Cell);
        }

        private bool IsAtCellCenter(IMovable unit)
        {
            Vector3 center = _grid.GetCellCenter(unit.Cell);
            return (unit.GameObject.transform.position - center).sqrMagnitude <= ArrivalSqrDistance;
        }

        private Route GetRoute(IMovable unit)
        {
            if (!_routes.TryGetValue(unit, out Route route))
            {
                route = new Route();
                _routes.Add(unit, route);
            }
            return route;
        }

        private static void EnqueuePath(Route route, List<Vector2Int> path)
        {
            foreach (Vector2Int cell in path)
                route.Cells.Enqueue(cell);
        }

        private void StartTracking(IMovable unit)
        {
            if (!_movingUnits.Contains(unit))
                _movingUnits.Add(unit);
        }
    }
}
