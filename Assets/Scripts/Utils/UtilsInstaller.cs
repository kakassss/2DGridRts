using Zenject;

namespace Rts2D.Utils
{
    public class UtilsInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<ApproachCellFinderService>().AsSingle().NonLazy();
            Container.Bind<SurroundingCellFinder>().AsSingle().NonLazy();
        }
    }
}