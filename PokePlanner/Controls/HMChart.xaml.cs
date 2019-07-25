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
        /// Returns all HM labels.
        /// </summary>
        private IList<HMLabel> AllLabels => new List<HMLabel>
        {
            hmLabel1, hmLabel2, hmLabel3, hmLabel4, hmLabel5, hmLabel6, hmLabel7, hmLabel8
        };

        /// <summary>
        /// Create a label for each HM move.
        /// </summary>
        public HMChart()
        {
            InitializeComponent();

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                for (int i = 0; i < AllLabels.Count; i++)
                {
                    var label = AllLabels[i];
                    label.Prefix = $@"HM{i + 1:D2}: ";
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
            for (var i = 0; i < AllLabels.Count; i++)
            {
                var label = AllLabels[i];
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