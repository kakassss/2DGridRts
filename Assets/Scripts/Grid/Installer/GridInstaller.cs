using Zenject;
using Rts2D.Grid.Manager;

namespace Rts2D.Grid.Installer
{
    public class GridInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<GridManager>().AsSingle().NonLazy();
        }
    }
}
