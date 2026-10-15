using Zenject;
using Rts2D.Grid.Manager;
using Rts2D.Test;

namespace Rts2D.Grid.Installer
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GridManager>().AsSingle().NonLazy();
            Container.Bind<MouseInput>().AsSingle().NonLazy();
        }
    }
}
