using System;
using Rts2D.Building.SO;

namespace Rts2D.ProductionMenu.Manager
{
    public class ProductSelectionService
    {
        public event Action<BuildingSO> ProductSelected;

        public void Select(BuildingSO data) => ProductSelected?.Invoke(data);
    }
}
