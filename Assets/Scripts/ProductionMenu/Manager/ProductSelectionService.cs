using System;
using Rts2D.Building.DataSO;

namespace Rts2D.ProductionMenu.Manager
{
    public class ProductSelectionService
    {
        public event Action<BuildingDataSO> ProductSelected;

        public void Select(BuildingDataSO data) => ProductSelected?.Invoke(data);
    }
}
