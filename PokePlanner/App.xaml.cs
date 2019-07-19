using System;
using System.Net.Sockets;
using System.Windows;
using PokeAPI;

namespace PokePlanner
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// Timeout period for test connecting to PokeAPI in milliseconds.
        /// </summary>
        private const int HealthCheckTimeoutMillis = 1000;

        /// <summary>
        /// Set DataFetcher to use local instance of PokeAPI.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            var baseUrl = "https://pokeapi.co/api/v2/";

#if DEBUG
            baseUrl = "http://localhost:8000/api/v2/";
            DataFetcher.DataBackend = new HttpBackend(baseUrl, "My fork of PokeAPI.NET!");
#endif

            // try to connect to the API
            bool success;
            var uri = new Uri(baseUrl);
            using (var tcp = new TcpClient())
            {
                var result = tcp.BeginConnect(uri.Host, uri.Port, null, null);
                success = result.AsyncWaitHandle.WaitOne(HealthCheckTimeoutMillis);
            }

            if (success)
            {
                Console.WriteLine($@"Connected to PokeAPI at {uri.AbsoluteUri}.");
            }
            else
            {
                var msg = $@"PokeAPI is not running at {uri.AbsoluteUri}!";
                Console.WriteLine(msg);
                MessageBox.Show(msg, "PokePlanner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown(1);
            }
        }
    }
}