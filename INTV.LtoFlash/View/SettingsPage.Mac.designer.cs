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
	[Register ("SettingsPageController")]
	partial class SettingsPageController
	{
		[Outlet]
		NSArrayController SerialPortReadChunkSizesArrayController { get; set; }

		[Outlet]
		NSArrayController SerialPortWriteChunkSizesArrayController { get; set; }

		[Action ("_reconcileDeviceMenuToLocalMenu:")]
		partial void _reconcileDeviceMenuToLocalMenu (NSObject sender);

		[Action ("_searchAtStartupAction:")]
		partial void _searchAtStartupAction (NSObject sender);

		[Action ("_validateMenuAtStartupAction:")]
		partial void _validateMenuAtStartupAction (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (SerialPortWriteChunkSizesArrayController != null) {
				SerialPortWriteChunkSizesArrayController.Dispose ();
				SerialPortWriteChunkSizesArrayController = null;
			}

			if (SerialPortReadChunkSizesArrayController != null) {
				SerialPortReadChunkSizesArrayController.Dispose ();
				SerialPortReadChunkSizesArrayController = null;
			}
		}
	}

	[Register ("SettingsPage")]
	partial class SettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
