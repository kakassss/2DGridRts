using Zenject;
using Rts2D.ProductionMenu.Manager;

namespace Rts2D.ProductionMenu.Installer
{
    public class ProductionInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ProductSelectionService>().AsSingle().NonLazy();
        }
    }
}
