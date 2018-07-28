using System;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace ImageGallery.API.Client.Console.Classes
{
    public static class Metric
    {
        private static readonly ConcurrentDictionary<string, MetricEntity> Entities = new ConcurrentDictionary<string, MetricEntity>();

        public static void Start(string name)
        {
            if (!Entities.ContainsKey(name))
                Entities.TryAdd(name, new MetricEntity());
            Entities[name].Start();            
        }

        public static string Stop(string name)
        {
            if (!Entities.ContainsKey(name)) return null;

            Entities[name].Stop();
            var value = Entities[name].GetElapsed();

            return $"{name}: {value:mm\\:ss\\:fff}";
        }

        public static void StopAll()
        {
            foreach (var value in Entities.Values)
            {
                value.Stop();
            }
        }


        public static void StopAndWriteConsole(string name)
        {
            var old = System.Console.ForegroundColor;
            System.Console.ForegroundColor = ConsoleColor.Yellow;
            System.Console.WriteLine(Stop(name));
            System.Console.ForegroundColor = old;
        }

        private class MetricEntity
        {
            public bool IsRunning => Watch.IsRunning;
            private Stopwatch Watch { get; } = new Stopwatch();

            public void Start()
            {
                Watch.Restart();
            }

            public void Stop()
            {
                Watch.Stop();
            }

            public TimeSpan GetElapsed()
            {
                return Watch.Elapsed;
            }
        }

    }

}
