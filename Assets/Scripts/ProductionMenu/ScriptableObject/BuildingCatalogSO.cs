using System.Collections.Generic;
using UnityEngine;
using Rts2D.Building.DataSO;

namespace Rts2D.ProductionMenu.DataSO
{
    [CreateAssetMenu(menuName = "Rts2D/BuildingCatalogSO", fileName = "BuildingCatalogSO")]
    public class BuildingCatalogSO : ScriptableObject
    {
        [SerializeField] private BuildingSO[] buildings;
        [SerializeField, Min(0)] private int prewarmCountPerType = 10;
        public IReadOnlyList<BuildingSO> Buildings => buildings;
        public int PrewarmCountPerType => prewarmCountPerType;
    }
}
