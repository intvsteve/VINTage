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
	[Register ("ReportDialogController")]
	partial class ReportDialogController
	{
		[Outlet]
		MonoMac.AppKit.NSButton CopyToClipboardButton { get; set; }

		[Outlet]
		MonoMac.AppKit.NSTextView ReportText { get; set; }

		[Action ("OnCopyToClipboard:")]
		partial void OnCopyToClipboard (MonoMac.Foundation.NSObject sender);

		[Action ("OnExit:")]
		partial void OnExit (MonoMac.Foundation.NSObject sender);

		[Action ("OnSendErrorReport:")]
		partial void OnSendErrorReport (MonoMac.Foundation.NSObject sender);

		[Action ("OnShowAttachments:")]
		partial void OnShowAttachments (MonoMac.Foundation.NSObject sender);
		
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
