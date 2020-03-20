using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace CityStations
{
    public static class Logger
    {
        
        public static string WriteLog(string message, string initiator)
        {
            try
            {
                var manager = new ContextManager();
                manager.CreateEvent(message, initiator);
            }
            catch (Exception e)
            {
                //var path = "Log";
                //var directory = new DirectoryInfo(path);
                //if (!directory.Exists)
                //{
                //    directory.Create();
                //}
                var text = message + '\n' +
                           $"Инициатор ошибки - пользователь или остановка с идентификатором {initiator}, ошибка {e.Message}, подробности {e.InnerException}, стек вызова {e.StackTrace}";
                //using (FileStream fstream = new FileStream($"D:\\log.txt", FileMode.OpenOrCreate))
                //{
                //    byte[] array = System.Text
                //                         .Encoding
                //                         .Default
                //                         .GetBytes(text);
                //    fstream.Write(array,0,array.Length);
                //}
            }
            return message;
        } 
    }
}