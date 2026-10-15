using System;
using UnityEngine;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.GameConfig.DataSO;
using Rts2D.Grid.Manager;
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
        private readonly SoldierListSO _soldierList;
        private readonly GameConfigSO _gameConfig;
        private readonly DestroyManager _destroyManager;

        public SoldierSpawnManager(SoldierProductionService service, SoldierPoolManager pools,
            GridManager grid, SoldierListSO soldierList, GameConfigSO gameConfig,
            DestroyManager destroyManager)
        {
            _service = service;
            _pools = pools;
            _grid = grid;
            _soldierList = soldierList;
            _gameConfig = gameConfig;
            _destroyManager = destroyManager;
        }

        public void Initialize()
        {
            _service.ProduceRequested += OnProduceRequested;

            foreach (SoldierDataSo data in _soldierList.Soldiers)
                _pools.Prewarm(data, _soldierList.PrewarmCountPerType);
        }

        public void Dispose()
        {
            _service.ProduceRequested -= OnProduceRequested;   
        }

        private void OnProduceRequested(SoldierDataSo data, ISelectable producer)
        {
            if (producer is not ISoldierProducer building) return;

            if (!TryFindCellAroundBuilding(building, out Vector2Int cell))
            {
                Debug.LogWarning("Binanın çevresinde boş hücre yok.");
                return;
            }

            SoldierView soldier = _pools.Spawn(data);
            soldier.Initialize(data, cell, CellCenter(cell));
            _grid.SetOccupant(cell.x, cell.y, soldier); // hücreyi işgal et
            _destroyManager.Track(soldier);             // ölümü dinlemeye başla
        }

        /// <summary>Binanın hemen dışındaki hücre halkasını (1 hücre kalınlık) tarar, sonra genişletir.</summary>
        private bool TryFindCellAroundBuilding(ISoldierProducer b, out Vector2Int result)
        {
            const int MaxDistance = 5;

            int minX = b.OriginCell.x;
            int minY = b.OriginCell.y;
            int maxX = minX + b.Size.x - 1;
            int maxY = minY + b.Size.y - 1;

            for (int d = 1; d <= MaxDistance; d++)
            {
                for (int x = minX - d; x <= maxX + d; x++)
                {
                    for (int y = minY - d; y <= maxY + d; y++)
                    {
                        bool onRing = x == minX - d || x == maxX + d || y == minY - d || y == maxY + d;
                        if (!onRing) continue;

                        if (_grid.InBounds(x, y) && !_grid.GetPositionIsOccupied(x, y))
                        {
                            result = new Vector2Int(x, y);
                            return true;
                        }
                    }
                }
            }

            result = default;
            return false;
        }

        private Vector3 CellCenter(Vector2Int cell)
        {
            float s = _gameConfig.CellSize;
            return new Vector3((cell.x + 0.5f) * s, (cell.y + 0.5f) * s, 0f);
        }
    }
}
