using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Building.DataSO
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
