// <copyright file="ConnectionTests.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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

using INTV.Core.Model.Device;
using Xunit;

namespace INTV.Core.Tests.Model.Device
{
    public class ConnectionTests
    {
        [Fact]
        public void Connection_CreatePseudoConnection_VerifyProperties()
        {
            var name = "Hankster";
            var connectionType = ConnectionType.None;

            var connection = Connection.CreatePseudoConnection(name, connectionType);

            Assert.Equal(name, connection.Name);
            Assert.Equal(connectionType, connection.Type);
        }

        [Fact]
        public void Connection_CreateConnectionAndAssignName_NameChanges()
        {
            var connection = new TestConnection();
            Assert.Equal(null, connection.Name);
            Assert.Equal(ConnectionType.None, connection.Type);
            var valueChangeCalled = false;
            connection.PropertyChanged += (s, e) =>
                {
                    valueChangeCalled = e.PropertyName == "Name";
                };

            connection.ChangeName("bub");

            Assert.True(valueChangeCalled);
            Assert.Equal("bub", connection.Name);
        }

        [Fact]
        public void Connection_CreateConnectionAndAssignType_TypeChanges()
        {
            var connection = new TestConnection();
            Assert.Equal(null, connection.Name);
            Assert.Equal(ConnectionType.None, connection.Type);
            var valueChangeCalled = false;
            connection.PropertyChanged += (s, e) =>
                {
                    valueChangeCalled = e.PropertyName == "Type";
                };

            connection.ChangeConnectionType(ConnectionType.CartridgePort);

            Assert.True(valueChangeCalled);
            Assert.Equal(ConnectionType.CartridgePort, connection.Type);
        }

        private class TestConnection : Connection
        {
            public TestConnection()
                : base(null, ConnectionType.None)
            {
            }

            public void ChangeName(string newName)
            {
                Name = newName;
            }

            public void ChangeConnectionType(ConnectionType newType)
            {
                Type = newType;
            }
        }
    }
}
