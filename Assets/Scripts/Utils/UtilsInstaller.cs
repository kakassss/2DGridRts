using Zenject;

namespace Rts2D.Utils
{
    public class UtilsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ApproachCellFinder>().AsSingle().NonLazy();
            Container.Bind<FreeCellFinder>().AsSingle().NonLazy();
        }
    }
}