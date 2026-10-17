using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Grid.Util;
using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Combat.Manager
{
    public class AttackManager : ITickable
    {
        private class AttackOrder
        {
            public IMovable Unit;
            public IAttacker Attacker;
            public IDamageable Target;
            public float NextAttackTime;
            public Vector2Int LastTargetCell = new Vector2Int(int.MinValue, int.MinValue);
        }

        private readonly MoveManager _move;
        private readonly ApproachCellFinderService _cellFinder;
        private readonly List<AttackOrder> _orders = new List<AttackOrder>();

        // Hits collected during the loop; applied once the loop has finished
        private readonly List<(IDamageable target, int damage)> _pendingHits
            = new List<(IDamageable, int)>();

        public AttackManager(MoveManager move, ApproachCellFinderService cellFinder)
        {
            _move = move;
            _cellFinder = cellFinder;
        }

        public void Attack(IMovable unit, IDamageable target)
        {
            if (unit is not IAttacker attacker || target == null || target.IsDead) return;

            Cancel(unit);
            _orders.Add(new AttackOrder { Unit = unit, Attacker = attacker, Target = target });
        }

        public void Cancel(IMovable unit) => _orders.RemoveAll(o => o.Unit == unit);

        /// <summary>Removes every order that is chasing the dead target.</summary>
        public void CancelTargeting(IDamageable dead) => _orders.RemoveAll(o => o.Target == dead);

        public void Tick()
        {
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                if (!ProcessOrder(_orders[i]))
                    _orders.RemoveAt(i);
            }

            ApplyPendingHits(); // loop finished, so the death chain can safely run
        }

        /// <summary>Advances the order by one step. Returns false when the order is finished.</summary>
        private bool ProcessOrder(AttackOrder order)
        {
            if (order.Target.IsDead || !order.Unit.GameObject.activeSelf)
                return false;

            bool adjacent = OccupantGeometry.IsAdjacent(order.Unit.Cell, order.Target);

            if (adjacent && !IsMoving(order.Target))
            {
                _move.Stop(order.Unit);
                TryQueueHit(order);
                return true;
            }

            return ChaseTarget(order);
        }

        private void TryQueueHit(AttackOrder order)
        {
            if (Time.time < order.NextAttackTime) return;

            _pendingHits.Add((order.Target, order.Attacker.Damage));
            order.NextAttackTime = Time.time + order.Attacker.AttackInterval;
        }

        /// <summary>Re-approaches when the path ended or the target moved. Returns false if unreachable.</summary>
        private bool ChaseTarget(AttackOrder order)
        {
            bool targetMoved = order.Target.MinCell != order.LastTargetCell;
            if (order.Unit.Path.Count > 0 && !targetMoved) return true;

            order.LastTargetCell = order.Target.MinCell;

            if (_cellFinder.TryFind(order.Unit.Cell, order.Target, out Vector2Int cell))
            {
                _move.MoveTo(order.Unit, cell);
                return true;
            }

            return order.Unit.Path.Count > 0; // keep going if a path exists, otherwise unreachable
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

        private static bool IsMoving(IDamageable target)
            => target is IMovable movable && movable.Path.Count > 0;
    }
}