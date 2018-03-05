// <copyright file="ProgressIndicatorViewModel.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017-2018 All Rights Reserved
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

using INTV.Shared.Utility;
using INTV.Shared.View;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class ProgressIndicatorViewModel
    {
        private static readonly string ProgressIndicatorViewModelDataContextProperty = ProgressIndicatorViewModelDataContextPropertyName;

        /// <summary>
        /// GTK-specific implementation.
        /// </summary>
        /// <param name="ready">If set to <c>true</c>, application is ready.</param>
        static partial void InitializeProgressIndicatorIfNecessary(bool ready)
        {
            if (ready && (SingleInstanceApplication.Current.MainWindow.GetValue(ProgressIndicatorViewModelDataContextProperty) == null))
            {
                // Lazy-initialize the progress indicator viewmodel.
                var progressIndicatorViewModel = new ProgressIndicatorViewModel();
                System.Diagnostics.Debug.Assert(object.ReferenceEquals(progressIndicatorViewModel, SingleInstanceApplication.Current.MainWindow.GetValue(ProgressIndicatorViewModelDataContextProperty)), "We got the wrong progress indicator visual!");
                ProgressIndicator.Initialize(progressIndicatorViewModel);
            }
        }

        private void PlatformOnShow(SingleInstanceApplication application)
        {
            DebugOutput("****** PlatformOnShow on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            if ((application != null) && (application.MainWindow != null))
            {
                application.MainWindow.Destroyed += HandleMainWindowDestroyed;
            }
        }

        private void PlatformOnHide(SingleInstanceApplication application)
        {
            DebugOutput("****** PlatformOnHide on thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId);
            if ((application != null) && (application.MainWindow != null))
            {
                application.MainWindow.Destroyed -= HandleMainWindowDestroyed;
            }
        }

        private void HandleMainWindowDestroyed(object sender, System.EventArgs e)
        {
            // TODO: Hmm.... This may be completely unnecessary... This is used on Mac and Windows
            // to prevent the main window from closing via the 'close box'. However, in GTK, we prevent
            // the main window from being accessible during progress indicator...
            DebugOutput("****** HandleMainWindowDestroyed");
        }
    }
}
