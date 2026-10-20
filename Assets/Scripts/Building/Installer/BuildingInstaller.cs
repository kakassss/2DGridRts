using UnityEngine;
using Zenject;
using Rts2D.Building.Manager;
using Rts2D.Building.DataSO;

namespace Rts2D.Building.Installer
{
    public class BuildingInstaller : MonoInstaller
    {
        [SerializeField] private Transform buildingPoolParent;
        [SerializeField] private BuildingCatalogSO buildingCatalog;

        public override void InstallBindings()
        {
            Container.BindInstance(buildingCatalog); // the production menu uses it too

            Container.BindInterfacesAndSelfTo<BuildingPoolManager>().AsSingle()
                .WithArguments(buildingPoolParent);
            Container.BindInterfacesTo<BuildingDespawnHandler>().AsSingle();
        }
    }
}
