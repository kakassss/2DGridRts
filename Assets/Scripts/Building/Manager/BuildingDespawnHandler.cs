using Rts2D.Building.View;
using Rts2D.Shared.Contracts;

namespace Rts2D.Building.Manager
{
    public class BuildingDespawnHandler : IDespawnHandler
    {
        private readonly BuildingPoolManager _poolManager;

        public BuildingDespawnHandler(BuildingPoolManager poolManager)
        {
            _poolManager = poolManager;
        }

        public bool CanHandle(IDamageable dead)
        {
            return dead is BuildingView;
        }

        public void Despawn(IDamageable dead)
        {
            _poolManager.Despawn((BuildingView)dead);
        }
    }
}
