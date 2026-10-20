using System.Collections.Generic;
using Rts2D.Grid.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Shared.Selection;

namespace Rts2D.Combat.Manager
{
    public class DestroyManager : IDamageableTracker
    {
        private readonly GridManager _grid;
        private readonly AttackManager _attack;
        private readonly SelectionService _selection;
        private readonly List<IDespawnHandler> _despawners;

        public DestroyManager(GridManager grid, AttackManager attack,
            SelectionService selection, List<IDespawnHandler> despawners)
        {
            _grid = grid;
            _attack = attack;
            _selection = selection;
            _despawners = despawners;
        }

        public void Track(IDamageable damageable)
        {
            damageable.Died += OnDied;
        }

        private void OnDied(IDamageable dead)
        {
            dead.Died -= OnDied;

            _attack.CancelTargeting(dead);
            if (dead is IMovable movable) _attack.Cancel(movable);

            if (_selection.Current == dead)
                _selection.Clear();

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