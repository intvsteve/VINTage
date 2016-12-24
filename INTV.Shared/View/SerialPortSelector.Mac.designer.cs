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
	[Register ("SerialPortSelectorController")]
	partial class SerialPortSelectorController
	{
		[Outlet]
		MonoMac.AppKit.NSTextField BaudRateLabel { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton BaudRatePopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController BaudRatesArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSArrayController SerialPortsListArrayController { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTableView SerialPortsTableView { get; set; }

		[Action ("SerialPortDoubleClick:")]
		partial void SerialPortDoubleClick (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SerialPortsTableView != null) {
				SerialPortsTableView.Dispose ();
				SerialPortsTableView = null;
			}

			if (BaudRateLabel != null) {
				BaudRateLabel.Dispose ();
				BaudRateLabel = null;
			}

			if (BaudRatePopUpButton != null) {
				BaudRatePopUpButton.Dispose ();
				BaudRatePopUpButton = null;
			}

			if (BaudRatesArrayController != null) {
				BaudRatesArrayController.Dispose ();
				BaudRatesArrayController = null;
			}

			if (SerialPortsListArrayController != null) {
				SerialPortsListArrayController.Dispose ();
				SerialPortsListArrayController = null;
			}
		}
	}

	[Register ("SerialPortSelector")]
	partial class SerialPortSelector
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
