using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Unit.Soldier.SO
{
    [CreateAssetMenu(menuName = "Rts2D/SoldierCatalogSO", fileName = "SoldierCatalogSO")]
    public class SoldierCatalogSO : ScriptableObject
    {
        [SerializeField] private SoldierSO[] soldiers;
        [SerializeField, Min(0)] private int prewarmCountPerType = 10;

        public IReadOnlyList<SoldierSO> Soldiers => soldiers;
        public int PrewarmCountPerType => prewarmCountPerType;
    }
}
