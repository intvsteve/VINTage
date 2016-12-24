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
	[Register ("RomListSettingsPageController")]
	partial class RomListSettingsPageController
	{
		[Outlet]
		MonoMac.AppKit.NSArrayController SearchDirectoriesArrayController { get; set; }

		[Action ("RemoveSearchDirectory:")]
		partial void RemoveSearchDirectory (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SearchDirectoriesArrayController != null) {
				SearchDirectoriesArrayController.Dispose ();
				SearchDirectoriesArrayController = null;
			}
		}
	}

	[Register ("RomListSettingsPage")]
	partial class RomListSettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
