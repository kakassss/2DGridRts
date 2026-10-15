using Zenject;

public class SelectionInstaller : MonoInstaller
{
   public override void InstallBindings()
   {
      Container.Bind<SelectionService>().AsSingle().NonLazy();
      Container.BindInterfacesAndSelfTo<SelectionManager>().AsSingle().NonLazy();
   }
}
