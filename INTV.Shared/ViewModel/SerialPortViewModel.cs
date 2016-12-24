// <copyright file="SerialPortViewModel.cs" company="INTV Funhouse">
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

using INTV.Core.ComponentModel;
using INTV.Shared.Utility;

#if WIN
using BaseClass = System.Object;
#elif MAC
using BaseClass = MonoMac.Foundation.NSObject;
#endif

namespace INTV.Shared.ViewModel
{
    /// <summary>
    /// The ViewModel for a serial port for use with dialogs.
    /// </summary>
    public class SerialPortViewModel : BaseClass, System.ComponentModel.INotifyPropertyChanged
    {
        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="portName">The name of the serial port to represent.</param>
        public SerialPortViewModel(string portName)
            : this(portName, true)
        {
        }

        /// <summary>
        /// Initializes a new instance of the type.
        /// </summary>
        /// <param name="portName">The name of the serial port to represent.</param>
        /// <param name="isSelectable">If <c>true</c>, the port may be selected; otherwise, it cannot.</param>
        public SerialPortViewModel(string portName, bool isSelectable)
        {
            PortName = portName;
            IsSelectable = isSelectable;
        }

        /// <summary>
        /// Gets the name of the serial port.
        /// </summary>
        [OSExport("PortName")]
        public string PortName { get; private set; }

        /// <summary>
        /// Gets a value indicating whether the port can be selected for use.
        /// </summary>
        public bool IsSelectable
        {
            get { return _isSelectable; }
            internal set { this.AssignAndUpdateProperty(PropertyChanged, "IsSelectable", value, ref _isSelectable); }
        }
        private bool _isSelectable;

        #region INotifyPropertyChanged

        /// <inheritdoc />
        public event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;

        #endregion // INotifyPropertyChanged

        /// <summary>
        /// Equality operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the equality.</param>
        /// <param name="rhs">The right-hand side of the equality.</param>
        /// <returns><c>true</c> if the two values refer to the same serial port by port name.</returns>
        public static bool operator ==(SerialPortViewModel lhs, SerialPortViewModel rhs)
        {
            bool areEqual = object.ReferenceEquals(lhs, rhs);
            if (!areEqual && !object.ReferenceEquals(lhs, null) && !object.ReferenceEquals(rhs, null))
            {
                areEqual = lhs.PortName == rhs.PortName;
            }
            return areEqual;
        }

        /// <summary>
        /// Inequality operator.
        /// </summary>
        /// <param name="lhs">The left-hand side of the inequality.</param>
        /// <param name="rhs">The right-hand side of the inequality.</param>
        /// <returns><c>true</c> if the two values refer to the different serial ports by comparing the port names.</returns>
        public static bool operator !=(SerialPortViewModel lhs, SerialPortViewModel rhs)
        {
            return !(lhs == rhs);
        }

        #region object Overrides

        /// <inheritdoc />
        public override bool Equals(object obj)
        {
            bool areEqual = obj is SerialPortViewModel;
            if (areEqual)
            {
                var other = (SerialPortViewModel)obj;
                areEqual = PortName == other.PortName;
            }
            return areEqual;
        }

        /// <inheritdoc />
        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        /// <inheritdoc />
        public override string ToString()
        {
            return PortName;
        }

        #endregion // object Overrides
    }
}
