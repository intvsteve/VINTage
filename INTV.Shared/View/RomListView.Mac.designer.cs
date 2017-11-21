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

namespace INTV.Shared.View
{
	[Register ("RomListViewController")]
	partial class RomListViewController
	{
		[Outlet]
		NSTextField DropFilesHint { get; set; }

		[Outlet]
		NSTextField ItemsCount { get; set; }

		[Outlet]
		NSArrayController RomsArrayController { get; set; }

		[Outlet]
		NSTextField SelectedItemsCount { get; set; }

		[Action ("OnDoubleClick:")]
		partial void OnDoubleClick (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DropFilesHint != null) {
				DropFilesHint.Dispose ();
				DropFilesHint = null;
			}

			if (ItemsCount != null) {
				ItemsCount.Dispose ();
				ItemsCount = null;
			}

			if (RomsArrayController != null) {
				RomsArrayController.Dispose ();
				RomsArrayController = null;
			}

			if (SelectedItemsCount != null) {
				SelectedItemsCount.Dispose ();
				SelectedItemsCount = null;
			}
		}
	}

	[Register ("RomListView")]
	partial class RomListView
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
