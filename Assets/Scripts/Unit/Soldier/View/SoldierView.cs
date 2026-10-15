using System;
using System.Collections.Generic;
using UnityEngine;
using Rts2D.Health.Model;
using Rts2D.Health.View;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;

namespace Rts2D.Unit.Soldier.View
{
    public class SoldierView : MonoBehaviour, IMovable, IDamageable, IAttacker
    {
        public event Action<IDamageable> Died;

        [SerializeField] private HealthBarView healthBar;

        private static readonly IReadOnlyList<SoldierDataSo> NoProducibles = Array.Empty<SoldierDataSo>();
        private readonly HealthModel _health = new HealthModel();

        public SoldierDataSo Data { get; private set; }
        public Vector2Int Cell { get; private set; }
        public string DisplayName => Data.DisplayName;
        public Sprite Icon => Data.Icon;
        public IReadOnlyList<SoldierDataSo> Producibles => NoProducibles;
        public Queue<Vector2Int> Path { get; } = new Queue<Vector2Int>();
        public Vector2Int Destination { get; set; }
        public GameObject GameObject => gameObject;
        public float MoveSpeed => Data.MoveSpeed;
        public bool IsDead => _health.IsDead;
        public int Damage => Data.Damage;
        public float AttackInterval => Data.AttackInterval;

        public void Initialize(SoldierDataSo data, Vector2Int cell, Vector3 worldPosition)
        {
            Data = data;
            _health.Reset(data.MaxHealth);
            Cell = cell;
            Path.Clear();
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

        public void SetCell(Vector2Int cell) => Cell = cell;
    }
}
