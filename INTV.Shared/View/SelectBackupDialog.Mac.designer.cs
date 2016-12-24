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
	[Register ("SelectBackupDialogController")]
	partial class SelectBackupDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSArrayController BackupDirectoriesArrayController { get; set; }

		[Action ("OnCancel:")]
		partial void OnCancel (MonoMac.Foundation.NSObject sender);

		[Action ("OnRestore:")]
		partial void OnRestore (MonoMac.Foundation.NSObject sender);
		
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
