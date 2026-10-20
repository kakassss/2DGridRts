using System.Collections.Generic;
using UnityEngine;
using Rts2D.Building.View;
using Rts2D.Shared.Contracts;
using Rts2D.Shared.Data;

namespace Rts2D.Building.DataSO
{
    [CreateAssetMenu(menuName = "Rts2D/BuildingSO", fileName = "BuildingSO")]
    public class BuildingSO : ProductSO
    {
        [SerializeField] private BuildingView prefab;
        [SerializeField] private Vector2Int size = new Vector2Int(2, 2);
        [SerializeField, Min(1)] private int maxHealth = 50;

        [Header("Production")]
        [Tooltip("Leave empty for buildings that produce nothing.")]
        [SerializeField] private ProductSO[] producibleSoldiers;
        [SerializeField] private Vector2Int spawnOffset = new Vector2Int(0, -1);

        public IReadOnlyList<IProduct> Producibles => producibleSoldiers;
        public Vector2Int Size => size;
        public Vector2Int SpawnOffset => spawnOffset;
        public int MaxHealth => maxHealth;
        public BuildingView Prefab => prefab;
    }
}
