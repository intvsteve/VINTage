// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace Locutus.View
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		NSToolbar Toolbar { get; set; }

		[Outlet]
		NSView OverlayLayer { get; set; }

		[Outlet]
		NSSplitView SplitView { get; set; }

		[Outlet]
		NSView RomListSplitView { get; set; }

		[Outlet]
		NSView MenuLayoutSplitView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Toolbar != null) {
				Toolbar.Dispose ();
				Toolbar = null;
			}

			if (OverlayLayer != null) {
				OverlayLayer.Dispose ();
				OverlayLayer = null;
			}

			if (SplitView != null) {
				SplitView.Dispose ();
				SplitView = null;
			}

			if (RomListSplitView != null) {
				RomListSplitView.Dispose ();
				RomListSplitView = null;
			}

			if (MenuLayoutSplitView != null) {
				MenuLayoutSplitView.Dispose ();
				MenuLayoutSplitView = null;
			}
		}
	}

	[Register ("MainWindow")]
	partial class MainWindow
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
