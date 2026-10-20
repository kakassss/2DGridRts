using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Utils;
using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Combat.Manager
{
    /// <summary>
    /// Runs attack orders: the unit walks into attack range of its target, then hits it every attack interval
    /// until the target dies, the order is cancelled, or the target can no longer be reached.
    /// </summary>
    public class AttackManager : ITickable, IInitializable, IDisposable
    {
        private enum OrderStatus
        {
            Active,
            Finished
        }

        private class AttackOrder
        {
            // Placeholder so the first update always searches for a path
            public static readonly Vector2Int NoCell = new Vector2Int(int.MinValue, int.MinValue);

            // Unit and Attacker are the same soldier, seen as something that moves and something that hits
            public IMovable Unit;
            public IAttacker Attacker;
            public IDamageable Target;

            public float NextAttackTime;
            public Vector2Int LastTargetCell = NoCell; // where the target was at the last path search
        }

        private readonly MoveManager _move;
        private readonly ApproachCellFinderService _cellFinder;
        private readonly IDamageableTracker _damageableTracker;
        private readonly List<AttackOrder> _orders = new List<AttackOrder>();

        // Reused buffer for the free cells around a target, so chasing does not allocate
        private readonly List<Vector2Int> _approachCells = new List<Vector2Int>();

        // Hits collected during the loop; applied once the loop has finished
        private readonly List<(IDamageable target, int damage)> _pendingHits
            = new List<(IDamageable, int)>();

        public AttackManager(MoveManager move, ApproachCellFinderService cellFinder,
            IDamageableTracker damageableTracker)
        {
            _move = move;
            _cellFinder = cellFinder;
            _damageableTracker = damageableTracker;
        }

        public void Initialize() => _damageableTracker.Destroyed += HandleDestroyed;

        public void Dispose() => _damageableTracker.Destroyed -= HandleDestroyed;

        // ---------- Orders ----------

        /// <summary>Gives the unit a new attack order, replacing any order it already has.</summary>
        public void Attack(IMovable unit, IDamageable target)
        {
            if (unit is not IAttacker attacker || target == null || target.IsDead) return;

            Cancel(unit);
            _orders.Add(new AttackOrder { Unit = unit, Attacker = attacker, Target = target });
        }

        public void Cancel(IMovable unit) => _orders.RemoveAll(order => order.Unit == unit);

        /// <summary>A dead target ends every order chasing it; a dead unit's own order ends too.</summary>
        private void HandleDestroyed(IDamageable dead)
            => _orders.RemoveAll(order => order.Target == dead || ReferenceEquals(order.Unit, dead));

        public void Tick()
        {
            // Iterate backwards so finished orders can be removed while looping
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                if (UpdateOrder(_orders[i]) == OrderStatus.Finished)
                    _orders.RemoveAt(i);
            }

            ApplyPendingHits(); // loop finished, so the death chain can safely run
        }

        private OrderStatus UpdateOrder(AttackOrder order)
        {
            if (order.Target.IsDead || !order.Unit.GameObject.activeSelf)
                return OrderStatus.Finished;

            if (IsInRange(order) && !IsTargetMoving(order))
            {
                HitWhenReady(order);
                return OrderStatus.Active;
            }

            return ChaseTarget(order);
        }

        // ---------- Hitting ----------

        /// <summary>Hits the target once the unit has settled on its cell and the attack interval has passed.</summary>
        private void HitWhenReady(AttackOrder order)
        {
            // Never attack mid-movement: finish the current step first
            if (!_move.IsSettled(order.Unit))
            {
                _move.Stop(order.Unit);
                return;
            }

            if (Time.time < order.NextAttackTime) return;

            _pendingHits.Add((order.Target, order.Attacker.Damage));
            order.NextAttackTime = Time.time + order.Attacker.AttackInterval;
        }

        private void ApplyPendingHits()
        {
            foreach ((IDamageable target, int damage) in _pendingHits)
            {
                if (!target.IsDead)
                    target.TakeDamage(damage);
            }
            _pendingHits.Clear();
        }

        // ---------- Chasing ----------

        /// <summary>
        /// Walks into attack range. Searches for a path only when the target changed cell or the unit stopped,
        /// so an unchanged situation costs nothing. Finishes the order if the target cannot be reached.
        /// </summary>
        private OrderStatus ChaseTarget(AttackOrder order)
        {
            bool targetChangedCell = order.Target.MinCell != order.LastTargetCell;
            bool isInRange = IsInRange(order);

            // Nothing changed since the last search: keep walking, or keep waiting for a passing target in range
            if (!targetChangedCell && (_move.IsMoving(order.Unit) || isInRange))
                return OrderStatus.Active;

            order.LastTargetCell = order.Target.MinCell;

            if (TryWalkIntoRange(order))
                return OrderStatus.Active;

            // Already in range of a target that is still walking: wait for it to stop
            if (isInRange)
                return OrderStatus.Active;

            // No free cell in range of the target can be reached: give up instead of searching every frame
            _move.Stop(order.Unit);
            return OrderStatus.Finished;
        }

        /// <summary>Tries the free cells in range of the target, nearest first, and walks to the first reachable one.</summary>
        private bool TryWalkIntoRange(AttackOrder order)
        {
            _cellFinder.FindCells(order.Unit.Cell, order.Target, order.Attacker.AttackRange, _approachCells);

            foreach (Vector2Int cell in _approachCells)
            {
                if (_move.MoveTo(order.Unit, cell))
                    return true;
            }
            return false;
        }

        // ---------- Queries ----------

        private bool IsInRange(AttackOrder order)
            => OccupantGeometry.DistanceTo(order.Unit.Cell, order.Target) <= order.Attacker.AttackRange;

        private bool IsTargetMoving(AttackOrder order)
            => order.Target is IMovable movingTarget && _move.IsMoving(movingTarget);
    }
}
