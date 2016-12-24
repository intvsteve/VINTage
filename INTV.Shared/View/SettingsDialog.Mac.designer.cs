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
	[Register ("SettingsDialogController")]
	partial class SettingsDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSTabView PropertyPages { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (PropertyPages != null) {
				PropertyPages.Dispose ();
				PropertyPages = null;
			}
		}
	}

	[Register ("SettingsDialog")]
	partial class SettingsDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
