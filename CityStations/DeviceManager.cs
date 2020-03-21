using System;
using System.Collections.Generic;
using System.Text;
using CityStations.Models;
using Renci.SshNet;
using Renci.SshNet.Common;

namespace CityStations
{
    public class DeviceManager
    {
        private readonly string _userName;
        private readonly string _password;

        public void RebootStations()
        {
            var manager = new ContextManager();
            var stations = manager.GetActivStation();
            if (_password != null && _userName != null && stations != null)
            {
                foreach (var station in stations)
                {
                    if (string.IsNullOrEmpty(station.IpDevice)) continue;
                    using (var sshClient = new SshClient(station.IpDevice, _userName, _password))
                    {
                        try
                        {
                            sshClient.Connect();
                        }
                        catch(Exception ex)
                        {
                            Logger.WriteLog(
                                $"Произошла ошибка при попытки соеденения {ex.Message}, подробности {ex.StackTrace}",
                                "RebootDevices");
                        }
                        var command = sshClient.CreateCommand("sudo reboot now\n", Encoding.UTF8);
                        try
                        {
                            command.Execute();
                        }
                        catch (SshConnectionException ex)
                        {
                            Logger.WriteLog($"Выполнена перезагрузка устройства {station.IpDevice}", "ConfigurateDevice");
                            return;
                        }
                    }
                }
            }
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
                if (!ssh.IsConnected) 
                     ssh.Connect();
                command = ssh.CreateCommand("sudo reboot now\n", Encoding.UTF8);
                try
                {
                    command.Execute();
                }
                catch (SshConnectionException ex)
                {
                    Logger.WriteLog($"Выполнена перезагрузка устройства {ipAddressDevice}", "ConfigurateDevice");
                    return;
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