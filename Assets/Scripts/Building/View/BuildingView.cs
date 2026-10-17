using System;
using System.Collections.Generic;
using Rts2D.Building.DataSO;
using Rts2D.Health.Model;
using Rts2D.Health.View;
using UnityEngine;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Building.View
{
    public class BuildingView : MonoBehaviour, ISoldierProducer, IDamageable
    {
        [SerializeField] private SpriteRenderer _spriteRenderer;
        [SerializeField] private HealthBarView _healthBar;

        private readonly HealthModel _health = new HealthModel();

        public BuildingDataSO buildingDataSO;

        public event Action<IDamageable> Died;

        public string DisplayName => buildingDataSO.DisplayName;
        public Sprite Icon => buildingDataSO.Icon;
        public IReadOnlyList<IProduct> Producibles => buildingDataSO.Producibles;

        public Vector2Int OriginCell { get; private set; }
        public Vector2Int Size => buildingDataSO.Size;

        public Vector2Int MinCell => OriginCell;
        public Vector2Int MaxCell => OriginCell + Size - Vector2Int.one;

        public bool IsDead => _health.IsDead;

        public void Initialize(BuildingDataSO data, Vector2Int originCell)
        {
            buildingDataSO = data;
            OriginCell = originCell;
            _health.Reset(data.MaxHealth);

            _healthBar.Setup(data.Size.x * 0.8f, data.Size.y);
            _healthBar.SetHealth(_health.Current, _health.Max);
        }

        public void TakeDamage(int amount)
        {
            bool died = _health.Take(amount);
            _healthBar.SetHealth(_health.Current, _health.Max);

            if (died)
                Died?.Invoke(this);
        }

        /// <summary>Görsele renk uygular (yerleştirme önizlemesinde geçerli/geçersiz göstermek için).</summary>
        public void SetTint(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}