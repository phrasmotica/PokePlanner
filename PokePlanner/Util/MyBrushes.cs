using System.Windows.Media;

namespace PokePlanner.Util
{
    /// <summary>
    /// More brushes!
    /// </summary>
    public static class MyBrushes
    {
        /// <summary>
        /// A neutral light grey.
        /// </summary>
        public static Brush LightGrey => Color.FromRgb(219, 219, 219).MakeBrush();

        /// <summary>
        /// A cool pastel blue!
        /// </summary>
        public static Brush PastelBlue => Color.FromRgb(186, 232, 255).MakeBrush();

        /// <summary>
        /// A pastoral green...
        /// </summary>
        public static Brush PastelGreen => Color.FromRgb(199, 255, 199).MakeBrush();

        /// <summary>
        /// A sultry pastel red.
        /// </summary>
        public static Brush PastelRed => Color.FromRgb(255, 161, 161).MakeBrush();
    }
}