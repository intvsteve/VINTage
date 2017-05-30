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
	[Register ("KeyboardComponentFeaturesConfigurationPageController")]
	partial class KeyboardComponentFeaturesConfigurationPageController
	{
		[Outlet]
		NSPopUpButton CassetteRequirementPopUpButton { get; set; }

		[Outlet]
		NSPopUpButton KeyboardComponentCompatibilityPopUpButton { get; set; }

		[Outlet]
		NSPopUpButton MicrosoftBasicCartridgeRequirementPopUpButton { get; set; }

		[Outlet]
		NSTextField MicrosoftBasicUsageLabel { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (CassetteRequirementPopUpButton != null) {
				CassetteRequirementPopUpButton.Dispose ();
				CassetteRequirementPopUpButton = null;
			}

			if (KeyboardComponentCompatibilityPopUpButton != null) {
				KeyboardComponentCompatibilityPopUpButton.Dispose ();
				KeyboardComponentCompatibilityPopUpButton = null;
			}

			if (MicrosoftBasicCartridgeRequirementPopUpButton != null) {
				MicrosoftBasicCartridgeRequirementPopUpButton.Dispose ();
				MicrosoftBasicCartridgeRequirementPopUpButton = null;
			}

			if (MicrosoftBasicUsageLabel != null) {
				MicrosoftBasicUsageLabel.Dispose ();
				MicrosoftBasicUsageLabel = null;
			}
		}
	}

	[Register ("KeyboardComponentFeaturesConfigurationPage")]
	partial class KeyboardComponentFeaturesConfigurationPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
