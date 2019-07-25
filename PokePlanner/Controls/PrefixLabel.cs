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
        public string Prefix
        {
            get => prefix;
            set
            {
                var oldContent = Content as string;
                if (!string.IsNullOrEmpty(prefix))
                {
                    oldContent = oldContent?.Replace(prefix, string.Empty);
                }

                prefix = value;
                SetContent(oldContent);
            }
        }
        private string prefix;

        /// <summary>
        /// Method for setting the label text after the prefix.
        /// </summary>
        public void SetContent(string newContent)
        {
            Content = Prefix + newContent;
        }
    }
}