// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//
using MonoMac.Foundation;
using System.CodeDom.Compiler;

namespace INTV.LtoFlash.View
{
	[Register ("DeviceInformationController")]
	partial class DeviceInformationController
	{
		[Outlet]
		MonoMac.AppKit.NSButton BackgroundGCCheckBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton CloseButtonControl { get; set; }

		[Outlet]
		MonoMac.AppKit.NSCollectionViewItem CollectionViewItemController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSCollectionView ControllerButtonsGrid { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController ControllerElementsArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField DeviceNameEntry { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextField DeviceOwnerEntry { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton ECSCompatibilityButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton IntellivisionIICompatibilityButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton KeyclicksCheckBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton RememberMenuPositionCheckBox { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton SaveMenuPositionButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton ShowTitleScreenButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSButton UpdateFirmwareButton { get; set; }

		[Action ("OnClose:")]
		partial void OnClose (MonoMac.Foundation.NSObject sender);

		[Action ("OnUpdateFirmware:")]
		partial void OnUpdateFirmware (MonoMac.Foundation.NSObject sender);
		
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
