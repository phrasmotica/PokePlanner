using System.Windows.Media;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for HMLabel.xaml.
    /// </summary>
    public class HMLabel : PrefixLabel, ISwitchable
    {
        /// <summary>
        /// Whether this label is active.
        /// </summary>
        private bool active = true;

        /// <summary>
        /// Constructor.
        /// </summary>
        public HMLabel()
        {
            Prefix = "HM00: ";
            Foreground = Brushes.Black;
            Background = MyBrushes.PastelGreen;
        }

        /// <summary>
        /// Display the label as active.
        /// </summary>
        public void Activate()
        {
            if (!active)
            {
                active = true;
                Opacity = 1;
            }
        }

        /// <summary>
        /// Display the label as inactive.
        /// </summary>
        public void Deactivate()
        {
            if (active)
            {
                active = false;
                Opacity = 0.5;
            }
        }
    }
}