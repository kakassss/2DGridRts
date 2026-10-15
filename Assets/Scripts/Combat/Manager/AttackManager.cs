using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Building.View;
using Rts2D.Grid.Manager;
using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Combat.Manager
{
    public class AttackManager : ITickable
    {
        #region Nested Types

        private class AttackOrder
        {
            public IMovable Unit;
            public IAttacker Attacker;
            public IDamageable Target;
            public float NextAttackTime;
            public Vector2Int LastTargetCell;
        }

        #endregion

        #region Fields

        private readonly MoveManager _move;
        private readonly GridManager _grid;
        private readonly List<AttackOrder> _orders = new List<AttackOrder>();

        // Döngü sırasında biriken vuruşlar, döngü bitince uygulanır
        private readonly List<(IDamageable target, int damage)> _pendingHits
            = new List<(IDamageable, int)>();

        #endregion

        #region Lifecycle

        public AttackManager(MoveManager move, GridManager grid)
        {
            _move = move;
            _grid = grid;
        }

        public void Tick()
        {
            // 1) Emirleri işle. Bu döngüde TakeDamage çağrılmaz.
            for (int i = _orders.Count - 1; i >= 0; i--)
            {
                AttackOrder attackOrder = _orders[i];

                if (attackOrder.Target.IsDead || !attackOrder.Unit.GameObject.activeSelf)
                {
                    _orders.RemoveAt(i);
                    continue;
                }

                if (IsAdjacent(attackOrder.Unit.Cell, attackOrder.Target))
                {
                    // Hedef yürüyorsa vurma, takibe devam et (aşağıdaki yaklaşma koduna düş)
                    if (!IsMoving(attackOrder.Target))
                    {
                        _move.Stop(attackOrder.Unit);

                        if (Time.time >= attackOrder.NextAttackTime)
                        {
                            _pendingHits.Add((attackOrder.Target, attackOrder.Attacker.Damage));
                            attackOrder.NextAttackTime = Time.time + attackOrder.Attacker.AttackInterval;
                        }
                        continue;
                    }
                }

                // Bitişik değil: yol bittiyse ya da hedef yer değiştirdiyse yeniden yaklaş
                GetBounds(attackOrder.Target, out Vector2Int targetMin, out _);
                bool targetMoved = targetMin != attackOrder.LastTargetCell;

                if (attackOrder.Unit.Path.Count == 0 || targetMoved)
                {
                    attackOrder.LastTargetCell = targetMin;

                    if (TryFindAdjacentCell(attackOrder.Unit.Cell, attackOrder.Target, out Vector2Int cell))
                        _move.MoveTo(attackOrder.Unit, cell);
                    else if (attackOrder.Unit.Path.Count == 0)
                        _orders.RemoveAt(i); // ulaşılamaz
                }
            }

            // 2) Döngü bitti: hasarları uygula
            for (int i = 0; i < _pendingHits.Count; i++)
            {
                (IDamageable target, int damage) = _pendingHits[i];
                if (!target.IsDead)
                    target.TakeDamage(damage);
            }
            _pendingHits.Clear();
        }

        #endregion

        #region Public API

        public void Attack(IMovable unit, IDamageable target)
        {
            if (unit is not IAttacker attacker || target == null || target.IsDead) return;

            _orders.RemoveAll(o => o.Unit == unit);
            _orders.Add(new AttackOrder
            {
                Unit = unit,
                Attacker = attacker,
                Target = target,
                LastTargetCell = new Vector2Int(int.MinValue, int.MinValue)
            });
        }

        public void Cancel(IMovable unit) => _orders.RemoveAll(o => o.Unit == unit);

        /// <summary>Ölen hedefi kovalayan tüm emirleri siler.</summary>
        public void CancelTargeting(IDamageable dead) => _orders.RemoveAll(o => o.Target == dead);

        #endregion

        #region Internals

        private static void GetBounds(IDamageable t, out Vector2Int min, out Vector2Int max)
        {
            switch (t)
            {
                case BuildingView buildingView:
                    min = buildingView.OriginCell;
                    max = buildingView.OriginCell + buildingView.Size - Vector2Int.one;
                    break;
                case IMovable m:
                    min = max = m.Cell;
                    break;
                default:
                    min = max = Vector2Int.zero;
                    break;
            }
        }

        /// <summary>Hücrenin hedef footprint'ine Manhattan mesafesi (içindeyse 0).</summary>
        private static int DistanceTo(Vector2Int cell, IDamageable target)
        {
            GetBounds(target, out Vector2Int min, out Vector2Int max);
            int dx = Mathf.Max(min.x - cell.x, 0, cell.x - max.x);
            int dy = Mathf.Max(min.y - cell.y, 0, cell.y - max.y);
            return dx + dy;
        }

        /// <summary>Hücre, hedefin footprint'ine 4 yönde bitişik mi?</summary>
        private bool IsAdjacent(Vector2Int cell, IDamageable target)
        {
            return DistanceTo(cell, target) == 1;
        }

        private static bool IsMoving(IDamageable target)
            => target is IMovable m && m.Path.Count > 0;

        /// <summary>Hedefe bitişik, askere en yakın boş hücreyi bulur.</summary>
        private bool TryFindAdjacentCell(Vector2Int from, IDamageable target, out Vector2Int result)
        {
            GetBounds(target, out Vector2Int min, out Vector2Int max);

            int best = int.MaxValue;
            result = default;
            bool found = false;

            for (int x = min.x - 1; x <= max.x + 1; x++)
            {
                for (int y = min.y - 1; y <= max.y + 1; y++)
                {
                    if (!_grid.InBounds(x, y) || _grid.GetPositionIsOccupied(x, y)) continue;
                    if (!IsAdjacent(new Vector2Int(x, y), target)) continue;

                    int dist = Mathf.Abs(x - from.x) + Mathf.Abs(y - from.y);
                    if (dist < best) { best = dist; result = new Vector2Int(x, y); found = true; }
                }
            }
            return found;
        }

        #endregion
    }
}
