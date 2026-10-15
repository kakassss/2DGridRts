using System.Collections.Generic;
using UnityEngine;
using Zenject;

public class AttackManager : ITickable
{
    private class AttackOrder
    {
        public IMovable Unit;
        public IAttacker Attacker;
        public IDamageable Target;
        public float NextAttackTime;
        public Vector2Int LastTargetCell;
    }

    private readonly MoveManager _move;
    private readonly GridManager _grid;
    private readonly List<AttackOrder> _orders = new List<AttackOrder>();

    public AttackManager(MoveManager move, GridManager grid)
    {
        _move = move;
        _grid = grid;
    }

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

    public void Tick()
    {
        for (int i = _orders.Count - 1; i >= 0; i--)
        {
            AttackOrder o = _orders[i];

            if (o.Target.IsDead || !o.Unit.GameObject.activeSelf)
            {
                _orders.RemoveAt(i);
                continue;
            }

            if (IsInRange(o.Unit.Cell, o.Target, o.Attacker.AttackRange))
            {
                _move.Stop(o.Unit);

                if (Time.time >= o.NextAttackTime)
                {
                    o.Target.TakeDamage(o.Attacker.Damage);
                    o.NextAttackTime = Time.time + o.Attacker.AttackInterval;
                }
                continue;
            }

            // Menzil dışı: yol bittiyse ya da hedef yer değiştirdiyse yeniden yaklaş
            GetBounds(o.Target, out Vector2Int targetMin, out _);
            bool targetMoved = targetMin != o.LastTargetCell;

            if (o.Unit.Path.Count == 0 || targetMoved)
            {
                o.LastTargetCell = targetMin;

                if (TryFindCellInRange(o.Unit.Cell, o.Target, o.Attacker.AttackRange, out Vector2Int cell))
                    _move.MoveTo(o.Unit, cell);
                else if (o.Unit.Path.Count == 0)
                    _orders.RemoveAt(i); // ulaşılamaz
            }
        }
    }

    private static void GetBounds(IDamageable t, out Vector2Int min, out Vector2Int max)
    {
        switch (t)
        {
            case BuildingView b:
                min = b.OriginCell;
                max = b.OriginCell + b.Size - Vector2Int.one;
                break;
            case IMovable m:
                min = max = m.Cell;
                break;
            default:
                min = max = Vector2Int.zero;
                break;
        }
    }

    private static int DistanceTo(Vector2Int cell, IDamageable target)
    {
        GetBounds(target, out Vector2Int min, out Vector2Int max);
        int dx = Mathf.Max(min.x - cell.x, 0, cell.x - max.x);
        int dy = Mathf.Max(min.y - cell.y, 0, cell.y - max.y);
        return dx + dy;
    }

    private static bool IsInRange(Vector2Int cell, IDamageable target, int range)
        => DistanceTo(cell, target) <= range;

    private bool TryFindCellInRange(Vector2Int from, IDamageable target, int range, out Vector2Int result)
    {
        GetBounds(target, out Vector2Int min, out Vector2Int max);

        int best = int.MaxValue;
        result = default;
        bool found = false;

        for (int x = min.x - range; x <= max.x + range; x++)
        {
            for (int y = min.y - range; y <= max.y + range; y++)
            {
                if (!_grid.InBounds(x, y) || _grid.GetPositionIsOccupied(x, y)) continue;
                if (!IsInRange(new Vector2Int(x, y), target, range)) continue;

                int dist = Mathf.Abs(x - from.x) + Mathf.Abs(y - from.y);
                if (dist < best) { best = dist; result = new Vector2Int(x, y); found = true; }
            }
        }
        return found;
    }
}