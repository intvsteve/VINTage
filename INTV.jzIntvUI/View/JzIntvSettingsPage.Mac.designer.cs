// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace INTV.JzIntvUI.View
{
	[Register ("JzIntvSettingsPageController")]
	partial class JzIntvSettingsPageController
	{
		[Outlet]
		MonoMac.AppKit.NSArrayController DisplayResolutionsArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController JzIntvDisplayModesController { get; set; }

		[Action ("ClearConfigurationPath:")]
		partial void ClearConfigurationPath (MonoMac.Foundation.NSObject sender);

		[Action ("CommandLineOptionSelected:")]
		partial void CommandLineOptionSelected (MonoMac.Foundation.NSObject sender);

		[Action ("ResetResolutionToDefault:")]
		partial void ResetResolutionToDefault (MonoMac.Foundation.NSObject sender);

		[Action ("SetConfigurationPath:")]
		partial void SetConfigurationPath (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (DisplayResolutionsArrayController != null) {
				DisplayResolutionsArrayController.Dispose ();
				DisplayResolutionsArrayController = null;
			}

			if (JzIntvDisplayModesController != null) {
				JzIntvDisplayModesController.Dispose ();
				JzIntvDisplayModesController = null;
			}
		}
	}

	[Register ("JzIntvSettingsPage")]
	partial class JzIntvSettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
