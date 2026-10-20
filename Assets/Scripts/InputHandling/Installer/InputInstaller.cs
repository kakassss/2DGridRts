using Rts2D.InputHandling.Manager;
using Zenject;

namespace Rts2D.InputHandling.Installer
{
    public class InputInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<MouseInputService>().AsSingle();
            Container.Bind<InteractionService>().AsSingle();
        }
    }

}
