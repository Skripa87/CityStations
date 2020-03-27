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
        public string UserName { get; set; }
        public string Password { get; set; }

        public void RebootStations()
        {
            var manager = new ContextManager();
            var stations = manager.GetActivStations();
            if (Password != null && UserName != null && stations != null)
            {
                foreach (var station in stations)
                {
                    if (string.IsNullOrEmpty(station.IpDevice)) continue;
                    using (var sshClient = new SshClient(station.IpDevice, UserName, Password))
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
            using (var ssh = new SshClient(ipAddressDevice, UserName, Password))
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
                using (var sftp = new SftpClient(ipAddressDevice, UserName, Password))
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
                    try
                    {
                        sftp.AppendAllLines(".config/autostart/chromium.desktop", text);
                    }
                    catch (SftpPermissionDeniedException ex)
                    {
                        Logger.WriteLog($"Произошла ошибка при попытки создания файлов {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                        if (!ssh.IsConnected) ssh.Connect();
                        command = ssh.CreateCommand("sudo rm -f -r .config/autostart\n");
                        try
                        {
                            command.Execute();
                        }
                        catch (SshException sshex)
                        {
                            Logger.WriteLog($"Произошла ошибка при попытки удаления папки {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                            return;
                        }
                        command = ssh.CreateCommand("mkdir .config/autostart\n");
                        try
                        {
                            command.Execute();
                        }
                        catch (SshException sshexep)
                        {
                            Logger.WriteLog($"Произошла ошибка при попытки создания папки {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                            return;
                        }
                        try
                        {
                            sftp.AppendAllLines(".config/autostart/chromium.desktop", text);
                        }
                        catch(SftpPermissionDeniedException sftpError)
                        {
                            Logger.WriteLog($"Произошла ошибка при попытки добавления данных в файл {ex.Message}, подробности {ex.StackTrace}", "ConfigurateDevice");
                            return;
                        }
                    }
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

        public void SetAuthenticationInfo(string password, string userName)
        {
            Password = password;
            UserName = userName;
        }
        
    }
}