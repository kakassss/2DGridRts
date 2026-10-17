using Rts2D.Building.Manager;
using Zenject;
using Rts2D.Combat.Manager;
using Rts2D.Unit.Soldier.Manager;

namespace Rts2D.Combat.Installer
{
    public class CombatInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<DestroyManager>().AsSingle().NonLazy();
            Container.BindInterfacesTo<SoldierDespawnHandler>().AsSingle().NonLazy();
            Container.BindInterfacesTo<BuildingDespawnHandler>().AsSingle().NonLazy();
            Container.BindInterfacesAndSelfTo<AttackManager>().AsSingle().NonLazy();
        }
    }
}
