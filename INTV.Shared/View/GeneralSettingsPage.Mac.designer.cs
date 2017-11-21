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
	[Register ("GeneralSettingsPageController")]
	partial class GeneralSettingsPageController
	{
		[Action ("ShowBackupDirectory:")]
		partial void ShowBackupDirectory (NSObject sender);

		[Action ("ShowBoxesDirectory:")]
		partial void ShowBoxesDirectory (NSObject sender);

		[Action ("ShowErrorLogsDirectory:")]
		partial void ShowErrorLogsDirectory (NSObject sender);

		[Action ("ShowLabelsDirectory:")]
		partial void ShowLabelsDirectory (NSObject sender);

		[Action ("ShowManualsDirectory:")]
		partial void ShowManualsDirectory (NSObject sender);

		[Action ("ShowOverlaysDirectory:")]
		partial void ShowOverlaysDirectory (NSObject sender);

		[Action ("ShowRomsDirectory:")]
		partial void ShowRomsDirectory (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("GeneralSettingsPage")]
	partial class GeneralSettingsPage
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
