using Zenject;
using Rts2D.Grid.Manager;

namespace Rts2D.Grid.Installer
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<GridManager>().AsSingle().NonLazy();
            Container.Bind<GridPathfinderManager>().AsSingle();
        }
    }
}
