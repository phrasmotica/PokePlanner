using System;
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
        private readonly Settings settings;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();

            // set settings file
            settings = Settings.Default;
        }
        
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            LoadTeam();
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
            display1.Species = settings.pokemon1;
            display2.Species = settings.pokemon2;
            display3.Species = settings.pokemon3;
            display4.Species = settings.pokemon4;
            display5.Species = settings.pokemon5;
            display6.Species = settings.pokemon6;
        }

        /// <summary>
        /// Save the current team.
        /// </summary>
        private void SaveTeam()
        {
            settings.pokemon1 = display1.Species;
            settings.pokemon2 = display2.Species;
            settings.pokemon3 = display3.Species;
            settings.pokemon4 = display4.Species;
            settings.pokemon5 = display5.Species;
            settings.pokemon6 = display6.Species;
            settings.Save();
        }
    }
}
