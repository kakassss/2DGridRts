using Zenject;
using Rts2D.Selection.Manager;

namespace Rts2D.Selection.Installer
{
    public class SelectionInstaller : MonoInstaller
    {
       public override void InstallBindings()
       {
          Container.Bind<SelectionService>().AsSingle().NonLazy();
          Container.BindInterfacesAndSelfTo<SelectionManager>().AsSingle().NonLazy();
       }
    }
}
