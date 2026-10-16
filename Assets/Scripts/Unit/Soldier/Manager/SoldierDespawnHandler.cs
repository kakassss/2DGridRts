using Rts2D.Move.Manager;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.View;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierDespawnHandler : IDespawnHandler
    {
        private readonly SoldierPoolManager _pools;
        private readonly MoveManager _move;

        public SoldierDespawnHandler(SoldierPoolManager pools, MoveManager move)
        {
            _pools = pools;
            _move = move;
        }

        public bool CanHandle(IDamageable dead)
        { 
            return dead is SoldierView;
        }

        public void Despawn(IDamageable dead)
        {
            var soldier = (SoldierView)dead;
            _move.Stop(soldier);
            _pools.Despawn(soldier);
        }
    }
}
