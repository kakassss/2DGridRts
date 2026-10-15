using System;

public class HealthModel
{
    public int Current { get; private set; }
    public int Max { get; private set; }
    public bool IsDead => Current <= 0;

    public void Reset(int max)
    {
        Max = max;
        Current = max;
    }

    public bool Take(int amount)
    {
        if (IsDead) return false;
        Current = Math.Max(0, Current - amount);
        return IsDead;
    }
}