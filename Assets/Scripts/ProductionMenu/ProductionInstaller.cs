using Zenject;

public class ProductionInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<ProductSelectionService>().AsSingle().NonLazy();
    }
}
