using System;
using System.Collections.Generic;
using Rts2D.Building.SO;
using Rts2D.Health.Model;
using Rts2D.Health.View;
using UnityEngine;
using Rts2D.Shared.Contracts;

namespace Rts2D.Building.View
{
    public class BuildingView : MonoBehaviour, IProducer, IDamageable
    {
        [SerializeField] private SpriteRenderer spriteRenderer;
        [SerializeField] private HealthBarView healthBar;
        [SerializeField] private Transform spawnMarker;

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
        public Vector2Int SpawnCell { get; private set; }

        public bool IsDead => _health.IsDead;

        /// <summary>Called when the building is taken from the pool; it is a ghost until Initialize runs.</summary>
        public void Prepare(BuildingSO data)
        {
            Data = data;
            healthBar.gameObject.SetActive(false);

            if (spawnMarker != null)
                spawnMarker.gameObject.SetActive(false);
        }

        /// <summary>
        /// Called when the ghost is actually placed on the grid. The spawn cell is resolved by the
        /// placement code because only the grid knows its bounds.
        /// </summary>
        public void Initialize(Vector2Int originCell, Vector2Int spawnCell, Vector3 spawnCellCenter)
        {
            OriginCell = originCell;
            SpawnCell = spawnCell;
            _health.Reset(Data.MaxHealth);

            healthBar.Setup(Data.Size);
            healthBar.SetHealth(_health.Current, _health.Max);
            PlaceSpawnMarker(spawnCellCenter);
        }

        public void TakeDamage(int amount)
        {
            bool died = _health.Take(amount);
            healthBar.SetHealth(_health.Current, _health.Max);

            if (died)
                Died?.Invoke(this);
        }

        /// <summary>Moves the optional marker onto the spawn cell.</summary>
        private void PlaceSpawnMarker(Vector3 spawnCellCenter)
        {
            if (spawnMarker == null) return;

            bool producesSomething = Producibles.Count > 0;
            spawnMarker.gameObject.SetActive(producesSomething);
            if (!producesSomething) return;

            spawnMarker.position = new Vector3(spawnCellCenter.x, spawnCellCenter.y, spawnMarker.position.z);
        }

        /// <summary>Tints the sprite (used by the placement preview to show valid / invalid).</summary>
        public void SetTint(Color color)
        {
            spriteRenderer.color = color;
        }
    }
}