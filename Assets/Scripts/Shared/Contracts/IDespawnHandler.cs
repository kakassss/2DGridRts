
namespace Rts2D.Shared.Contracts
{
    public interface IDespawnHandler
    {
        bool CanHandle(IDamageable dead);
        void Despawn(IDamageable dead);
    }
}