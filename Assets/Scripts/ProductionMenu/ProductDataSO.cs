using UnityEngine;


[CreateAssetMenu(menuName = "ProductDataSO", fileName = "Data/ProductDataSO")]
public class ProductDataSO : ScriptableObject
{
    [SerializeField] private BuildingDataSO[] buildings;
    
    public BuildingDataSO[] Buildings => buildings;
}
