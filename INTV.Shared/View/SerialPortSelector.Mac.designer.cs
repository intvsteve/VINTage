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
	[Register ("SerialPortSelectorController")]
	partial class SerialPortSelectorController
	{
		[Outlet]
		NSTextField BaudRateLabel { get; set; }

		[Outlet]
		NSPopUpButton BaudRatePopUpButton { get; set; }

		[Outlet]
		NSArrayController BaudRatesArrayController { get; set; }

		[Outlet]
		NSArrayController SerialPortsListArrayController { get; set; }

		[Outlet]
		NSTableView SerialPortsTableView { get; set; }

		[Action ("SerialPortDoubleClick:")]
		partial void SerialPortDoubleClick (NSObject sender);
		
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
