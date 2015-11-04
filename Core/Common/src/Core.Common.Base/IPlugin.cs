namespace Core.Common.Base
{
    public interface IPlugin
    {
        /// <summary>
        /// Activates the plugin.
        /// </summary>
        void Activate();

        /// <summary>
        /// Deactivates the plugin.
        /// </summary>
        void Deactivate();
    }
}