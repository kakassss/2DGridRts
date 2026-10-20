using System;
using UnityEngine;
using Rts2D.Health.Model;
using Rts2D.Health.View;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.SO;

namespace Rts2D.Unit.Soldier.View
{
    public class SoldierView : MonoBehaviour, IMovable, IDamageable, IAttacker
    {
        public event Action<IDamageable> Died;

        [SerializeField] private HealthBarView healthBar;

        private readonly HealthModel _health = new HealthModel();

        public SoldierSO Data { get; private set; }
        public Vector2Int Cell { get; private set; }
        public string DisplayName => Data.DisplayName;
        public Sprite Icon => Data.Icon;
        public GameObject GameObject => gameObject;
        public float MoveSpeed => Data.MoveSpeed;
        public bool IsDead => _health.IsDead;
        public int Damage => Data.Damage;
        public float AttackInterval => Data.AttackInterval;
        public Vector2Int MinCell => Cell;
        public Vector2Int MaxCell => Cell;

        public void Initialize(SoldierSO data, Vector2Int cell, Vector3 worldPosition)
        {
            Data = data;
            _health.Reset(data.MaxHealth);
            Cell = cell;
            transform.position = worldPosition;

            healthBar.Setup(0.8f, 1f);
            healthBar.SetHealth(_health.Current, _health.Max);
        }

        public void TakeDamage(int amount)
        {
            bool died = _health.Take(amount);
            healthBar.SetHealth(_health.Current, _health.Max);

            if (died)
                Died?.Invoke(this);
        }

        public void SetCell(Vector2Int cell)
        {
            Cell = cell;
        }
    }
}
