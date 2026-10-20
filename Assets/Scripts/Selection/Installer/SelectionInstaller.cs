using Zenject;
using Rts2D.Selection.Manager;
using Rts2D.Shared.Selection;

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
