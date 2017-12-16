#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    
    // Should subclass MonoMac.AppKit.NSView
    [Register("EcsFeaturesConfigurationPage")]
    public partial class EcsFeaturesConfigurationPage
    {
    }
    
    // Should subclass MonoMac.AppKit.NSViewController
    [Register("EcsFeaturesConfigurationPageController")]
    public partial class EcsFeaturesConfigurationPageController
    {
    }
}

