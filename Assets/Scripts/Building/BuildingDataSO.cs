using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "BuildingData", fileName = "Data/BuildingData")]
public class BuildingDataSO : ScriptableObject, IProduct
{
    [SerializeField] private GameObject buildingPrefab;
    [SerializeField] private Vector2Int size = new Vector2Int(2, 2);
    [SerializeField, Min(1)] private int maxHealth = 50;
    [SerializeField] private string displayName = "Building";
    [SerializeField] private Sprite icon;
    [SerializeField] private List<SoldierDataSo> producibleSoldiers = new List<SoldierDataSo>();
    public IReadOnlyList<SoldierDataSo> ProducibleSoldiers => producibleSoldiers;
    public string DisplayName => displayName;
    public Sprite Icon => icon;
    public Vector2Int Size => size;
    public int MaxHealth => maxHealth;
    public GameObject BuildingPrefab => buildingPrefab;
}