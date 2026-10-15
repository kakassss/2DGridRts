using System;

public class ProductSelectionService
{
    public event Action<BuildingDataSO> ProductSelected;

    public void Select(BuildingDataSO data) => ProductSelected?.Invoke(data);
}