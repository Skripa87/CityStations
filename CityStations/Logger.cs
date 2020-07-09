using System;

namespace CityStations
{
    public static class Logger
    {
        
        public static string WriteLog(string message, string initiator)
        {
            string result = null;
            try
            {
                var manager = new ContextManager();
                result = manager.CreateEvent(message, initiator);
            }
            catch (Exception)
            {
                // ignored
            }
            return result;
        } 
    }
}