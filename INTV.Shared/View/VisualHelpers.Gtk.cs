// <copyright file="VisualHelpers.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

using System.Linq;

namespace INTV.Shared.View
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public static partial class VisualHelpers
    {
        /// <summary>
        /// Gets parent visual of the given visual.
        /// </summary>
        /// <param name="visual">The visual whose parent is desired.</param>
        /// <returns>The parent visual.</returns>
        public static Gtk.Widget GetParentVisual(this Gtk.Widget visual)
        {
            var parent = visual.Parent;
            return parent;
        }

        /// <summary>
        /// Get the number of child visuals.
        /// </summary>
        /// <param name="visual">The visual whose child count is desired.</param>
        /// <returns>The number of child visuals in the given visual.</returns>
        public static int GetChildVisualCount(this Gtk.Widget visual)
        {
            var container = visual as Gtk.Container;
            var numChildren = container == null ? 0 : container.Children.Length;
            return numChildren;
        }

        /// <summary>
        /// Get the child visual at a specific index.
        /// </summary>
        /// <typeparam name="T">The type of the child visual to retrieve from the given index.</typeparam>
        /// <param name="visual">The visual containing the child.</param>
        /// <param name="index">The index of the child to get.</param>
        /// <returns>The child visual at the given index, as the desired type.</returns>
        public static T GetChildAtIndex<T>(this Gtk.Widget visual, int index) where T : class
        {
            T typedChild = null;
            var container = visual as Gtk.Container;
            if (container != null)
            {
                typedChild = container.Children[index] as T;
            }
            return typedChild;
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <returns>A value indicating whether the dialog was cancelled (<c>false</c>), aborted (<c>null</c>) or approved (<c>true</c>).</returns>
        /// <remarks>This function also handles the close button of the dialog, treating it as a 'cancel' operation.</remarks>
        public static bool? ShowDialog(this Gtk.Dialog dialog)
        {
            return VisualHelpers.ShowDialog(dialog, true);
        }

        /// <summary>
        /// Shows a modal dialog.
        /// </summary>
        /// <param name="dialog">The dialog to show.</param>
        /// <param name="alwaysClose">If set to <c>true</c> always close the dialog as soon as a response is known.</param>
        /// <param name="closeForResponses">If <paramref name="alwaysClose"/> is <c>false</c>, only the provided values will exit the dialog run loop.</param>
        /// <returns>A value indicating whether the dialog was cancelled (<c>false</c>), aborted (<c>null</c>) or approved (<c>true</c>).</returns>
        /// <remarks>Dialogs with 'sub-functions' should always use a response value of Gtk.ResponseType.None. Other explicit values
        /// may be ignored -- that is, only specific values will exit the dialog if the <paramref name="closeForResponses"/> argument
        /// is non-empty and <paramref name="alwaysClose"/> is <c>false</c>. If alwaysClose is false and no response values for close are
        /// provided, it is a Bad Thing and you shouldn't do that.</remarks>
        public static bool? ShowDialog(this Gtk.Dialog dialog, bool alwaysClose, params Gtk.ResponseType[] closeForResponses)
        {
            bool? accepted = null;
            var result = Gtk.ResponseType.None;
            do
            {
                result = (Gtk.ResponseType)dialog.Run();
                if (result != Gtk.ResponseType.None)
                {
                    switch(result)
                    {
                        case Gtk.ResponseType.Reject:
                        case Gtk.ResponseType.Cancel:
                        case Gtk.ResponseType.Close:
                        case Gtk.ResponseType.No:
                            accepted = false;
                            break;
                        case Gtk.ResponseType.Accept:
                        case Gtk.ResponseType.Apply:
                        case Gtk.ResponseType.Help:
                        case Gtk.ResponseType.Yes:
                        case Gtk.ResponseType.Ok:
                            accepted = true;
                            break;
                        default:
                            break;
                    }
                }
            } while (!alwaysClose && ((closeForResponses == null) || !closeForResponses.Contains(result)));
            VisualHelpers.Close(dialog);
            return accepted;
        }

        /// <summary>
        /// Close the specified window.
        /// </summary>
        /// <param name="window">The window to close.</param>
        public static void Close(this Gtk.Window window)
        {
            window.Destroy();
            INTV.Shared.ComponentModel.CommandManager.InvalidateRequerySuggested();
        }

        /// <summary>
        /// Cell renderer helper function for text cell rendering.
        /// </summary>
        /// <typeparam name="T">The data type of the element in the model.</typeparam>
        /// <param name="column">The column in which the cell is being rendered.</param>
        /// <param name="cell">The renderer attached to the cell.</param>
        /// <param name="model">The model containing data for the cell.</param>
        /// <param name="iter">The iterator into the model data.</param>
        /// <param name="textGetter">The delegate to call to get the text to render.</param>
        public static void CellTextColumnRenderer<T>(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter, System.Func<T, string> textGetter) where T : class
        {
            CellTextRenderer<T>((Gtk.CellLayout)column, cell, model, iter, textGetter);
        }

        /// <summary>
        /// Cell renderer helper function for text cell rendering.
        /// </summary>
        /// <typeparam name="T">The data type of the element in the model.</typeparam>
        /// <param name="cellLayout">The cell layout element being rendered.</param>
        /// <param name="cell">The renderer attached to the cell.</param>
        /// <param name="model">The model containing data for the cell.</param>
        /// <param name="iter">The iterator into the model data.</param>
        /// <param name="textGetter">The delegate to call to get the text to render.</param>
        public static void CellTextRenderer<T>(Gtk.CellLayout cellLayout, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter, System.Func<T, string> textGetter) where T : class
        {
            var data = model.GetValue(iter, 0) as T;
            ((Gtk.CellRendererText)cell).Text = textGetter(data);
        }

        /// <summary>
        /// Cell renderer helper function for image cell rendering.
        /// </summary>
        /// <typeparam name="T">The data type of the element in the model.</typeparam>
        /// <param name="column">The column in which the cell is being rendered.</param>
        /// <param name="cell">The renderer attached to the cell.</param>
        /// <param name="model">The model containing data for the cell.</param>
        /// <param name="iter">The iterator into the model data.</param>
        /// <param name="imageGetter">The delegate to call to get the image to render.</param>
        public static void CellImageColumnRenderer<T>(Gtk.TreeViewColumn column, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter, System.Func<T, Gdk.Pixbuf> imageGetter) where T : class
        {
            CellImageRenderer<T>((Gtk.CellLayout)column, cell, model, iter, imageGetter);
        }

        /// <summary>
        /// Cell renderer helper function for image cell rendering.
        /// </summary>
        /// <typeparam name="T">The data type of the element in the model.</typeparam>
        /// <param name="cellLayout">The cell layout element being rendered.</param>
        /// <param name="cell">The renderer attached to the cell.</param>
        /// <param name="model">The model containing data for the cell.</param>
        /// <param name="iter">The iterator into the model data.</param>
        /// <param name="imageGetter">The delegate to call to get the image to render.</param>
        public static void CellImageRenderer<T>(Gtk.CellLayout cellLayout, Gtk.CellRenderer cell, Gtk.TreeModel model, Gtk.TreeIter iter, System.Func<T, Gdk.Pixbuf> imageGetter) where T : class
        {
            var data = model.GetValue(iter, 0) as T;
            ((Gtk.CellRendererPixbuf)cell).Pixbuf = imageGetter(data);
        }

        private static System.Tuple<int, int, int> OSGetPrimaryDisplayInfo()
        {
            var mainScreen = Gdk.Screen.Default;
            var width = System.Convert.ToInt32(mainScreen.Width);
            var height = System.Convert.ToInt32(mainScreen.Height);
            var x = mainScreen.SystemVisual;
            var depth = Gdk.Visual.BestDepth; // hope this holds up
            return new System.Tuple<int, int, int>(width, height, depth);
        }
    }
}
