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
	[Register ("SplashScreenController")]
	partial class SplashScreenController
	{
		[Outlet]
		NSImageView SplashScreenImageView { get; set; }
		
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
