using System.Collections.Generic;
using UnityEngine;

namespace Rts2D.Unit.Soldier.DataSO
{
    [CreateAssetMenu(menuName = "SoldierListSO", fileName = "Data/SoldierListSO")]
    public class SoldierListSO : ScriptableObject
    {
        [SerializeField] private List<SoldierDataSo> _soldiers = new List<SoldierDataSo>();
        [SerializeField, Min(0)] private int _prewarmCountPerType = 10;

        public IReadOnlyList<SoldierDataSo> Soldiers => _soldiers;
        public int PrewarmCountPerType => _prewarmCountPerType;
    }
}
