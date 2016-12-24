// <copyright file="VisualHelpers.WPF.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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

using System.Windows;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Windows-specific implementation.
    /// </summary>
    public static partial class VisualHelpers
    {
        /// <summary>
        /// Compatibility version of Window.ShowWindow.
        /// </summary>
        /// <param name="window">The window to run as a dialog.</param>
        /// <param name="asSheet">This value is ignored.</param>
        /// <returns>The dialog result.</returns>
        public static bool? ShowDialog(this Window window, bool asSheet)
        {
            return window.ShowDialog();
        }

        /// <summary>
        /// Gets parent visual of the given visual.
        /// </summary>
        /// <param name="visual">The visual whose parent is desired.</param>
        /// <returns>The parent visual.</returns>
        public static DependencyObject GetParentVisual(this DependencyObject visual)
        {
            var parent = (visual is System.Windows.Media.Visual) ? System.Windows.Media.VisualTreeHelper.GetParent(visual) : null;
            return parent;
        }

        /// <summary>
        /// Get the number of child visuals.
        /// </summary>
        /// <param name="visual">The visual whose child count is desired.</param>
        /// <returns>The number of child visuals in the given visual.</returns>
        public static int GetChildVisualCount(this DependencyObject visual)
        {
            var numChildren = System.Windows.Media.VisualTreeHelper.GetChildrenCount(visual);
            if (numChildren == 0)
            {
                var control = visual as System.Windows.Controls.Control;
                if (control != null)
                {
                    control.ApplyTemplate();
                    numChildren = System.Windows.Media.VisualTreeHelper.GetChildrenCount(control);
                }
            }
            return numChildren;
        }

        /// <summary>
        /// Get the child visual at a specific index.
        /// </summary>
        /// <typeparam name="T">The type of the child visual to retrieve from the given index.</typeparam>
        /// <param name="visual">The visual containing the child.</param>
        /// <param name="index">The index of the child to get.</param>
        /// <returns>The child visual at the given index, as the desired type.</returns>
        public static T GetChildAtIndex<T>(this DependencyObject visual, int index) where T : class
        {
            var typedChild = System.Windows.Media.VisualTreeHelper.GetChild(visual, index) as T;
            return typedChild;
        }

        /// <summary>
        /// Gets an owner window to use if the input is <c>null</c>.
        /// </summary>
        /// <param name="owner">If value of this argument is <c>null</c>, the last window in the application's window list will be used as the new value.</param>
        static partial void GetWindowOwner(ref Window owner)
        {
            if (owner == null)
            {
                var windows = System.Windows.Application.Current.Windows;
                var lastWindowIndex = windows.Count - 1;
                owner = windows[lastWindowIndex];
            }
        }

        /// <summary>
        /// Sets the owner of a window.
        /// </summary>
        /// <param name="newWindow">The window whose owner is being set.</param>
        /// <param name="owner">The window's owner.</param>
        static partial void SetWindowOwner(this Window newWindow, Window owner)
        {
            newWindow.WindowStartupLocation = WindowStartupLocation.CenterOwner;
            newWindow.Owner = owner;
        }
    }
}
