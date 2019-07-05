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
        public static Brush LightGrey => MakeBrush(Color.FromRgb(219, 219, 219));

        /// <summary>
        /// A cool pastel blue!
        /// </summary>
        public static Brush PastelBlue => MakeBrush(Color.FromRgb(186, 232, 255));

        /// <summary>
        /// A pastoral green...
        /// </summary>
        public static Brush PastelGreen => MakeBrush(Color.FromRgb(199, 255, 199));

        /// <summary>
        /// A sultry pastel red.
        /// </summary>
        public static Brush PastelRed => MakeBrush(Color.FromRgb(255, 161, 161));

        /// <summary>
        /// Creates a brush from the given colour.
        /// </summary>
        private static Brush MakeBrush(Color c)
        {
            return new SolidColorBrush(c);
        }
    }
}
