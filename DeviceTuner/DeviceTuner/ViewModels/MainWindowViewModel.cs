using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace DeviceTuner.ViewModels
{
    public class MainWindowViewModel : BindableBase
    {
        private string _title = "Prism Application";
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }

        private IDialogService _dialogService;
        private IExcelDataDecoder _excelDataDecoder;
        private IDataRepositoryService _dataRepositoryService;

        public DelegateCommand OpenFileCommand { get; private set; }
        public DelegateCommand SaveFileCommand { get; private set; }
        public DelegateCommand CloseAppCommand { get; private set; }

        public MainWindowViewModel(IDialogService dialogService, IExcelDataDecoder excelDataDecoder, IDataRepositoryService dataRepositoryService)
        {
            _dialogService = dialogService;
            _excelDataDecoder = excelDataDecoder;
            _dataRepositoryService = dataRepositoryService;
            OpenFileCommand = new DelegateCommand(OpenFileExecute, OpenFileCanExecute);
            SaveFileCommand = new DelegateCommand(SaveFileExecute, SaveFileCanExecute);
            CloseAppCommand = new DelegateCommand(CloseAppExecute, CloseAppCanExecute);


        }

        private bool CloseAppCanExecute()
        {
            return true;
        }

        private void CloseAppExecute()
        {
            throw new NotImplementedException();
        }

        private bool SaveFileCanExecute()
        {
            return true;
        }

        private void SaveFileExecute()
        {
            _dialogService.SaveFileDialog();
        }

        private bool OpenFileCanExecute()
        {
            return true;
        }

        private void OpenFileExecute()
        {
            if(_dialogService.OpenFileDialog())
            {
                string selectedFile = _dialogService.FullFileNames;
                
                _dataRepositoryService.SetDevices(_excelDataDecoder.GetNetworkDevices(selectedFile));

                
            }
        }
    }
}
