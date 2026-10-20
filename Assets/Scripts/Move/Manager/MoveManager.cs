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
    /// </summary>
    public class MoveManager : ITickable
    {
        // Squared distance (world units) under which a unit counts as standing on a cell center.
        // Squared so we can skip the square root; 0.0001 = 0.01 units.
        private const float ArrivalSqrDistance = 0.0001f;

        private readonly GridManager _grid;
        private readonly GridPathfinderManager _pathfinder;
        private readonly List<IMovable> _movingUnits = new List<IMovable>();

        public MoveManager(GridManager grid, GridPathfinderManager pathfinder)
        {
            _grid = grid;
            _pathfinder = pathfinder;
        }

        /// <summary>Finds a path to the target cell and starts moving. Does nothing if no path exists.</summary>
        public void MoveTo(IMovable unit, Vector2Int targetCell)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, targetCell);
            if (path == null || path.Count == 0) return;

            unit.Path.Clear();
            EnqueueCurrentCellIfOffCenter(unit); // finish the half-completed step first
            EnqueuePath(unit, path);
            unit.Destination = targetCell;

            StartTracking(unit);
        }

        /// <summary>Drops the remaining path but lets the unit finish walking onto its current cell center.</summary>
        public void Stop(IMovable unit)
        {
            unit.Path.Clear();
            EnqueueCurrentCellIfOffCenter(unit);

            if (unit.Path.Count == 0)
                _movingUnits.Remove(unit);
            else
                StartTracking(unit);
        }

        /// <summary>Drops the unit from movement right away (used when it despawns).</summary>
        public void Remove(IMovable unit)
        {
            unit.Path.Clear();
            _movingUnits.Remove(unit);
        }

        /// <summary>True when the unit has no pending path and stands exactly on its cell center.</summary>
        public bool IsSettled(IMovable unit)
            => unit.Path.Count == 0 && IsAtCellCenter(unit);

        public void Tick()
        {
            // Iterate backwards so units can be removed while looping
            for (int i = _movingUnits.Count - 1; i >= 0; i--)
            {
                IMovable unit = _movingUnits[i];

                if (!unit.GameObject.activeSelf || unit.Path.Count == 0)
                {
                    _movingUnits.RemoveAt(i);
                    continue;
                }

                Vector2Int nextCell = unit.Path.Peek();

                if (!TryReserveCell(unit, nextCell))
                {
                    RecalculatePath(unit, i);
                    continue;
                }

                StepTowards(unit, nextCell);
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

        /// <summary>Moves the unit toward the cell center and pops the cell from the path once reached.</summary>
        private void StepTowards(IMovable unit, Vector2Int cell)
        {
            Transform unitTransform = unit.GameObject.transform;
            Vector3 targetPosition = _grid.GetCellCenter(cell);

            unitTransform.position = Vector3.MoveTowards(
                unitTransform.position, targetPosition, unit.MoveSpeed * Time.deltaTime);

            if ((unitTransform.position - targetPosition).sqrMagnitude < ArrivalSqrDistance)
                unit.Path.Dequeue();
        }

        /// <summary>Called when the next cell got blocked: searches a new path to the same destination.</summary>
        private void RecalculatePath(IMovable unit, int movingIndex)
        {
            List<Vector2Int> path = _pathfinder.FindPath(unit.Cell, unit.Destination);
            unit.Path.Clear();
            EnqueueCurrentCellIfOffCenter(unit);

            if (path == null || path.Count == 0)
            {
                // No way through: stop, but only once it stands on its cell center
                if (unit.Path.Count == 0) _movingUnits.RemoveAt(movingIndex);
                return;
            }

            EnqueuePath(unit, path);
        }

        /// <summary>If the unit is not at the center of its own cell, queues that cell first so it gets there.</summary>
        private void EnqueueCurrentCellIfOffCenter(IMovable unit)
        {
            if (!IsAtCellCenter(unit))
                unit.Path.Enqueue(unit.Cell);
        }

        private bool IsAtCellCenter(IMovable unit)
        {
            Vector3 center = _grid.GetCellCenter(unit.Cell);
            return (unit.GameObject.transform.position - center).sqrMagnitude <= ArrivalSqrDistance;
        }

        private static void EnqueuePath(IMovable unit, List<Vector2Int> path)
        {
            foreach (Vector2Int cell in path)
                unit.Path.Enqueue(cell);
        }

        private void StartTracking(IMovable unit)
        {
            if (!_movingUnits.Contains(unit))
                _movingUnits.Add(unit);
        }
    }
}
