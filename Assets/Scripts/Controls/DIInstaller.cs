using Zenject;

namespace Morkwa.Test.Data
{
    public class DIInstaller : MonoInstaller
    {
        public override void InstallBindings()
        {
            Container.Bind<SettingGame>().AsSingle();
        }
    }
}