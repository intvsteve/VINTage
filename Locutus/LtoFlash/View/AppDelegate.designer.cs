#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace Locutus.View
{
    // Should subclass MonoMac.AppKit.NSResponder
    [Register("AppDelegate")]
	public partial class AppDelegate
    {
    }
}

