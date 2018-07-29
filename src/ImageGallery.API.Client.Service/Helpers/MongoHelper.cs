using System;
using System.Collections.Concurrent;
using Serilog;
using Serilog.Core;

namespace ImageGallery.API.Client.Service.Helpers
{
    public static class MongoHelper
    {
        private static readonly ConcurrentDictionary<string, Logger> Loggers = new ConcurrentDictionary<string, Logger>();

        public static bool SetupLogger(string dbPath, string collectionName = "log")
        {
            try
            {
                var log = new LoggerConfiguration()
                    .WriteTo.MongoDB(dbPath, collectionName: collectionName)
                    .CreateLogger();
                Loggers.TryAdd(collectionName, log);
            }
            catch (Exception ex)
            {
                //
                return false;
            }

            return true;
        }

        public static Logger GetLogger(string collectionName = "log")
        {
            return !Loggers.TryGetValue(collectionName, out var log) ? null : log;
        }
    }
}
