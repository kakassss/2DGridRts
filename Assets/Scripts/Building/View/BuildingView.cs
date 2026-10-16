using System;
using System.Collections.Generic;
using UnityEngine;
using Rts2D.Building.DataSO;
using Rts2D.Health.Model;
using Rts2D.Health.View;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Building.View
{
    public class BuildingView : MonoBehaviour, ISoldierProducer, IDamageable
    {
        public event Action<IDamageable> Died;

        [SerializeField] private HealthBarView healthBar;
        [SerializeField] private BuildingDataSO buildingDataSO;

        private readonly HealthModel _health = new HealthModel();

        public string DisplayName => buildingDataSO.DisplayName;
        public Sprite Icon => buildingDataSO.Icon;
        public IReadOnlyList<IProduct> Producibles => buildingDataSO.Producibles;
        public Vector2Int OriginCell { get; private set; }
        public Vector2Int Size => buildingDataSO.Size;
        public bool IsDead => _health.IsDead;
        public Vector2Int MinCell => OriginCell;
        public Vector2Int MaxCell => OriginCell + Size - Vector2Int.one;


        public void Initialize(BuildingDataSO data, Vector2Int originCell)
        {
            buildingDataSO = data;
            OriginCell = originCell;
            _health.Reset(data.MaxHealth);

            healthBar.Setup(data.Size.x * 0.8f, data.Size.y);
            healthBar.SetHealth(_health.Current, _health.Max);
        }

        public void TakeDamage(int amount)
        {
            bool died = _health.Take(amount);
            healthBar.SetHealth(_health.Current, _health.Max);

            if (died)
                Died?.Invoke(this);
        }
    }
}
