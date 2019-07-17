﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Label that can show activated and deactivated states.
    /// </summary>
    public class TwoWayLabel : Label
    {
        /// <summary>
        /// The last-used content.
        /// </summary>
        private object lastContent;

        /// <summary>
        /// The last-used font weight.
        /// </summary>
        private FontWeight lastFontWeight;

        /// <summary>
        /// The last-used background colour.
        /// </summary>
        private Color lastBackground;

        /// <summary>
        /// The last-used tooltip.
        /// </summary>
        private object lastTooltip;

        /// <summary>
        /// Whether this label is active.
        /// </summary>
        private bool active = true;

        /// <summary>
        /// Default constructor.
        /// </summary>
        public TwoWayLabel()
        {
            HorizontalContentAlignment = HorizontalAlignment.Center;
            VerticalContentAlignment = VerticalAlignment.Center;
            FontSize = 16;
            Foreground = Brushes.Black;
            ToolTipService.SetInitialShowDelay(this, 1000);
            this.AddBorder();
        }

        /// <summary>
        /// Display the label as active.
        /// </summary>
        public void Activate()
        {
            if (!active)
            {
                active = true;
                Content = lastContent;
                FontWeight = lastFontWeight;
                Background = new SolidColorBrush(lastBackground);
                ToolTip = lastTooltip;
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

                lastContent = Content;
                Content = "-";

                lastFontWeight = FontWeight;
                FontWeight = FontWeights.Normal;

                lastBackground = ((SolidColorBrush) Background).Color;
                Background = Brushes.White;

                lastTooltip = ToolTip;
                ToolTip = null;
            }
        }
    }
}