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
	[Register ("CompressedArchiveAccessSettingsPageController")]
	partial class CompressedArchiveAccessSettingsPageController
	{
		[Outlet]
		NSArrayController CompressedArchiveFormatsArrayController { get; set; }

		[Outlet]
		NSTextField MaxArchiveSizeLabel { get; set; }

		[Action ("OnCompressedArchiveModeSelected:")]
		partial void OnCompressedArchiveModeSelected (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
			if (MaxArchiveSizeLabel != null) {
				MaxArchiveSizeLabel.Dispose ();
				MaxArchiveSizeLabel = null;
			}

			if (CompressedArchiveFormatsArrayController != null) {
				CompressedArchiveFormatsArrayController.Dispose ();
				CompressedArchiveFormatsArrayController = null;
			}
		}
	}

	[Register ("CompressedArchiveAccessSettingsPage")]
	partial class CompressedArchiveAccessSettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
