using System;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierProductionService
    {
        public event Action<SoldierDataSo, ISelectable> ProduceRequested;

        public void Request(SoldierDataSo soldier, ISelectable producer)
            => ProduceRequested?.Invoke(soldier, producer);
    }
}
