// <copyright file="StartDragProgramsCommand.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using INTV.Core.Model.Program;

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// Implements the IDragStartCommand for supporting the dragging of ProgramDescription objects (or their view models) in a UI.
    /// </summary>
    public class StartDragProgramsCommand : INTV.Shared.Behavior.MultiSelectDragStartCommand
    {
        private IEnumerable<ProgramDescription> _programs;

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the DragProgramsCommand class.
        /// </summary>
        /// <param name="execute">The function to execute when the drag operation starts.</param>
        public StartDragProgramsCommand(Action<object> execute)
            : base(execute)
        {
        }

        #endregion // Constructors

        #region Properties

        #region IDragStartCommand Properties

        /// <inheritdoc />
        public override string Format
        {
            get { return ProgramDescriptionViewModel.DragDataFormat; }
        }

        /// <inheritdoc />
        public override object Data
        {
            get { return Programs; }
        }

        /// <inheritdoc />
        public override System.Windows.DragDropEffects Effects
        {
            get { return System.Windows.DragDropEffects.Copy; }
        }

        #endregion // IDragStartCommand Properties

        /// <summary>
        /// Gets or sets the list of ProgramDescriptions to drag in the drag and drop operation.
        /// </summary>
        public IEnumerable<ProgramDescription> Programs
        {
            get { return _programs; }
            set { _programs = value; }
        }

        #endregion // Properties
    }
}
