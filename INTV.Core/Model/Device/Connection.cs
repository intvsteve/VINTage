// <copyright file="Connection.cs" company="INTV Funhouse">
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

namespace INTV.Core.Model.Device
{
    /// <summary>
    /// Simplistic implementation of a device connection.
    /// </summary>
    public class Connection : INTV.Core.ComponentModel.ModelBase, IConnection
    {
        private string _name;
        private ConnectionType _type;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.Device.Connection"/> class.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="type">The kind of connection.</param>
        protected Connection(string name, ConnectionType type)
        {
            _name = name;
            _type = type;
        }

        /// <inheritdoc />
        public string Name
        {
            get { return _name; }
            protected set { AssignAndUpdateProperty("Name", value, ref _name); }
        }

        /// <inheritdoc />
        public ConnectionType Type
        {
            get { return _type; }
            protected set { AssignAndUpdateProperty("Type", value, ref _type); }
        }

        /// <summary>
        /// Creates a psuedo connection, useful for sitations in which a fully-functional
        /// connection implementation is not necessary.
        /// </summary>
        /// <param name="name">The name of the connection.</param>
        /// <param name="type">The kind of connection.</param>
        /// <returns>The psuedo connection.</returns>
        public static IConnection CreatePseudoConnection(string name, ConnectionType type)
        {
            return new Connection(name, type);
        }
    }
}
