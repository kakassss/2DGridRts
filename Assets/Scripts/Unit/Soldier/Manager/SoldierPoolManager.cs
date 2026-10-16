using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Unit.Soldier.DataSO;
using Rts2D.Unit.Soldier.View;
using Rts2D.ObjectPool;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierPoolManager
    {
        private readonly IInstantiator _instantiator;
        private readonly Transform _container;
        private readonly GridManager _grid;
        private readonly Dictionary<SoldierDataSo, ObjectPool<SoldierView>> _pools
            = new Dictionary<SoldierDataSo, ObjectPool<SoldierView>>();

        public SoldierPoolManager(IInstantiator instantiator, Transform container, GridManager grid)
        {
            _instantiator = instantiator;
            _container = container;
            _grid = grid;
        }

        public void Prewarm(SoldierDataSo data, int count)
        {
            if (_pools.ContainsKey(data)) return;
            _pools[data] = new ObjectPool<SoldierView>(_instantiator, data.Prefab, _container, count);
        }

        public SoldierView Spawn(SoldierDataSo data)
        {
            if (!_pools.TryGetValue(data, out var pool))
            {
                pool = new ObjectPool<SoldierView>(_instantiator, data.Prefab, _container);
                _pools[data] = pool;
            }
            return pool.Spawn();
        }

        
        public void Despawn(SoldierView soldier)
        {
            _grid.RemoveOccupant(soldier);

            if (_pools.TryGetValue(soldier.Data, out var pool))
                pool.Despawn(soldier);
        }
    }
}