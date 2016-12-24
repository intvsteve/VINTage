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
	[Register ("SerialPortSelectorDialogController")]
	partial class SerialPortSelectorDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSButton SelectPort { get; set; }

		[Outlet]
		MonoMac.AppKit.NSView SerialPortSelectorView { get; set; }

		[Action ("OnCancel:")]
		partial void OnCancel (MonoMac.Foundation.NSObject sender);

		[Action ("OnSelect:")]
		partial void OnSelect (MonoMac.Foundation.NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SelectPort != null) {
				SelectPort.Dispose ();
				SelectPort = null;
			}

			if (SerialPortSelectorView != null) {
				SerialPortSelectorView.Dispose ();
				SerialPortSelectorView = null;
			}
		}
	}

	[Register ("SerialPortSelectorDialog")]
	partial class SerialPortSelectorDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
