using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Utils;
using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Combat.Manager
{
    /// <summary>Walks units into range of their target and hits it every attack interval.</summary>
    public class AttackManager : ITickable, IInitializable, IDisposable
    {
        private enum OrderStatus
        {
            Active,
            Finished
        }

        private class AttackOrder
        {
            // Forces a path search on the first update
            public static readonly Vector2Int NoCell = new Vector2Int(int.MinValue, int.MinValue);

            // Same soldier, seen as a mover and as an attacker
            public IMovable Unit;
            public IAttacker Attacker;
            public IDamageable Target;

            public float NextAttackTime;
            public Vector2Int LastTargetCell = NoCell; // target cell at the last path search
        }

        private readonly MoveManager _move;
        private readonly ApproachCellFinder _cellFinder;
        private readonly IDamageableTracker _damageableTracker;
        private readonly List<AttackOrder> _orders = new List<AttackOrder>();

        // Reused buffer, so chasing does not allocate
        private readonly List<Vector2Int> _approachCells = new List<Vector2Int>();

        // Hits are applied after the loop
        private readonly List<(IDamageable target, int damage)> _pendingHits
            = new List<(IDamageable, int)>();

        public AttackManager(MoveManager move, ApproachCellFinder cellFinder,
            IDamageableTracker damageableTracker)
        {
            _move = move;
            _cellFinder = cellFinder;
            _damageableTracker = damageableTracker;
        }

        public void Initialize() => _damageableTracker.Destroyed += HandleDestroyed;

        public void Dispose() => _damageableTracker.Destroyed -= HandleDestroyed;

        // ---------- Orders ----------

        /// <summary>Gives the unit a new attack order, replacing its current one.</summary>
        public void Attack(IMovable unit, IDamageable target)
        {
            if (unit is not IAttacker attacker || target == null || target.IsDead) return;

            Cancel(unit);
            _orders.Add(new AttackOrder { Unit = unit, Attacker = attacker, Target = target });
        }

        public void Cancel(IMovable unit) => _orders.RemoveAll(order => order.Unit == unit);

        /// <summary>Ends orders on the dead target and the dead unit's own order.</summary>
        private void HandleDestroyed(IDamageable dead)
            => _orders.RemoveAll(order => order.Target == dead || ReferenceEquals(order.Unit, dead));

        public void Tick()
        {
            // Backwards, so finished orders can be removed
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                if (UpdateOrder(_orders[i]) == OrderStatus.Finished)
                    _orders.RemoveAt(i);
            }

            ApplyPendingHits(); // safe to run the death chain now
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

        /// <summary>Hits once the unit is settled and the attack interval has passed.</summary>
        private void HitWhenReady(AttackOrder order)
        {
            // Never attack mid-step
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

        /// <summary>Walks into attack range. Finishes the order if the target cannot be reached.</summary>
        private OrderStatus ChaseTarget(AttackOrder order)
        {
            bool targetChangedCell = order.Target.MinCell != order.LastTargetCell;
            bool isInRange = IsInRange(order);

            // Nothing changed: keep walking, or wait for a passing target
            if (!targetChangedCell && (_move.IsMoving(order.Unit) || isInRange))
                return OrderStatus.Active;

            order.LastTargetCell = order.Target.MinCell;

            if (TryWalkIntoRange(order))
                return OrderStatus.Active;

            // In range of a walking target: wait for it to stop
            if (isInRange)
                return OrderStatus.Active;

            // Nothing in range is reachable: give up
            _move.Stop(order.Unit);
            return OrderStatus.Finished;
        }

        /// <summary>Walks to the nearest reachable free cell in range.</summary>
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
