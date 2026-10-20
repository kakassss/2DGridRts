namespace Rts2D.Shared.Contracts
{
    /// <summary>Watches damageables so their death is handled (orders, selection, grid, pooling).</summary>
    public interface IDamageableTracker
    {
        void Track(IDamageable damageable);
    }
}
