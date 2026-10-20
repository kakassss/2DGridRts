using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Move.Manager
{
    /// <summary>
    /// Moves units cell by cell along A* paths. A unit reserves the next cell on the grid
    /// before it starts walking there, so two units can never head into the same cell.
    /// Each unit's path and destination are kept here, so no other system can change them.
    /// </summary>
    public class MoveManager : ITickable
    {
        // Squared distance (world units) under which a unit counts as standing on a cell center.
        // Squared so we can skip the square root; 0.0001 = 0.01 units.
        private const float ArrivalSqrDistance = 0.0001f;

        /// <summary>Where a unit is going and the cells left on the way there.</summary>
        private class Route
        {
            public readonly Queue<Vector2Int> Cells = new Queue<Vector2Int>();
            public Vector2Int Destination;
        }

        private readonly GridManager _grid;
        private readonly GridPathfinderManager _pathfinder;
        private readonly List<IMovable> _movingUnits = new List<IMovable>();

        // One route per unit, created on its first move and reused afterwards (soldiers are pooled)
        private readonly Dictionary<IMovable, Route> _routes = new Dictionary<IMovable, Route>();

        public MoveManager(GridManager grid, GridPathfinderManager pathfinder)
        {
            _grid = grid;
            _pathfinder = pathfinder;
        }

        /// <summary>
        /// Finds a path to the target cell and starts moving.
        /// Returns false and changes nothing if the cell cannot be reached.
        /// </summary>
        public bool MoveTo(IMovable unit, Vector2Int targetCell)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, targetCell);
            if (path == null || path.Count == 0) return false;

            Route route = GetRoute(unit);
            route.Cells.Clear();
            EnqueueCurrentCellIfOffCenter(unit, route); // finish the half-completed step first
            EnqueuePath(route, path);
            route.Destination = targetCell;

            StartTracking(unit);
            return true;
        }

        /// <summary>Drops the remaining path but lets the unit finish walking onto its current cell center.</summary>
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

        /// <summary>Drops the unit from movement right away (used when it despawns).</summary>
        public void Remove(IMovable unit)
        {
            if (_routes.TryGetValue(unit, out Route route))
                route.Cells.Clear();

            _movingUnits.Remove(unit);
        }

        /// <summary>True while the unit still has cells to walk.</summary>
        public bool IsMoving(IMovable unit)
            => _routes.TryGetValue(unit, out Route route) && route.Cells.Count > 0;

        /// <summary>True when the unit has no pending path and stands exactly on its cell center.</summary>
        public bool IsSettled(IMovable unit)
            => !IsMoving(unit) && IsAtCellCenter(unit);

        public void Tick()
        {
            // Iterate backwards so units can be removed while looping
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

        /// <summary>
        /// Moves the unit's grid occupancy to the next cell before it visually gets there.
        /// Returns false if another occupant already holds that cell.
        /// </summary>
        private bool TryReserveCell(IMovable unit, Vector2Int nextCell)
        {
            if (nextCell == unit.Cell) return true; // already owns its own cell

            if (!_grid.TryMoveOccupant(unit.Cell, nextCell, unit)) return false;

            unit.SetCell(nextCell);
            return true;
        }

        /// <summary>Moves the unit toward the cell center and pops the cell from the route once reached.</summary>
        private void StepTowards(IMovable unit, Route route, Vector2Int cell)
        {
            Transform unitTransform = unit.GameObject.transform;
            Vector3 targetPosition = _grid.GetCellCenter(cell);

            unitTransform.position = Vector3.MoveTowards(
                unitTransform.position, targetPosition, unit.MoveSpeed * Time.deltaTime);

            if ((unitTransform.position - targetPosition).sqrMagnitude < ArrivalSqrDistance)
                route.Cells.Dequeue();
        }

        /// <summary>Called when the next cell got blocked: searches a new path to the same destination.</summary>
        private void RecalculatePath(IMovable unit, Route route, int movingIndex)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, route.Destination);
            route.Cells.Clear();
            EnqueueCurrentCellIfOffCenter(unit, route);

            if (path == null || path.Count == 0)
            {
                // No way through: stop, but only once it stands on its cell center
                if (route.Cells.Count == 0) _movingUnits.RemoveAt(movingIndex);
                return;
            }

            EnqueuePath(route, path);
        }

        /// <summary>If the unit is not at the center of its own cell, queues that cell first so it gets there.</summary>
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
