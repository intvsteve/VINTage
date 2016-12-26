// <copyright file="InvokeProgramEventArgs.cs" company="INTV Funhouse">
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

using INTV.Core.Model.Program;

namespace INTV.Shared.Model.Program
{
    /// <summary>
    /// This event argument identifies when a <see cref="ProgramDescription"/> has been acted
    /// upon, typically implying the ROM should be run, downloaded, or similarly manipulated.
    /// </summary>
    public class InvokeProgramEventArgs : System.EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Model.Program.InvokeProgramEventArgs"/> class.
        /// </summary>
        /// <param name="programDescription">Program description.</param>
        internal InvokeProgramEventArgs(ProgramDescription programDescription)
        {
            Program = programDescription;
        }

        /// <summary>
        /// Gets the <see cref="ProgramDescription"/> identifying a ROM to be invoked.
        /// </summary>
        public ProgramDescription Program { get; private set; }

        /// <summary>
        /// Gets or sets a value indicating whether the event has been handled.
        /// </summary>
        /// <remarks>The first to mark the event handled will prevent subsequent processing fo the event.</remarks>
        public bool Handled { get; set; }
    }
}
