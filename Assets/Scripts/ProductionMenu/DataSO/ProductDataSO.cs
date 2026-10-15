using UnityEngine;
using Rts2D.Building.DataSO;

namespace Rts2D.ProductionMenu.DataSO
{

    [CreateAssetMenu(menuName = "ProductDataSO", fileName = "Data/ProductDataSO")]
    public class ProductDataSO : ScriptableObject
    {
        [SerializeField] private BuildingDataSO[] buildings;

        public BuildingDataSO[] Buildings => buildings;
    }
}
