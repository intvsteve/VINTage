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
	partial class SystemCompatibilityConfigurationPageController
	{
		[Outlet]
		MonoMac.AppKit.NSPopUpButton IntellivisionIIPopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton SuperVideoArcadePopUpButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSPopUpButton TutorvisionPopUpButton { get; set; }
		
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
