using UnityEngine;
using Zenject;
using Rts2D.Unit.Soldier.SO;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Unit.Soldier.Installer
{
    public class SoldierInstaller : MonoInstaller
    {
        [SerializeField] private Transform soldierPoolParent;
        [SerializeField] private SoldierCatalogSO soldierCatalog;

        public override void InstallBindings()
        {
            Container.Bind<SoldierProductionService>().AsSingle();

            Container.BindInterfacesAndSelfTo<SoldierPoolManager>().AsSingle()
                .WithArguments(soldierPoolParent, soldierCatalog);
            Container.BindInterfacesAndSelfTo<SoldierSpawnManager>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SoldierDespawnHandler>().AsSingle();
        }
    }
}
