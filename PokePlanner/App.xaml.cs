using System.Windows;
using PokeAPI;

namespace PokePlanner
{
    /// <summary>
    /// Interaction logic for App.xaml.
    /// </summary>
    public partial class App
    {
#if DEBUG
        /// <summary>
        /// Set DataFetcher to use local instance of PokeAPI.
        /// </summary>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            DataFetcher.DataBackend = new HttpBackend("http://localhost:8000/api/v2/", "My fork of PokeAPI.NET!");
        }
#endif
    }
}