using System;
using Rts2D.Shared.Contracts;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierProductionService
    {
        public event Action<IProduct, ISelectable> ProduceRequested;

        public void Request(IProduct product, ISelectable producer)
        {
            ProduceRequested?.Invoke(product, producer);
        }
    }
}
