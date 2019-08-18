using System.Runtime.Serialization;

namespace INTV.Shared.Properties
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    [DataContract(Namespace = "https://www.intvfunhouse.com")]
    public abstract partial class SettingsBase<T>
    {
        /// <summary>
        /// Initialize settings from file for GTK implementation.
        /// </summary>
        /// <param name="instance">The settings instance to initialize.</param>
        static partial void LateInitialize(T instance)
        {
            InitializeFromSettingsFile<T>(instance);
        }
    }
}
