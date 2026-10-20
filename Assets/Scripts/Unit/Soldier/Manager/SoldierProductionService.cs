using System;
using Rts2D.Shared.Contracts;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierProductionService
    {
        public event Action<IProduct, IProducer> ProduceRequested;

        public void Request(IProduct product, IProducer producer)
        {
            ProduceRequested?.Invoke(product, producer);
        }
    }
}
