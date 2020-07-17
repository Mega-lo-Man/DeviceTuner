using DeviceTuner.Services.Interfaces;
using DeviceTuner.SharedDataModel;
using Renci.SshNet;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace DeviceTuner.Services
{
    public class SSH_Sender : ISender
    {
        private SshClient sshClient;

        public bool CloseConnection()
        {
            sshClient.Disconnect();
            return true;
        }

        public bool CreateConnection(string IPaddress, ushort Port, string Username, string Password)
        {
            try
            {
                ConnectionInfo ConnNfo = new ConnectionInfo(IPaddress, Username,
                    new AuthenticationMethod[] {
                                //Password based Authentication
                                new PasswordAuthenticationMethod(Username, Password),
                                //Key Based Authentication (using keys in OpneSSH format)
                                new PrivateKeyAuthenticationMethod(Username, new PrivateKeyFile[]
                                {
                                    new PrivateKeyFile(@"id_rsa.key", "testrsa") //@"..\openssh.key"
                                }),
                    });

                sshClient = new SshClient(ConnNfo);
                sshClient.Connect();
            }
            catch (Exception ex)
            {
                Debug.Print( "Fault SSH connect." + ex.ToString() );
                return false;
            }
            return true;
        }

        public NetworkDevice Send(NetworkDevice networkDevice, Dictionary<string, string> SettingsDict)
        {

            return networkDevice;
        }
    }
}
