using Zenject;
using Rts2D.Placement.Manager;

namespace Rts2D.Placement.Installer
{
    public class PlacementInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<PlacementManager>().AsSingle().NonLazy();
        }
    }
}
