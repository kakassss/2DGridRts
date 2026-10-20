using System;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Watches damageables so their death is handled (orders, selection, grid, pooling).</summary>
    public interface IDamageableTracker
    {
        /// <summary>Raised when a tracked building or unit dies, before it leaves the grid.</summary>
        event Action<IDamageable> Destroyed;

        void Track(IDamageable damageable);
    }
}
