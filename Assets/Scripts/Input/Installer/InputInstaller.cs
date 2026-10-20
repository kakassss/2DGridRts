using Rts2D.InputHandling.Service;
using Zenject;

namespace Rts2D.InputHandling
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
