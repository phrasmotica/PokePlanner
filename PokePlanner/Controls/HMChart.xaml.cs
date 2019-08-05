using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
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
        /// Matrix indicating whether the pokemon in each slot can learn each HM.
        /// FIrst index (col) for a Pokemon, second index (row) for an HM.
        /// </summary>
        public bool[][] CanLearnMatrix;

        /// <summary>
        /// Create a label for each HM move.
        /// </summary>
        public HMChart()
        {
            InitializeComponent();

            for (int i = 0; i < AllLabels.Count; i++)
            {
                var hmLabel = AllLabels[i];
                hmLabel.Prefix = $@"HM{i + 1:D2}: ";
            }

            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                CanLearnMatrix = new bool[Constants.TEAM_SIZE][];
                for (int i = 0; i < CanLearnMatrix.Length; i++)
                {
                    CanLearnMatrix[i] = new bool[Constants.NUMBER_OF_HMS];
                }
            }
        }

        /// <summary>
        /// Returns all HM labels.
        /// </summary>
        private IList<HMLabel> AllLabels => new List<HMLabel>
        {
            hmLabel1, hmLabel2, hmLabel3, hmLabel4, hmLabel5, hmLabel6, hmLabel7, hmLabel8
        };

        /// <summary>
        /// Fetch all HM moves for the version group and display them.
        /// </summary>
        public async void UpdateHMs()
        {
            var versionGroup = SessionCache.Instance.VersionGroup;
            var vgName = await versionGroup.GetName();
            Console.WriteLine($@"Retrieving HM moves for {vgName}...");

            SessionCache.Instance.HMMoves = await versionGroup.GetHMMoves();
            var hmMoves = SessionCache.Instance.HMMoves;

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

            // set columns of learn matrix
            var moveNames = hmMoves.Select(m => m.Name).ToArray();
            var team = TeamManager.Instance.Team;
            for (var col = 0; col < team.Length; col++)
            {
                var canLearn = team[col].Pokemon.CanLearn(moveNames);
                SetCanLearn(col, canLearn);
            }

            // update coverage once, after every matrix column has been updated
            UpdateHMCoverage(hmCount);
        }

        /// <summary>
        /// Returns whether the team can learn the given HM.
        /// </summary>
        public bool GetCanLearn(int hmRowIndex)
        {
            return CanLearnMatrix.Any(pokemonCol => pokemonCol[hmRowIndex]);
        }

        /// <summary>
        /// Sets the given column in the learn matrix and updates the HM labels accordingly.
        /// </summary>
        public void SetCanLearn(int pokemonColIndex, bool[] canLearn)
        {
            CanLearnMatrix[pokemonColIndex] = canLearn;
        }

        /// <summary>
        /// Returns the names of all team members.
        /// </summary>
        private static async Task<string[]> GetTeamNames()
        {
            var tasks = TeamManager.Instance.Team.Select(async tm => await tm.GetName()).ToArray();
            return await Task.WhenAll(tasks);
        }

        /// <summary>
        /// Returns the names of team members that can learn the given HM.
        /// </summary>
        private async Task<IEnumerable<string>> GetLearners(int hmRowIndex)
        {
            var ret = new List<string>();
            var teamNames = await GetTeamNames();
            for (int pokemonColIndex = 0; pokemonColIndex < Constants.TEAM_SIZE; pokemonColIndex++)
            {
                if (CanLearnMatrix[pokemonColIndex][hmRowIndex])
                {
                    ret.Add(teamNames[pokemonColIndex]);
                }
            }

            return ret;
        }

        /// <summary>
        /// Update chart according to the HMs that can be learnt by the team.
        /// </summary>
        public async void UpdateHMCoverage(int hmCount)
        {
            Console.WriteLine(@"Updating HM move coverage...");

            // activate/deactivate labels accordingly
            for (var hmRowIndex = 0; hmRowIndex < hmCount; hmRowIndex++)
            {
                var hmLabel = AllLabels[hmRowIndex];
                if (GetCanLearn(hmRowIndex))
                {
                    hmLabel.Activate();

                    string toolTip;
                    var learners = (await GetLearners(hmRowIndex)).ToList();
                    var learnersCount = learners.Count;
                    if (learnersCount == 1)
                    {
                        toolTip = $"{learners[0]} can learn {hmLabel.MoveName}";
                    }
                    else if (learnersCount <= 3)
                    {
                        toolTip = string.Join(", ", learners.Take(learnersCount - 1))
                                  + $" and {learners.Last()} can learn {hmLabel.MoveName}";
                    }
                    else
                    {
                        toolTip = string.Join(", ", learners.Take(3))
                                  + $" and {learnersCount - 3} more\ncan learn {hmLabel.MoveName}";
                    }

                    hmLabel.ToolTip = toolTip;
                }
                else
                {
                    hmLabel.Deactivate();
                    hmLabel.ToolTip = $"None of your team can learn {hmLabel.MoveName}!";
                }
            }

            Console.WriteLine(@"Updated HM move coverage.");
        }
    }
}