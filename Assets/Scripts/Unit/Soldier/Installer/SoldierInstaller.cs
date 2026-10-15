using UnityEngine;
using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Move.Manager;
using Rts2D.Unit.Soldier.DataSO;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Unit.Soldier.Installer
{
    public class SoldierInstaller : MonoInstaller
    {
        [SerializeField] private Transform _soldierContainer;
        [SerializeField] private SoldierListSO _soldierList;

        public override void InstallBindings()
        {
            Container.BindInstance(_soldierList);

            Container.Bind<SoldierProductionService>().AsSingle();
            Container.Bind<SoldierPoolManager>().AsSingle().WithArguments(_soldierContainer);
            Container.BindInterfacesAndSelfTo<SoldierSpawnManager>().AsSingle().NonLazy();

            Container.Bind<GridPathfinderManager>().AsSingle();
            Container.BindInterfacesAndSelfTo<MoveManager>().AsSingle().NonLazy();
        }
    }
}
