using UnityEngine;
using Zenject;
using Rts2D.Building.Manager;
using Rts2D.Building.SO;

namespace Rts2D.Building.Installer
{
    public class BuildingInstaller : MonoInstaller
    {
        [SerializeField] private Transform buildingPoolParent;
        [SerializeField] private BuildingCatalogSO buildingCatalog;

        public override void InstallBindings()
        {
            Container.BindInstance(buildingCatalog); // also used by the production menu

            Container.BindInterfacesAndSelfTo<BuildingPoolManager>().AsSingle()
                .WithArguments(buildingPoolParent);
            Container.BindInterfacesTo<BuildingDespawnHandler>().AsSingle();
        }
    }
}
