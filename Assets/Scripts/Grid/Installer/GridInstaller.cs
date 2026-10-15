using Zenject;

public class GridInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<GridManager>().AsSingle().NonLazy();
        Container.Bind<MouseInput>().AsSingle().NonLazy();
    }
}
