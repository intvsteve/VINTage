// <copyright file="FileNodeViewModel.Mac.cs" company="INTV Funhouse">
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

using System.Collections.Generic;
using System.Linq;
#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Core.Model.Program;
using INTV.LtoFlash.Model;
using INTV.Shared.ComponentModel;

namespace INTV.LtoFlash.ViewModel
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    [Register("FileNodeViewModel")]
    public abstract partial class FileNodeViewModel : NSObject
    {
        #region Property Names

        public const string ItemsPropertyName = "Items";
        public const string ItemCountPropertyName = "ItemCount";

        #endregion // Property Names

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.LtoFlash.ViewModel.FileNodeViewModel"/> class.
        /// </summary>
        public FileNodeViewModel()
        {
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public FileNodeViewModel(System.IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// Gets the child objects.
        /// </summary>
        /// <remarks>Acts as "model" for Cocoa UI for <see cref="NSOutlineView"/>.</remarks>
        [INTV.Shared.Utility.OSExport("Items")]
        public virtual NSMutableArray Children
        {
            get
            {
                if (_children == null)
                {
                    _children = new NSMutableArray();
                }
                return _children;
            }
        }
        private NSMutableArray _children;

        /// <summary>
        /// Gets or sets the number of child objects.
        /// </summary>
        /// <remarks>Acts as "model" for Cocoa UI for <see cref="NSOutlineView"/>.</remarks>
        [INTV.Shared.Utility.OSExport(ItemCountPropertyName)]
        public virtual uint ItemCount
        {
            get { return 0; }
            protected set { }
        }

        /// <summary>
        /// Gets whether this is a leaf node in the data tree.
        /// </summary>
        /// <remarks>Acts as "model" for Cocoa UI for <see cref="NSOutlineView"/>.</remarks>
        [INTV.Shared.Utility.OSExport("IsLeaf")]
        public bool IsLeaf
        {
            get { return !(this is INTV.LtoFlash.Model.IFileContainer); }
        }

        /// <summary>
        /// Adds child items to this container.
        /// </summary>
        /// <param name="menuLayout">The root of the data tree.</param>
        /// <param name="insertIndex">The location at which to add the items.</param>
        /// <param name="items">The items to add.</param>
        internal void AddItems(MenuLayoutViewModel menuLayout, int insertIndex, IEnumerable<ProgramDescription> items)
        {
            IFileContainer dropTarget = Model as IFileContainer;
            AddItems(menuLayout, dropTarget, items, insertIndex);
        }
    }
}
