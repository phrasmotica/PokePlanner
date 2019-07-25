namespace PokePlanner.Controls
{
    /// <summary>
    /// Interface for controls that can be switched between active and inactive states.
    /// </summary>
    public interface ISwitchable
    {
        /// <summary>
        /// Activate the control.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivate the control.
        /// </summary>
        void Deactivate();
    }
}