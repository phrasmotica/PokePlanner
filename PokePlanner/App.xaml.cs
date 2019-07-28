using System;
using System.Windows;
using PokePlanner.Util;

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

            var baseUri = SessionCache.Client.BaseUri.AbsoluteUri;
            var success = await SessionCache.Client.GetResourceAsync<PokeApiNet.Models.Type>(1) != null;
            if (success)
            {
                Console.WriteLine($@"Connected to PokeAPI at {baseUri}.");
            }
            else
            {
                var msg = $@"PokeAPI is not running at {baseUri}!";
                Console.WriteLine(msg);
                MessageBox.Show(msg, "PokePlanner", MessageBoxButton.OK, MessageBoxImage.Exclamation);
                Shutdown(1);
            }
        }
    }
}