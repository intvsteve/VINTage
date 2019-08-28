using System.Runtime.Serialization;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [DataContract]
    public abstract partial class SettingsBase<T>
    {
        protected SettingsBase()
        {
            FinishInitialization();
        }

        /// <summary>
        /// Initialize settings from file for GTK implementation.
        /// </summary>
        protected abstract void InitializeFromSettingsFile();

        private void FinishInitialization()
        {
            InitializeFromSettingsFile();
        }
    }
}
