// WARNING
//
// This file has been generated automatically by Xamarin Studio to store outlets and
// actions made in the UI designer. If it is removed, they will be lost.
// Manual changes to this file may not be handled correctly.
//

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using System.CodeDom.Compiler;

namespace INTV.LtoFlash.View
{
	[Register ("PromptToInstallFtdiDriverDialogController")]
	partial class PromptToInstallFtdiDriverDialogController
	{
		[Action ("DoNotAskAgain:")]
		partial void DoNotAskAgain (NSObject sender);

		[Action ("DoNotInstall:")]
		partial void DoNotInstall (NSObject sender);

		[Action ("LaunchInstaller:")]
		partial void LaunchInstaller (NSObject sender);
		
		void ReleaseDesignerOutlets ()
		{
		}
	}

	[Register ("PromptToInstallFtdiDriverDialog")]
	partial class PromptToInstallFtdiDriverDialog
	{
		
		void ReleaseDesignerOutlets ()
		{
		}
	}
}
