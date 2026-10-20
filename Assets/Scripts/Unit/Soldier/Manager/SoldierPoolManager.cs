using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Pooling.Manager;
using Rts2D.Unit.Soldier.SO;
using Rts2D.Unit.Soldier.View;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierPoolManager : IInitializable
    {
        private readonly IInstantiator _instantiator;
        private readonly Transform _poolParent;
        private readonly SoldierCatalogSO _catalog;
        private readonly Dictionary<SoldierSO, ObjectPool<SoldierView>> _pools
            = new Dictionary<SoldierSO, ObjectPool<SoldierView>>();

        public SoldierPoolManager(IInstantiator instantiator, Transform poolParent, SoldierCatalogSO catalog)
        {
            _instantiator = instantiator;
            _poolParent = poolParent;
            _catalog = catalog;
        }

        public void Initialize()
        {
            foreach (SoldierSO data in _catalog.Soldiers)
                Prewarm(data, _catalog.PrewarmCountPerType);
        }

        public SoldierView Spawn(SoldierSO data)
        {
            if (!_pools.TryGetValue(data, out ObjectPool<SoldierView> pool))
                pool = CreatePool(data, 0);

            return pool.Spawn();
        }

        public void Despawn(SoldierView soldier)
        {
            if (_pools.TryGetValue(soldier.Data, out ObjectPool<SoldierView> pool))
                pool.Despawn(soldier);
        }

        private void Prewarm(SoldierSO data, int count)
        {
            if (!_pools.ContainsKey(data))
                CreatePool(data, count);
        }

        private ObjectPool<SoldierView> CreatePool(SoldierSO data, int prewarm)
        {
            var pool = new ObjectPool<SoldierView>(_instantiator, data.Prefab, _poolParent, prewarm);
            _pools[data] = pool;
            return pool;
        }
    }
}
