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
	[Register ("RomFeaturesConfigurationController")]
	partial class RomFeaturesConfigurationController
	{
		[Outlet]
		MonoMac.AppKit.NSArrayController FeaturePagesArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTabView FeaturePagesTabView { get; set; }

		[Action ("OnCancelChanges:")]
		partial void OnCancelChanges (MonoMac.Foundation.NSObject sender);

		[Action ("OnRevertChanges:")]
		partial void OnRevertChanges (MonoMac.Foundation.NSObject sender);

		[Action ("OnRevertToInternalDatabase:")]
		partial void OnRevertToInternalDatabase (MonoMac.Foundation.NSObject sender);

		[Action ("OnUpdateFeatures:")]
		partial void OnUpdateFeatures (MonoMac.Foundation.NSObject sender);
		
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
