using System;

namespace Rts2D.Shared.Contracts
{
    public interface IDamageable : ISelectable,IOccupant
    {
        bool IsDead { get; }
        event Action<IDamageable> Died;
        void TakeDamage(int amount);
    }
}
