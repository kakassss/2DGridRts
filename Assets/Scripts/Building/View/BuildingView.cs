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
        [SerializeField] private Transform _spawnMarker;

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
        public bool CanProduceSoldiers => buildingDataSO.CanProduceSoldiers;
        public Vector2Int SpawnCell => OriginCell + buildingDataSO.SpawnOffset;

        public bool IsDead => _health.IsDead;

        public void Initialize(BuildingDataSO data, Vector2Int originCell)
        {
            buildingDataSO = data;
            OriginCell = originCell;
            _health.Reset(data.MaxHealth);

            _healthBar.Setup(data.Size.x * 0.8f, data.Size.y);
            _healthBar.SetHealth(_health.Current, _health.Max);
            PlaceSpawnMarker();
        }

        public void TakeDamage(int amount)
        {
            bool died = _health.Take(amount);
            _healthBar.SetHealth(_health.Current, _health.Max);

            if (died)
                Died?.Invoke(this);
        }

        /// <summary>Moves the optional marker onto the spawn cell (cell size is 1 world unit).</summary>
        private void PlaceSpawnMarker()
        {
            if (_spawnMarker == null) return;

            _spawnMarker.gameObject.SetActive(CanProduceSoldiers);
            if (!CanProduceSoldiers) return;

            Vector2 center = (Vector2)SpawnCell + new Vector2(0.5f, 0.5f);
            _spawnMarker.position = new Vector3(center.x, center.y, _spawnMarker.position.z);
        }

        /// <summary>Görsele renk uygular (yerleştirme önizlemesinde geçerli/geçersiz göstermek için).</summary>
        public void SetTint(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}