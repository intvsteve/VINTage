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
	[Register ("ReportDialogController")]
	partial class ReportDialogController
	{
		[Outlet]
		NSButton CopyToClipboardButton { get; set; }

		[Outlet]
		NSTextView ReportText { get; set; }

		[Action ("OnCopyToClipboard:")]
		partial void OnCopyToClipboard (NSObject sender);

		[Action ("OnExit:")]
		partial void OnExit (NSObject sender);

		[Action ("OnSendErrorReport:")]
		partial void OnSendErrorReport (NSObject sender);

		[Action ("OnShowAttachments:")]
		partial void OnShowAttachments (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (CopyToClipboardButton != null) {
				CopyToClipboardButton.Dispose ();
				CopyToClipboardButton = null;
			}

			if (ReportText != null) {
				ReportText.Dispose ();
				ReportText = null;
			}
		}
	}

	[Register ("ReportDialog")]
	partial class ReportDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
