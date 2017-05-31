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

namespace INTV.LtoFlash.View
{
	[Register ("DeviceInformationController")]
	partial class DeviceInformationController
	{
		[Outlet]
		NSButton BackgroundGCCheckBox { get; set; }

		[Outlet]
		NSButton CloseButtonControl { get; set; }

		[Outlet]
		NSCollectionViewItem CollectionViewItemController { get; set; }

		[Outlet]
		NSCollectionView ControllerButtonsGrid { get; set; }

		[Outlet]
		NSArrayController ControllerElementsArrayController { get; set; }

		[Outlet]
		NSTextField DeviceNameEntry { get; set; }

		[Outlet]
		NSTextField DeviceOwnerEntry { get; set; }

		[Outlet]
		NSPopUpButton ECSCompatibilityButton { get; set; }

		[Outlet]
		NSPopUpButton IntellivisionIICompatibilityButton { get; set; }

		[Outlet]
		NSButton KeyclicksCheckBox { get; set; }

		[Outlet]
		NSButton RememberMenuPositionCheckBox { get; set; }

		[Outlet]
		NSPopUpButton SaveMenuPositionButton { get; set; }

		[Outlet]
		NSPopUpButton ShowTitleScreenButton { get; set; }

		[Outlet]
		NSButton UpdateFirmwareButton { get; set; }

		[Action ("OnClose:")]
		partial void OnClose (NSObject sender);

		[Action ("OnUpdateFirmware:")]
		partial void OnUpdateFirmware (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CloseButtonControl != null) {
				CloseButtonControl.Dispose ();
				CloseButtonControl = null;
			}

			if (BackgroundGCCheckBox != null) {
				BackgroundGCCheckBox.Dispose ();
				BackgroundGCCheckBox = null;
			}

			if (CollectionViewItemController != null) {
				CollectionViewItemController.Dispose ();
				CollectionViewItemController = null;
			}

			if (ControllerButtonsGrid != null) {
				ControllerButtonsGrid.Dispose ();
				ControllerButtonsGrid = null;
			}

			if (ControllerElementsArrayController != null) {
				ControllerElementsArrayController.Dispose ();
				ControllerElementsArrayController = null;
			}

			if (DeviceNameEntry != null) {
				DeviceNameEntry.Dispose ();
				DeviceNameEntry = null;
			}

			if (DeviceOwnerEntry != null) {
				DeviceOwnerEntry.Dispose ();
				DeviceOwnerEntry = null;
			}

			if (ECSCompatibilityButton != null) {
				ECSCompatibilityButton.Dispose ();
				ECSCompatibilityButton = null;
			}

			if (IntellivisionIICompatibilityButton != null) {
				IntellivisionIICompatibilityButton.Dispose ();
				IntellivisionIICompatibilityButton = null;
			}

			if (KeyclicksCheckBox != null) {
				KeyclicksCheckBox.Dispose ();
				KeyclicksCheckBox = null;
			}

			if (RememberMenuPositionCheckBox != null) {
				RememberMenuPositionCheckBox.Dispose ();
				RememberMenuPositionCheckBox = null;
			}

			if (SaveMenuPositionButton != null) {
				SaveMenuPositionButton.Dispose ();
				SaveMenuPositionButton = null;
			}

			if (ShowTitleScreenButton != null) {
				ShowTitleScreenButton.Dispose ();
				ShowTitleScreenButton = null;
			}

			if (UpdateFirmwareButton != null) {
				UpdateFirmwareButton.Dispose ();
				UpdateFirmwareButton = null;
			}
		}
	}

	[Register ("DeviceInformation")]
	partial class DeviceInformation
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
