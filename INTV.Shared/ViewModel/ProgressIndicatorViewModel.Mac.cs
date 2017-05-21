// <copyright file="ProgressIndicatorViewModel.Mac.cs" company="INTV Funhouse">
// Copyright (c) 2014-2017 All Rights Reserved
// <author>Steven A. Orth</author>
//
// This program is free software: you can redistribute it and/or modify it
// under the terms of the GNU General Public License as published by the
// Free Software Foundation, either version 2 of the License, or (at your
// option) any later version.
//
// This program is distributed in the hope that it will be useful, but
// WITHOUT ANY WARRANTY; without even the implied warranty of MERCHANTABILITY
// or FITNESS FOR A PARTICULAR PURPOSE. See the GNU General Public License
// for more details.
//
// You should have received a copy of the GNU General Public License along
// with this software. If not, see: http://www.gnu.org/licenses/.
// or write to the Free Software Foundation, Inc.,
// 51 Franklin Street, Fifth Floor, Boston, MA 02110-1301, USA
// </copyright>

////#define ENABLE_DEBUG_OUTPUT

#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__
using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class ProgressIndicatorViewModel
    {
        private static readonly string ProgressIndicatorViewModelDataContextProperty = ProgressIndicatorViewModelDataContextPropertyName;

        private NSObjectPredicate _previousShouldClosePredicate;

        private void PlatformOnShow(SingleInstanceApplication application)
        {
            if ((application != null) && (application.MainWindow != null))
            {
                _previousShouldClosePredicate = application.MainWindow.WindowShouldClose;
                application.MainWindow.WindowShouldClose = MainWindowShouldClose;
            }
#if ENABLE_DEBUG_OUTPUT
            System.Diagnostics.Debug.WriteLine("****** SHOWING PROGRESS from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
#endif // ENABLE_DEBUG_OUTPUT
        }

        private void PlatformOnHide(SingleInstanceApplication application)
        {
            if ((application != null) && (application.MainWindow != null))
            {
                application.MainWindow.WindowShouldClose = _previousShouldClosePredicate;
                _previousShouldClosePredicate = null;
            }
        }

        private void MainWindowClosing(object sender, System.EventArgs e)
        {
            ErrorReporting.ReportNotImplementedError("ProgressIndicatorViewModel.MainWindowClosing");
        }

        private bool MainWindowShouldClose(NSObject sender)
        {
            return !IsVisible && (_timer == null);
        }

        static partial void InitializeProgressIndicatorIfNecessary(bool ready)
        {
            if (ready && (SingleInstanceApplication.Current.MainWindow.GetValue(ProgressIndicatorViewModelDataContextProperty) == null))
            {
                // Lazy-initialize the progress indicator.
                var controller = SingleInstanceApplication.Current.MainWindow.WindowController as NSWindowController;
                controller.TryToPerformwith(new Selector("finishInitialization:"), SingleInstanceApplication.Current.MainWindow);
            }
        }
    }
}
