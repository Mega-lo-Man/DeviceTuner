using Prism.Ioc;
using DeviceTuner.Views;
using System.Windows;
using Prism.Modularity;
using DeviceTuner.Modules.ModuleName;
using DeviceTuner.Services.Interfaces;
using DeviceTuner.Services;
using DeviceTuner.Modules.ModuleRS485;
using DeviceTuner.Core;
using DryIoc;
using Prism.DryIoc;
using Renci.SshNet;

namespace DeviceTuner
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {

        enum srvKey { telnetKey, sshKey };

        protected override Window CreateShell()
        {
            return Container.Resolve<MainWindow>();
        }

        //protected override Rules CreateContainerRules() => Rules.Default.WithDefaultIfAlreadyRegistered(IfAlreadyRegistered.AppendNewImplementation);

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            ConnectionInfo ConnNfo = new ConnectionInfo("192.168.1.239", "admin",
                    new AuthenticationMethod[] {
                                //Password based Authentication
                                new PasswordAuthenticationMethod("192.168.1.239", "admin"),
                                //Key Based Authentication (using keys in OpneSSH format)
                                new PrivateKeyAuthenticationMethod("admin", new PrivateKeyFile[]
                                {
                                    new PrivateKeyFile(@"id_rsa.key", "testrsa") //@"..\openssh.key"
                                }),
                    });

            containerRegistry.RegisterSingleton<IMessageService, MessageService>();
            containerRegistry.RegisterSingleton<IDataRepositoryService, DataRepositoryService>();
            containerRegistry.Register<IDialogService, DialogService>();
            containerRegistry.Register<IExcelDataDecoder, ExcelDataDecoder>();
            containerRegistry.GetContainer().RegisterInstance(ConnNfo);
            containerRegistry.GetContainer().Register<ISender, Telnet_Sender>(serviceKey: srvKey.telnetKey);
            containerRegistry.GetContainer().Register<ISender, SSH_Sender>(serviceKey: srvKey.sshKey);
        }

        protected override void ConfigureModuleCatalog(IModuleCatalog moduleCatalog)
        {
            moduleCatalog.AddModule<ModuleSwitchModule>();
            moduleCatalog.AddModule<ModuleRS485Module>();
        }
    }
}
