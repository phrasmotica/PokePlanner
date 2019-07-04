using System;
using System.Windows;
using PokePlanner.Properties;

namespace PokePlanner
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml.
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// The settings file.
        /// </summary>
        private Settings settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            Loaded += OnLoaded;
        }

        protected void OnLoaded(object sender, RoutedEventArgs routedEventArgs)
        {
            // set settings file
            settings = Settings.Default;

            LoadTeam();

            // set minimum window width
            MinWidth = Width;
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);

            SaveTeam();
        }

        /// <summary>
        /// Load the saved team.
        /// </summary>
        private void LoadTeam()
        {
            Console.WriteLine(@"Loading saved team...");

            display1.Species = settings.pokemon1;
            display2.Species = settings.pokemon2;
            display3.Species = settings.pokemon3;
            display4.Species = settings.pokemon4;
            display5.Species = settings.pokemon5;
            display6.Species = settings.pokemon6;

            Console.WriteLine(@"Saved team loaded.");
        }

        /// <summary>
        /// Save the current team.
        /// </summary>
        private void SaveTeam()
        {
            Console.WriteLine(@"Saving current team...");

            settings.pokemon1 = display1.Species;
            settings.pokemon2 = display2.Species;
            settings.pokemon3 = display3.Species;
            settings.pokemon4 = display4.Species;
            settings.pokemon5 = display5.Species;
            settings.pokemon6 = display6.Species;
            settings.Save();

            Console.WriteLine(@"Current team saved.");
        }
    }
}