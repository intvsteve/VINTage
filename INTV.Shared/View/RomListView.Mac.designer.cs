// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace INTV.Shared.View
{
	[Register ("RomListViewController")]
	partial class RomListViewController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField DropFilesHint { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField ItemsCount { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController RomsArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField SelectedItemsCount { get; set; }

		[Action ("OnDoubleClick:")]
		partial void OnDoubleClick (MonoMac.Foundation.NSObject sender);
		
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
