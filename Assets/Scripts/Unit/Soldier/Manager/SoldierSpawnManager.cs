using System;
using UnityEngine;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Grid.Manager;
using Rts2D.Grid.Util;
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
        private readonly SurroundingCellFinder _cellFinder;
        private readonly DestroyManager _destroyManager;

        public SoldierSpawnManager(SoldierProductionService service, SoldierPoolManager pools,
            GridManager grid, SurroundingCellFinder cellFinder, DestroyManager destroyManager)
        {
            _service = service;
            _pools = pools;
            _grid = grid;
            _cellFinder = cellFinder;
            _destroyManager = destroyManager;
        }

        public void Initialize()
        {
            _service.ProduceRequested += OnProduceRequested;
        }

        public void Dispose()
        {
            _service.ProduceRequested -= OnProduceRequested;
        }

        private void OnProduceRequested(IProduct product, ISelectable producer)
        {
            if (product is not SoldierSO data) return;
            if (producer is not ISoldierProducer { CanProduceSoldiers: true } building) return;

            if (!_cellFinder.TryFindNear(building.SpawnCell, out Vector2Int cell))
            {
                Debug.LogWarning("No free cell near the spawn point.");
                return;
            }

            SoldierView soldier = _pools.Spawn(data);
            soldier.Initialize(data, cell, _grid.GetCellCenter(cell));
            _grid.PlaceOccupant(soldier);
            _destroyManager.Track(soldier);
        }
    }
}