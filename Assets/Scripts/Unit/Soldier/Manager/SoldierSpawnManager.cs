using System;
using System.Linq;
using UnityEngine;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Utils;
using Rts2D.Shared.Contracts;
using Rts2D.Unit.Soldier.SO;
using Rts2D.Unit.Soldier.View;

namespace Rts2D.Unit.Soldier.Manager
{
    public class SoldierSpawnManager : IInitializable, IDisposable
    {
        private readonly SoldierProductionService _service;
        private readonly SoldierPoolManager _pools;
        private readonly GridManager _grid;
        private readonly FreeCellFinder _cellFinder;
        private readonly IDamageableTracker _tracker;

        public SoldierSpawnManager(SoldierProductionService service, SoldierPoolManager pools,
            GridManager grid, FreeCellFinder cellFinder, IDamageableTracker tracker)
        {
            _service = service;
            _pools = pools;
            _grid = grid;
            _cellFinder = cellFinder;
            _tracker = tracker;
        }

        public void Initialize()
        {
            _service.ProduceRequested += OnProduceRequested;
        }

        public void Dispose()
        {
            _service.ProduceRequested -= OnProduceRequested;
        }

        private void OnProduceRequested(IProduct product, IProducer producer)
        {
            if (product is not SoldierSO data) return;
            if (!producer.Producibles.Contains(product)) return; // not in this producer's list

            if (!_cellFinder.TryFindNear(producer.SpawnCell, out Vector2Int cell)) return;

            SoldierView soldier = _pools.Spawn(data);
            soldier.Initialize(data, cell, _grid.GetCellCenter(cell));
            _grid.PlaceOccupant(soldier);
            _tracker.Track(soldier);
        }
    }
}