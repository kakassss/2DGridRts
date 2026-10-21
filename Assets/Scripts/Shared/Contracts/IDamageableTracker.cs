using System;

namespace Rts2D.Shared.Contracts
{
    /// <summary>Watches damageables and announces their death.</summary>
    public interface IDamageableTracker
    {
        /// <summary>Raised when a tracked one dies, before it leaves the grid.</summary>
        event Action<IDamageable> Destroyed;

        void Track(IDamageable damageable);
    }
}
