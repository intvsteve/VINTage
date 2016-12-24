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
	[Register ("SplashScreenController")]
	partial class SplashScreenController
	{
		[Outlet]
		MonoMac.AppKit.NSImageView SplashScreenImageView { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (SplashScreenImageView != null) {
				SplashScreenImageView.Dispose ();
				SplashScreenImageView = null;
			}
		}
	}

	[Register ("SplashScreen")]
	partial class SplashScreen
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
