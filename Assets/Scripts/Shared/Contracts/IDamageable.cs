using System;

namespace Rts2D.Shared.Contracts
{
    public interface IDamageable : IGridOccupant
    {
        bool IsDead { get; }
        event Action<IDamageable> Died;
        void TakeDamage(int amount);
    }
}
