// <copyright file="VisualHelpers.Mac.cs" company="INTV Funhouse">
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

using INTV.Shared.Commands;
using INTV.Shared.ComponentModel;
using INTV.Shared.Utility;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.View
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public static partial class VisualHelpers
    {
        /// <summary>
        /// Gets parent visual of the given visual.
        /// </summary>
        /// <param name="visual">The visual whose parent is desired.</param>
        /// <returns>The parent visual.</returns>
        public static NSView GetParentVisual(this NSView visual)
        {
            var parent = visual.Superview;
            return parent;
        }

        /// <summary>
        /// Get the number of child visuals.
        /// </summary>
        /// <param name="visual">The visual whose child count is desired.</param>
        /// <returns>The number of child visuals in the given visual.</returns>
        public static int GetChildVisualCount(this NSView visual)
        {
            var numChildren = visual.Subviews.Length;
            return numChildren;
        }

        /// <summary>
        /// Get the child visual at a specific index.
        /// </summary>
        /// <typeparam name="T">The type of the child visual to retrieve from the given index.</typeparam>
        /// <param name="visual">The visual containing the child.</param>
        /// <param name="index">The index of the child to get.</param>
        /// <returns>The child visual at the given index, as the desired type.</returns>
        public static T GetChildAtIndex<T>(this NSView visual, int index) where T : class
        {
            var typedChild = visual.Subviews[index] as T;
            return typedChild;
        }

        /// <summary>
        /// Ensures that an event handler is invoked on main the thread.
        /// </summary>
        /// <param name="instance">Typically a NSController or NSView that must handle an event from a ViewModel.</param>
        /// <param name="sender">Sender of the orginal event.</param>
        /// <param name="args">Event data.</param>
        /// <param name="handler">The event handler to ensure is executed on the main thread.</param>
        /// <typeparam name="TEventArgs">The data type of the event handler's data.</typeparam>
        public static void HandleEventOnMainThread<TEventArgs>(this NSResponder instance, object sender, TEventArgs args, System.Action<object, TEventArgs> handler) where TEventArgs : System.EventArgs
        {
            if (!OSDispatcher.IsMainThread)
            {
                instance.InvokeOnMainThread(() => handler(sender, args));
                return;
            }
            handler(sender, args);
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="window">The dialog to show.</param>
        /// <returns>A value indicating whether the dialog was cancelled (<c>false</c>), aborted (<c>null</c>) or approved (<c>true</c>).</returns>
        /// <remarks>This function also handles the close button of the dialog, treating it as a 'cancel' operation.</remarks>
        public static bool? ShowDialog(this NSWindow window)
        {
            return window.ShowDialog(false);
        }

        /// <summary>
        /// Shows a modal dialog, optionally as a sheet.
        /// </summary>
        /// <param name="window">The dialog to show.</param>
        /// <param name="asSheet">If <c>true</c>, attempt to show the dialog as a sheet.</param>
        /// <returns>A value indicating whether the dialog was cancelled (<c>false</c>), aborted (<c>null</c>) or approved (<c>true</c>).</returns>
        /// <remarks>This function also handles the close button of the dialog, treating it as a 'cancel' operation.</remarks>
        public static bool? ShowDialog(this NSWindow window, bool asSheet)
        {
            window.WillClose += HandleWillClose;
            window.BeginInvokeOnMainThread(INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested);
            if (asSheet)
            {
                var sheetOwner = INTV.Shared.Utility.SingleInstanceApplication.Current.MainWindow;
                INTV.Shared.Utility.SingleInstanceApplication.Current.BeginSheet(window, sheetOwner);
            }
            var result = INTV.Shared.Utility.SingleInstanceApplication.Current.RunModalForWindow(window);
            var accepted = result == (int)NSRunResponse.Stopped;
            return accepted;
        }

        /// <summary>
        /// Ends a modal dialog.
        /// </summary>
        /// <param name="window">The dialog to end.</param>
        /// <param name="response">The result to report for the dialog ending.</param>
        public static void EndDialog(this NSWindow window, NSRunResponse response)
        {
            window.WillClose -= HandleWillClose;
            SingleInstanceApplication.Current.StopModalWithCode((int)response);
            if (window.IsSheet)
            {
                SingleInstanceApplication.Current.EndSheet(window);
            }
            window.Close();
            window.Dispose();
        }

        private static void HandleWillClose(object sender, System.EventArgs e)
        {
            var closeNotification = sender as NSNotification;
            var dialog = closeNotification.Object as NSWindow;
            dialog.WillClose -= HandleWillClose;
            INTV.Shared.Utility.SingleInstanceApplication.Instance.StopModalWithCode((int)NSRunResponse.Aborted);
            dialog.Dispose();
        }

        private static System.Tuple<int, int, int> OSGetPrimaryDisplayInfo()
        {
            var mainScreen = NSScreen.MainScreen;
            var width = System.Convert.ToInt32(mainScreen.Frame.Width);
            var height = System.Convert.ToInt32(mainScreen.Frame.Height);
            var depth = (int)NSGraphics.BitsPerPixelFromDepth(mainScreen.Depth);
            return new System.Tuple<int, int, int>(width, height, depth);
        }

        /// <summary>
        /// Platform-specific method to attach a command's CanExecute handler.
        /// </summary>
        /// <param name="commandGroup">Command group.</param>
        /// <param name="command">Command to attach handler to.</param>
        static partial void AttachCanExecuteHandler(CommandGroup commandGroup, RelayCommand command)
        {
            if (commandGroup != null)
            {
                commandGroup.AttachCanExecuteChangeHandler(command);
            }
        }
    }
}
