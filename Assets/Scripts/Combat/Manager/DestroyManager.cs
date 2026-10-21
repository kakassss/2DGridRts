using System;
using System.Collections.Generic;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;

namespace Rts2D.Combat.Manager
{
    public class DestroyManager : IDamageableTracker
    {
        public event Action<IDamageable> Destroyed;

        private readonly GridManager _grid;
        private readonly List<IDespawnHandler> _despawners;

        public DestroyManager(GridManager grid, List<IDespawnHandler> despawners)
        {
            _grid = grid;
            _despawners = despawners;
        }

        public void Track(IDamageable damageable)
        {
            damageable.Died += OnDied;
        }

        private void OnDied(IDamageable dead)
        {
            dead.Died -= OnDied;

            // Listeners react before it leaves the grid
            Destroyed?.Invoke(dead);

            _grid.RemoveOccupant(dead);

            foreach (IDespawnHandler despawner in _despawners)
            {
                if (!despawner.CanHandle(dead)) continue;
                despawner.Despawn(dead);
                return;
            }
        }
    }
}