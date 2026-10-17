using System.Collections.Generic;
using UnityEngine;
using Rts2D.Building.View;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;

namespace Rts2D.Building.DataSO
{
    [CreateAssetMenu(menuName = "BuildingData", fileName = "Data/BuildingData")]
    public class BuildingDataSO : ScriptableObject, IProduct
    {
        private static readonly IReadOnlyList<IProduct> NoProducibles = new List<IProduct>();

        [SerializeField] private BuildingView buildingPrefab;
        [SerializeField] private Vector2Int size = new Vector2Int(2, 2);
        [SerializeField, Min(1)] private int maxHealth = 50;
        [SerializeField] private string displayName = "Building";
        [SerializeField] private Sprite icon;
        [SerializeField] private List<SoldierDataSo> producibleSoldiers;

        [Header("Production")]
        [SerializeField] private bool canProduceSoldiers;
        [SerializeField] private Vector2Int spawnOffset = new Vector2Int(0, -1);

        public IReadOnlyList<IProduct> Producibles => canProduceSoldiers ? producibleSoldiers : NoProducibles;
        public bool CanProduceSoldiers => canProduceSoldiers;
        public string DisplayName => displayName;
        public Sprite Icon => icon;
        public Vector2Int Size => size;
        public Vector2Int SpawnOffset => spawnOffset;
        public int MaxHealth => maxHealth;
        public BuildingView BuildingPrefab => buildingPrefab;
    }
}
