using UnityEngine;
using Rts2D.Building.View;
using Rts2D.Shared.Contracts;

namespace Rts2D.Building.Manager
{
    public class BuildingDespawnHandler : IDespawnHandler
    {
        public bool CanHandle(IDamageable dead)
        {
            return dead is BuildingView;
        }

        public void Despawn(IDamageable dead)
        {
            Object.Destroy(((BuildingView)dead).gameObject);
        }
    }
}