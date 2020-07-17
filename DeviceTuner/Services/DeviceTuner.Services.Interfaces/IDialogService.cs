using System;
using System.Collections.Generic;
using System.Text;

namespace DeviceTuner.Services.Interfaces
{
    public interface IDialogService
    {

        /// <summary>
        /// Full names of the selected files
        /// </summary>
        string FullFileNames { get; }

        
        bool OpenFileDialog();  // открытие файла
        bool SaveFileDialog();  // сохранение файла
    }
}
