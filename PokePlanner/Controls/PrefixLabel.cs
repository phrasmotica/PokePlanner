using System.Windows.Controls;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Label with a string prefix independent from its content.
    /// </summary>
    public class PrefixLabel : Label
    {
        /// <summary>
        /// The prefix for the label.
        /// </summary>
        public string Prefix;

        /// <summary>
        /// Constructor.
        /// </summary>
        public PrefixLabel(string prefix)
        {
            Prefix = prefix;
            Content = Prefix;
        }

        /// <summary>
        /// Method for setting the label text after the prefix.
        /// </summary>
        public void SetContent(string newContent)
        {
            Content = Prefix + newContent;
        }
    }
}