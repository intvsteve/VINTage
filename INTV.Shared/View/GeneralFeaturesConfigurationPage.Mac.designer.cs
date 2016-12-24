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
	[Register ("GeneralFeaturesConfigurationPageController")]
	partial class GeneralFeaturesConfigurationPageController
	{
		[Outlet]
		MonoMac.AppKit.NSPopUpButton IntellivoicePopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton NtscPopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton PalPopUpButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (IntellivoicePopUpButton != null) {
				IntellivoicePopUpButton.Dispose ();
				IntellivoicePopUpButton = null;
			}

			if (NtscPopUpButton != null) {
				NtscPopUpButton.Dispose ();
				NtscPopUpButton = null;
			}

			if (PalPopUpButton != null) {
				PalPopUpButton.Dispose ();
				PalPopUpButton = null;
			}
		}
	}

	[Register ("GeneralFeaturesConfigurationPage")]
	partial class GeneralFeaturesConfigurationPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
