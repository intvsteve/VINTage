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
	[Register ("SelectBackupDialogController")]
	partial class SelectBackupDialogController
	{
		[Outlet]
		NSArrayController BackupDirectoriesArrayController { get; set; }

		[Action ("OnCancel:")]
		partial void OnCancel (NSObject sender);

		[Action ("OnRestore:")]
		partial void OnRestore (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (BackupDirectoriesArrayController != null) {
				BackupDirectoriesArrayController.Dispose ();
				BackupDirectoriesArrayController = null;
			}
		}
	}

	[Register ("SelectBackupDialog")]
	partial class SelectBackupDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
