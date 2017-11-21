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
	[Register ("GeneralFeaturesConfigurationPageController")]
	partial class GeneralFeaturesConfigurationPageController
	{
		[Outlet]
		NSPopUpButton IntellivoicePopUpButton { get; set; }

		[Outlet]
		NSPopUpButton NtscPopUpButton { get; set; }

		[Outlet]
		NSPopUpButton PalPopUpButton { get; set; }
		
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
