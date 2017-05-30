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
	[Register ("ProgressIndicatorController")]
	partial class ProgressIndicatorController
	{
		[Outlet]
		NSButton Cancel { get; set; }

		[Outlet]
		NSView FeedbackArea { get; set; }

		[Outlet]
		NSProgressIndicator ProgressBar { get; set; }

		[Outlet]
		NSTextField UpdateText { get; set; }
		
		void ReleaseDesignerOutlets ()
		{
			if (Cancel != null) {
				Cancel.Dispose ();
				Cancel = null;
			}

			if (FeedbackArea != null) {
				FeedbackArea.Dispose ();
				FeedbackArea = null;
			}

			if (ProgressBar != null) {
				ProgressBar.Dispose ();
				ProgressBar = null;
			}

			if (UpdateText != null) {
				UpdateText.Dispose ();
				UpdateText = null;
			}
		}
	}

	[Register ("ProgressIndicator")]
	partial class ProgressIndicator
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
