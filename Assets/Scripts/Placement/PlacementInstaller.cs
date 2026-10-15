using Zenject;

public class PlacementInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.BindInterfacesAndSelfTo<PlacementManager>().AsSingle().NonLazy();
    }
}
