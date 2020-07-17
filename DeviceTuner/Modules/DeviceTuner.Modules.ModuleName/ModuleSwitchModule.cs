using DeviceTuner.Core;
using DeviceTuner.Modules.ModuleName.Views;
using Prism.Ioc;
using Prism.Modularity;
using Prism.Regions;

namespace DeviceTuner.Modules.ModuleName
{
    public class ModuleSwitchModule : IModule
    {
        private readonly IRegionManager _regionManager;

        public ModuleSwitchModule(IRegionManager regionManager)
        {
            _regionManager = regionManager;
        }

        public void OnInitialized(IContainerProvider containerProvider)
        {
            _regionManager.RequestNavigate(RegionNames.ContentRegion, "ViewSwitch");
        }

        public void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterForNavigation<ViewSwitch>();
        }
    }
}