#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    
    // Should subclass MonoMac.AppKit.NSView
    [Register("SystemCompatibilityConfigurationPage")]
    public partial class SystemCompatibilityConfigurationPage
    {
    }
    
    // Should subclass MonoMac.AppKit.NSViewController
    [Register("SystemCompatibilityConfigurationPageController")]
    public partial class SystemCompatibilityConfigurationPageController
    {
    }
}

