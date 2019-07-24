using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Windows;
using PokePlanner.Util;

namespace PokePlanner.Controls
{
    /// <summary>
    /// Interaction logic for HMChart.xaml.
    /// </summary>
    public partial class HMChart
    {
        /// <summary>
        /// The number of HM items across all games.
        /// </summary>
        public const int NUM_HMS = 8;

        /// <summary>
        /// Collection of labels for displaying each HM.
        /// </summary>
        private readonly IList<PrefixLabel> hmLabels;

        /// <summary>
        /// Create a label for each HM move.
        /// </summary>
        public HMChart()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                hmLabels = new List<PrefixLabel>();
                for (int i = 0; i < NUM_HMS; i++)
                {
                    var label = new PrefixLabel($@"HM{i + 1:D2}: ");
                    panel.Children.Add(label);
                    hmLabels.Add(label);
                }
            }
        }

        /// <summary>
        /// Fetch all HM moves for the version group and display them.
        /// </summary>
        public async void UpdateHMs()
        {
            var versionGroup = SessionCache.Instance.VersionGroup;
            var vgName = await versionGroup.GetName();
            Console.WriteLine($@"Retrieving HM moves for {vgName}...");

            var hmMoves = await versionGroup.GetHMMoves();
            var hmCount = hmMoves.Count;
            for (var i = 0; i < NUM_HMS; i++)
            {
                var label = hmLabels[i];
                if (i < hmCount)
                {
                    label.SetContent(hmMoves[i].GetName());
                    label.Visibility = Visibility.Visible;
                }
                else
                {
                    label.SetContent(string.Empty);
                    label.Visibility = Visibility.Hidden;
                }
            }

            Console.WriteLine($@"Retrieved {hmCount} HM moves for {vgName}.");
        }
    }
}