// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using System.CodeDom.Compiler;

namespace INTV.LtoFlash.View
{
	[Register ("MenuLayoutViewController")]
	partial class MenuLayoutViewController
	{
		[Outlet]
		NSImageView ConnectionIcon { get; set; }

		[Outlet]
		NSTextField DropFilesHereText { get; set; }

		[Outlet]
		NSColorWell ItemColorWell { get; set; }

		[Outlet]
		NSImageView MenuDifferencesIcon { get; set; }

		[Outlet]
		NSTreeController MenuLayoutController { get; set; }

		[Outlet]
		NSScrollView MenuTreeScrollView { get; set; }

		[Outlet]
		NSButton NewFolderButton { get; set; }

		[Outlet]
		NSImageView PowerStateIcon { get; set; }

		[Outlet]
		NSButton RemoveItemButton { get; set; }

		[Action ("OnDoubleClick:")]
		partial void OnDoubleClick (NSObject sender);
		
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
