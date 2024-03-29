using System;
using System.Threading.Tasks;
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
        /// Startup event handler.
        /// </summary>
        protected async void OnStartup(object sender, StartupEventArgs e)
        {
            await CheckPokeAPIRunning();
        }

        /// <summary>
        /// Verifies that PokeAPI is running, else shut the application down.
        /// </summary>
        private async Task CheckPokeAPIRunning()
        {
            var baseUri = SessionCache.Client.BaseUri.AbsoluteUri;

            var success = await SessionCache.Get<PokeApiNet.Models.Type>(1) != null;
            if (success)
            {
                Console.WriteLine($@"Connected to PokeAPI at {baseUri}.");
                new MainWindow().Show();
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