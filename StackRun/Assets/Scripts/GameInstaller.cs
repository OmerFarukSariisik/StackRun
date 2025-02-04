using Data;
using Zenject;

public class GameInstaller : MonoInstaller
{
    public override void InstallBindings()
    {
        Container.Bind<LevelProgressSaver>().AsSingle().NonLazy();
        Container.Bind<LevelDataLoader>().AsSingle().NonLazy();
    }
}
