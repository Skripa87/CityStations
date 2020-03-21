using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace CityStations
{
    public class DeviceManager
    {
        private readonly string _userName;
        private readonly string _password;

        private bool CheckConfigDevice(string ipAddressDevice)
        {
            if (_password != null && _userName != null && ipAddressDevice != null)
            {
                using (var sshClient = new SshClient(ipAddressDevice, _userName, _password))
                {
                    sshClient.Connect();
                    var command = sshClient.CreateCommand("ls .config/autostart", Encoding.UTF8);
                    command.Execute();
                    var answer = command.Result;
                    return answer.Contains("chromium.desktop\n");
                };
            }
            return false;
        }

        public void ConfigurateDevice(string ipAddressDevice, string stationId)
        {
            using (var ssh = new SshClient(ipAddressDevice, _userName, _password))
            {
                try
                {
                    ssh.Connect();
                }
                catch (SshConnectionException ex)
                {
                    Logger.WriteLog($"Произошла ошибка при попытки соеденения {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                    return;
                }
                var command = ssh.CreateCommand("ls .config/autostart", Encoding.UTF8);
                command.Execute();
                var answer = command.Result;
                if (answer.Contains("chromium.desktop\n"))
                {
                    command = ssh.CreateCommand("rm -f .config/autostart/chromium.desktop\n");
                    command.Execute();
                }
                command = ssh.CreateCommand("nano .config/autostart/chromium.desktop\n");
                command.Execute();
                using (var sftp = new SftpClient(ipAddressDevice, _userName, _password))
                {
                    try
                    {
                        sftp.Connect();
                    }
                    catch (SshConnectionException ex)
                    {
                        Logger.WriteLog($"Произошла ошибка при попытки соеденения {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                        return;
                    }
                    var manager = new ContextManager();
                    var text = new List<string>();
                    text.Add("[Desktop Entry]");
                    text.Add("Encoding=UTF-8");
                    text.Add("Name=Connect");
                    text.Add("Comment=Checks internet connectivity");
                    text.Add($"Exec=/usr/bin/chromium-browser -incognito --noerrdialogs --kiosk http://92.50.187.210/test/Home/DisplayInformationTable?stationId={stationId}&accessCode={manager.SetAccessCode(stationId)}");
                    sftp.AppendAllLines(".config/autostart/chromium.desktop",text);
                }
            }

        }

        public DeviceManager(string userName, string password)
        {
            _userName = userName;
            _password = password;
        }
    }
}