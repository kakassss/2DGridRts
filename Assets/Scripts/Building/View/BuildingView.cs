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

        public BuildingSO Data { get; private set; }

        public event Action<IDamageable> Died;

        public string DisplayName => Data.DisplayName;
        public Sprite Icon => Data.Icon;
        public IReadOnlyList<IProduct> Producibles => Data.Producibles;

        public Vector2Int OriginCell { get; private set; }
        public Vector2Int Size => Data.Size;

        public Vector2Int MinCell => OriginCell;
        public Vector2Int MaxCell => OriginCell + Size - Vector2Int.one;
        public bool CanProduceSoldiers => Data.CanProduceSoldiers;
        public Vector2Int SpawnCell => OriginCell + Data.SpawnOffset;

        public bool IsDead => _health.IsDead;

        /// <summary>Called when the building is taken from the pool; it is a ghost until Initialize runs.</summary>
        public void Prepare(BuildingSO data)
        {
            Data = data;
            _healthBar.gameObject.SetActive(false);

            if (_spawnMarker != null)
                _spawnMarker.gameObject.SetActive(false);
        }

        /// <summary>Called when the ghost is actually placed on the grid.</summary>
        public void Initialize(Vector2Int originCell)
        {
            OriginCell = originCell;
            _health.Reset(Data.MaxHealth);

            _healthBar.Setup(Data.Size.x * 0.8f, Data.Size.y);
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

            Vector2 center = SpawnCell + new Vector2(0.5f, 0.5f);
            _spawnMarker.position = new Vector3(center.x, center.y, _spawnMarker.position.z);
        }

        /// <summary>Tints the sprite (used by the placement preview to show valid / invalid).</summary>
        public void SetTint(Color color)
        {
            _spriteRenderer.color = color;
        }
    }
}