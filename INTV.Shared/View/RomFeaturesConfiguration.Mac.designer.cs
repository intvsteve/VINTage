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
	[Register ("RomFeaturesConfigurationController")]
	partial class RomFeaturesConfigurationController
	{
		[Outlet]
		NSArrayController FeaturePagesArrayController { get; set; }

		[Outlet]
		NSTabView FeaturePagesTabView { get; set; }

		[Action ("OnCancelChanges:")]
		partial void OnCancelChanges (NSObject sender);

		[Action ("OnRevertChanges:")]
		partial void OnRevertChanges (NSObject sender);

		[Action ("OnRevertToInternalDatabase:")]
		partial void OnRevertToInternalDatabase (NSObject sender);

		[Action ("OnUpdateFeatures:")]
		partial void OnUpdateFeatures (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (FeaturePagesTabView != null) {
				FeaturePagesTabView.Dispose ();
				FeaturePagesTabView = null;
			}

			if (FeaturePagesArrayController != null) {
				FeaturePagesArrayController.Dispose ();
				FeaturePagesArrayController = null;
			}
		}
	}

	[Register ("RomFeaturesConfiguration")]
	partial class RomFeaturesConfiguration
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
