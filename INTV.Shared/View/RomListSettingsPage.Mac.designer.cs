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
	[Register ("RomListSettingsPageController")]
	partial class RomListSettingsPageController
	{
		[Outlet]
		NSArrayController SearchDirectoriesArrayController { get; set; }

		[Action ("RemoveSearchDirectory:")]
		partial void RemoveSearchDirectory (NSObject sender);
		
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
