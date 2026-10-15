using System;

public class SoldierProductionService
{
    public event Action<SoldierDataSo, ISelectable> ProduceRequested;

    public void Request(SoldierDataSo soldier, ISelectable producer)
        => ProduceRequested?.Invoke(soldier, producer);
}