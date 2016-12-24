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
	partial class EcsFeaturesConfigurationPageController
	{
		[Outlet]
		MonoMac.AppKit.NSPopUpButton EcsCompatibilityPopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton SerialPortUsagePopUpButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (EcsCompatibilityPopUpButton != null) {
				EcsCompatibilityPopUpButton.Dispose ();
				EcsCompatibilityPopUpButton = null;
			}

			if (SerialPortUsagePopUpButton != null) {
				SerialPortUsagePopUpButton.Dispose ();
				SerialPortUsagePopUpButton = null;
			}
		}
	}
}
