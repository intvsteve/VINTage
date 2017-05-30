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
	[Register ("JlpFeaturesConfigurationPageController")]
	partial class JlpFeaturesConfigurationPageController
	{
		[Outlet]
		NSNumberFormatter FlashSectorsFormatter { get; set; }

		[Outlet]
		NSPopUpButton JlpVersionPopUpButton { get; set; }

		[Outlet]
		NSPopUpButton SerialPortPopUpButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (FlashSectorsFormatter != null) {
				FlashSectorsFormatter.Dispose ();
				FlashSectorsFormatter = null;
			}

			if (JlpVersionPopUpButton != null) {
				JlpVersionPopUpButton.Dispose ();
				JlpVersionPopUpButton = null;
			}

			if (SerialPortPopUpButton != null) {
				SerialPortPopUpButton.Dispose ();
				SerialPortPopUpButton = null;
			}
		}
	}

	[Register ("JlpFeaturesConfigurationPage")]
	partial class JlpFeaturesConfigurationPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
