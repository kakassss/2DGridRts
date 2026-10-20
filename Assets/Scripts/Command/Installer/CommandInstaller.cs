using Zenject;
using Rts2D.Command.Manager;

namespace Rts2D.Command.Installer
{
    public class CommandInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.BindInterfacesAndSelfTo<CommandManager>().AsSingle().NonLazy();
        }
    }
}
