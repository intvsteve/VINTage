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
	partial class SystemCompatibilityConfigurationPageController
	{
		[Outlet]
		NSPopUpButton IntellivisionIIPopUpButton { get; set; }

		[Outlet]
		NSPopUpButton SuperVideoArcadePopUpButton { get; set; }

		[Outlet]
		NSPopUpButton TutorvisionPopUpButton { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (IntellivisionIIPopUpButton != null) {
				IntellivisionIIPopUpButton.Dispose ();
				IntellivisionIIPopUpButton = null;
			}

			if (SuperVideoArcadePopUpButton != null) {
				SuperVideoArcadePopUpButton.Dispose ();
				SuperVideoArcadePopUpButton = null;
			}

			if (TutorvisionPopUpButton != null) {
				TutorvisionPopUpButton.Dispose ();
				TutorvisionPopUpButton = null;
			}
		}
	}
}
