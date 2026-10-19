using Zenject;
using Rts2D.Combat.Manager;

namespace Rts2D.Combat.Installer
{
    public class CombatInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DestroyManager>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AttackManager>().AsSingle().NonLazy();
        }
    }
}
