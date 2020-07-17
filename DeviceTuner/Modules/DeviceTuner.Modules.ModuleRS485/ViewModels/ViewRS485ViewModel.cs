using DeviceTuner.Core.Mvvm;
using DeviceTuner.Services.Interfaces;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Regions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DeviceTuner.Modules.ModuleRS485.ViewModels
{
    public class ViewRS485ViewModel : RegionViewModelBase
    {
        private string _message;
        public string Message
        {
            get { return _message; }
            set { SetProperty(ref _message, value); }
        }

        public ViewRS485ViewModel(IRegionManager regionManager, IMessageService messageService, IDataRepositoryService dataRepositoryService) :
            base(regionManager)
        {
            Title = "RS485";
            Message = "View RS485 from your Prism Module";
        }
    }
}
