using System;
using UnityEngine;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.Grid.Util;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.DataSO;
using Rts2D.Unit.Soldier.View;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierSpawnManager : IInitializable, IDisposable
    {
        private readonly SoldierProductionService _service;
        private readonly SoldierPoolManager _pools;
        private readonly GridManager _grid;
        private readonly SurroundingCellFinder _cellFinder;
        private readonly SoldierListSO _soldierList;
        private readonly DestroyManager _destroyManager;

        public SoldierSpawnManager(SoldierProductionService service, SoldierPoolManager pools,
            GridManager grid, SurroundingCellFinder cellFinder,
            SoldierListSO soldierList, DestroyManager destroyManager)
        {
            _service = service;
            _pools = pools;
            _grid = grid;
            _cellFinder = cellFinder;
            _soldierList = soldierList;
            _destroyManager = destroyManager;
        }

        public void Initialize()
        {
            _service.ProduceRequested += OnProduceRequested;

            foreach (SoldierDataSo data in _soldierList.Soldiers)
                _pools.Prewarm(data, _soldierList.PrewarmCountPerType);
        }

        public void Dispose() => _service.ProduceRequested -= OnProduceRequested;

        private void OnProduceRequested(IProduct product, ISelectable producer)
        {
            if (product is not SoldierDataSo data) return;
            if (producer is not ISoldierProducer building) return;

            if (!_cellFinder.TryFind(building, out Vector2Int cell))
            {
                Debug.LogWarning("Binanın çevresinde boş hücre yok.");
                return;
            }

            SoldierView soldier = _pools.Spawn(data);
            soldier.Initialize(data, cell, _grid.GetCellCenter(cell));
            _grid.PlaceOccupant(soldier);
            _destroyManager.Track(soldier);
        }
    }
}