using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Rts2D.Building.DataSO;
using Rts2D.Building.View;
using Rts2D.Pooling;
using Rts2D.ProductionMenu.DataSO;

namespace Rts2D.Building.Manager
{
    public class BuildingPoolManager : IInitializable
    {
        private readonly IInstantiator _instantiator;
        private readonly Transform _poolParent;
        private readonly BuildingCatalogSO _catalog;
        private readonly Dictionary<BuildingSO, ObjectPool<BuildingView>> _pools
            = new Dictionary<BuildingSO, ObjectPool<BuildingView>>();

        public BuildingPoolManager(IInstantiator instantiator, Transform poolParent, BuildingCatalogSO catalog)
        {
            _instantiator = instantiator;
            _poolParent = poolParent;
            _catalog = catalog;
        }

        public void Initialize()
        {
            foreach (BuildingSO data in _catalog.Buildings)
                Prewarm(data, _catalog.PrewarmCountPerType);
        }

        public BuildingView Spawn(BuildingSO data)
        {
            if (!_pools.TryGetValue(data, out ObjectPool<BuildingView> pool))
                pool = CreatePool(data, 0);

            BuildingView view = pool.Spawn();
            view.Prepare(data); // Data is always set before anyone can despawn it
            return view;
        }

        public void Despawn(BuildingView building)
        {
            if (_pools.TryGetValue(building.Data, out ObjectPool<BuildingView> pool))
                pool.Despawn(building);
        }

        private void Prewarm(BuildingSO data, int count)
        {
            if (!_pools.ContainsKey(data))
                CreatePool(data, count);
        }

        private ObjectPool<BuildingView> CreatePool(BuildingSO data, int prewarm)
        {
            var pool = new ObjectPool<BuildingView>(_instantiator, data.Prefab, _poolParent, prewarm);
            _pools[data] = pool;
            return pool;
        }
    }
}
