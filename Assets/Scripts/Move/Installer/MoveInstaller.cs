using Rts2D.Move.Manager;
using Zenject;

namespace Rts2D.Move.Installer
{
    public class MoveInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<MoveManager>().AsSingle().NonLazy();
        }
    }
}
