using System;
using System.Net.NetworkInformation;
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

            // ping the API
            PingReply response = null;
            try
            {
                response = new Ping().Send(baseUrl + "pokemon/");
            }
            catch (PingException ex)
            {
                Console.Error.WriteLine(ex);
            }

            var status = response?.Status ?? IPStatus.DestinationUnreachable;
            if (status != IPStatus.Success)
            {
                var code = (int) status;
                Console.WriteLine($@"PokeAPI is not running: ping returned error {code} ({status})!");
                Shutdown(code);
            }
        }
    }
}