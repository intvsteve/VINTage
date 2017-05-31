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

namespace INTV.JzIntvUI.View
{
	[Register ("JzIntvSettingsPageController")]
	partial class JzIntvSettingsPageController
	{
		[Outlet]
		NSArrayController DisplayResolutionsArrayController { get; set; }

		[Outlet]
		NSArrayController JzIntvDisplayModesController { get; set; }

		[Action ("ClearConfigurationPath:")]
		partial void ClearConfigurationPath (NSObject sender);

		[Action ("CommandLineOptionSelected:")]
		partial void CommandLineOptionSelected (NSObject sender);

		[Action ("ResetResolutionToDefault:")]
		partial void ResetResolutionToDefault (NSObject sender);

		[Action ("SetConfigurationPath:")]
		partial void SetConfigurationPath (NSObject sender);
		
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
