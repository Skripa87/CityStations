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
            foreach (var station in stations)
            {
                if (string.IsNullOrEmpty(station.InformationTable.IpDevice)) continue;
                RebootStation(station);
            }
        }

        public void ConfigurateAllDevice()
        {
            var manager = new ContextManager();
            var stations = manager.GetActivStations();
            foreach (var station in stations)
            {
                if (string.IsNullOrEmpty(station.InformationTable.IpDevice)) continue;
                ConfigurateDevice(station);
            }
        }

        public void ConfigurateDevice(StationModel station)
        {
            if (station?.InformationTable == null) return;
            UserName = station?.InformationTable?.UserNameDevice;
            Password = station?.InformationTable?.PasswordDevice;
            var ipAddressDevice = station?.InformationTable?.IpDevice;
            var stationId = station.Id;
            station.InformationTable
                   ?.CheckAccessCode();
            var accessCode = station.InformationTable
                                    ?.AccessCode;
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
                    var text = new List<string>();
                    text.Add("[Desktop Entry]");
                    text.Add("Encoding=UTF-8");
                    text.Add("Name=Connect");
                    text.Add("Comment=Checks internet connectivity");
                    text.Add($"Exec=/usr/bin/chromium-browser -incognito --noerrdialogs --kiosk http://92.50.187.210/test/Home/DisplayInformationTable?stationId={stationId}&accessCode={accessCode}");
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
                        catch (SftpPermissionDeniedException sftpError)
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

        public void ConfigurateDevice(string ipAddressDevice, string stationId)
        {
            var manager = new ContextManager();
            var station = manager.GetStation(stationId);
            if (station?.InformationTable == null) return;
            UserName = station?.InformationTable?.UserNameDevice;
            Password = station?.InformationTable?.PasswordDevice;
            if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password)) return;
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
                        catch (SftpPermissionDeniedException sftpError)
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

        public void RebootStation(string stationId)
        {
            var manager = new ContextManager();
            var station = manager.GetStation(stationId);
            if (station?.InformationTable == null) return;
            var ipAddress = station.InformationTable.IpDevice;
            UserName = station.InformationTable.UserNameDevice;
            Password = station.InformationTable.PasswordDevice;
            using (var sshClient = new SshClient(ipAddress, UserName, Password))
            {
                try
                {
                    sshClient.Connect();
                }
                catch (Exception ex)
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
                    Logger.WriteLog($"Выполнена перезагрузка устройства {station.InformationTable.IpDevice}", "ConfigurateDevice");
                    return;
                }
            }
        }

        public void RebootStation(StationModel station)
        {
            if (station?.InformationTable == null) return;
            var ipAddress = station.InformationTable.IpDevice;
            UserName = station.InformationTable.UserNameDevice;
            Password = station.InformationTable.PasswordDevice;
            using (var sshClient = new SshClient(ipAddress, UserName, Password))
            {
                try
                {
                    sshClient.Connect();
                }
                catch (Exception ex)
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
                    Logger.WriteLog($"Выполнена перезагрузка устройства {station.InformationTable.IpDevice}", "ConfigurateDevice");
                    return;
                }
            }
        }
    }
}