// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the Xcode designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;

namespace Locutus.View
{
	[Register ("MainWindowController")]
	partial class MainWindowController
	{
		[Outlet]
		MonoMac.AppKit.NSToolbar Toolbar { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView OverlayLayer { get; set; }

		[Outlet]
		MonoMac.AppKit.NSSplitView SplitView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView RomListSplitView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView MenuLayoutSplitView { get; set; }
		
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
