// <copyright file="ROMsTableView.Mac.cs" company="INTV Funhouse">
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
//

using INTV.Shared.Commands;
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
    /// Subclass NSTableView to get context menus to work the way we want.
    /// </summary>
    [Register("ROMsTableView")]
    public class ROMsTableView : NSTableView
    {
        #region Constructors

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public ROMsTableView(System.IntPtr handle)
            : base(handle)
        {
            Initialize();
        }

        /// <summary>
        /// Called when created directly from a XIB file.
        /// </summary>
        /// <param name="coder">Used to deserialize from a XIB.</param>
        [Export("initWithCoder:")]
        public ROMsTableView(NSCoder coder)
            : base(coder)
        {
            Initialize();
        }

        /// <summary>Shared initialization code.</summary>
        private void Initialize()
        {
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets or sets the controller of the table.
        /// </summary>
        internal RomListViewController Controller { get; set; }

        #endregion // Properties

        /// <inheritdoc/>
        public override NSMenu MenuForEvent(NSEvent theEvent)
        {
            // Handy advice found here:
            // http://forums.macrumors.com/threads/right-clicks-control-clicks-and-contextual-menus.166469/
#if false
            //Find which row is under the cursor
            [[self window] makeFirstResponder:self];
            NSPoint menuPoint = [self convertPoint:[event locationInWindow] fromView:nil];
            int row = [self rowAtPoint:menuPoint];

            /* Update the table selection before showing menu
            Preserves the selection if the row under the mouse is selected (to allow for
            multiple items to be selected), otherwise selects the row under the mouse */
            BOOL currentRowIsSelected = [[self selectedRowIndexes] containsIndex:row];
            if (!currentRowIsSelected)
            [self selectRow:row byExtendingSelection:NO];

            if ([self numberOfSelectedRows] <=0)
            {
            //No rows are selected, so the table should be displayed with all items disabled
            NSMenu* tableViewMenu = [[self menu] copy];
            int i;
            for (i=0;i<[tableViewMenu numberOfItems];i++)
            [[tableViewMenu itemAtIndex:i] setEnabled:NO];
            return [tableViewMenu autorelease];
            }
            else
            return [self menu];
#endif // false
            // Select the row we got context click for.
            Window.MakeFirstResponder(this);
            var menuPoint = ConvertPointFromView(theEvent.LocationInWindow, null);
            var row = GetRow(menuPoint);
            if (row >= 0)
            {
                var rowAlreadySelected = SelectedRows.Contains((uint)row);
                if (!rowAlreadySelected)
                {
                    SelectRow(row, false);
                }
            }

            // Build the context menu.
            var target = (row < 0) ? null : Controller.GetObjectAtRow((int)row);
            var context = Controller.View.ViewModel;
            Menu = target.CreateContextMenu("ROMListContextMenu", context);
            return base.MenuForEvent(theEvent);
        }

        /// <inheritdoc/>
        public override bool PerformKeyEquivalent(NSEvent theEvent)
        {
            var didIt = base.PerformKeyEquivalent(theEvent);
            if (!didIt)
            {
                var deleteCommand = RomListCommandGroup.RemoveRomsCommand;
                var context = RomListCommandGroup.Group.Context;
                didIt = this.PerformKeyEquivalentForDelete(theEvent, deleteCommand, context);
            }
            return didIt;
        }

        /// <summary>
        /// Called when ESC or Cmd+. pressed. Overridden to get rid of annoying beep.
        /// </summary>
        /// <param name="sender">The cancel button.</param>
        [Export("cancelOperation:")]
        public void CancelOperation(NSObject sender)
        {
        }
    }
}
