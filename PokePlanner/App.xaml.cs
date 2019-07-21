using System;
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
        protected override async void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // try to connect to the API
            var baseUrl = "https://pokeapi.co/api/v2/";

#if DEBUG
            baseUrl = "http://localhost:8000/api/v2/";
#endif

            DataFetcher.DataBackend = new HttpBackend(baseUrl, "PokePlanner");
            var success = await DataFetcher.GetApiObject<PokemonType>(1) != null;

            if (success)
            {
                Console.WriteLine($@"Connected to PokeAPI at {baseUrl}.");
            }
            else
            {
                var msg = $@"PokeAPI is not running at {baseUrl}!";
                Console.WriteLine(msg);
                MessageBox.Show(msg, "PokePlanner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown(1);
            }
        }
    }
}