using Zenject;
using Rts2D.Placement.Manager;
using Rts2D.Placement.Service;

namespace Rts2D.Placement.Installer
{
    public class PlacementInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GhostBuildingManager>().AsSingle();
            Container.Bind<SpawnCellResolver>().AsSingle();
            Container.BindInterfacesAndSelfTo<PlacementManager>().AsSingle().NonLazy();
        }
    }
}
