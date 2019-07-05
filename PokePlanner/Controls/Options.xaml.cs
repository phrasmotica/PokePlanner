using System.ComponentModel;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for Options.xaml.
    /// </summary>
    public partial class Options
    {
        /// <summary>
        /// Constructor.
        /// </summary>
        public Options()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                grid.Background = MyBrushes.PastelBlue;
            }
        }
    }
}