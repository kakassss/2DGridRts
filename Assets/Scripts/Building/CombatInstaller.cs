using Zenject;

public class CombatInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<DestroyManager>().AsSingle();
        Container.BindInterfacesAndSelfTo<AttackManager>().AsSingle().NonLazy();
    }
}
