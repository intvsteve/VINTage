// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace INTV.LtoFlash.View
{
	[Register ("MenuLayoutViewController")]
	partial class MenuLayoutViewController
	{
		[Outlet]
		MonoMac.AppKit.NSImageView ConnectionIcon { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField DropFilesHereText { get; set; }

		[Outlet]
		MonoMac.AppKit.NSColorWell ItemColorWell { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView MenuDifferencesIcon { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTreeController MenuLayoutController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSScrollView MenuTreeScrollView { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton NewFolderButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSImageView PowerStateIcon { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton RemoveItemButton { get; set; }

		[Action ("OnDoubleClick:")]
		partial void OnDoubleClick (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (ConnectionIcon != null) {
				ConnectionIcon.Dispose ();
				ConnectionIcon = null;
			}

			if (DropFilesHereText != null) {
				DropFilesHereText.Dispose ();
				DropFilesHereText = null;
			}

			if (ItemColorWell != null) {
				ItemColorWell.Dispose ();
				ItemColorWell = null;
			}

			if (MenuLayoutController != null) {
				MenuLayoutController.Dispose ();
				MenuLayoutController = null;
			}

			if (MenuTreeScrollView != null) {
				MenuTreeScrollView.Dispose ();
				MenuTreeScrollView = null;
			}

			if (NewFolderButton != null) {
				NewFolderButton.Dispose ();
				NewFolderButton = null;
			}

			if (PowerStateIcon != null) {
				PowerStateIcon.Dispose ();
				PowerStateIcon = null;
			}

			if (MenuDifferencesIcon != null) {
				MenuDifferencesIcon.Dispose ();
				MenuDifferencesIcon = null;
			}

			if (RemoveItemButton != null) {
				RemoveItemButton.Dispose ();
				RemoveItemButton = null;
			}
		}
	}

	[Register ("MenuLayoutView")]
	partial class MenuLayoutView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
