// <copyright file="PeripheralTests.cs" company="INTV Funhouse">
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

using System;
using System.Collections.Generic;
using INTV.Core.Model.Device;
using INTV.Core.Model.Program;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class PeripheralTests
    {
        [Fact]
        public void Peripheral_AttachAndDetach_EventsRaised()
        {
            var attachedDeviceIpAddress = string.Empty;
            IntelliNet attachedDevice = null;
            var attachedHandler = new EventHandler<PeripheralEventArgs>((s, e) =>
                {
                    attachedDeviceIpAddress = e.UniqueId;
                    attachedDevice = s as IntelliNet;
                });

            var detachedDeviceIpAddress = string.Empty;
            IntelliNet detachedDevice = null;
            var detachedHandler = new EventHandler<PeripheralEventArgs>((s, e) =>
                {
                    detachedDeviceIpAddress = e.UniqueId;
                    detachedDevice = s as IntelliNet;
                });

            Peripheral.PeripheralAttached += attachedHandler;
            Peripheral.PeripheralDetached += detachedHandler;
            var intelliNet = new IntelliNet();
            const string ResolvedIpAddress = "127.0.0.1";
            if (intelliNet.ResolveIpAddress(ResolvedIpAddress))
            {
                intelliNet.Connect();
                intelliNet.Disconnect();
            }

            Assert.True(object.ReferenceEquals(intelliNet, attachedDevice));
            Assert.Equal(ResolvedIpAddress, attachedDeviceIpAddress);
            Assert.Equal(ResolvedIpAddress, attachedDevice.Ipv4Address);
            Assert.True(object.ReferenceEquals(intelliNet, detachedDevice));
            Assert.Equal(ResolvedIpAddress, detachedDeviceIpAddress);
            Assert.Equal(ResolvedIpAddress, detachedDevice.Ipv4Address);

            Peripheral.PeripheralAttached -= attachedHandler;
            Peripheral.PeripheralDetached -= detachedHandler;
        }

        private class IntelliNet : Peripheral
        {
            public IntelliNet()
            {
                Name = "IntelliNet";
                Ipv4Address = "0.0.0.0";
            }

            public override IEnumerable<IConnection> Connections
            {
                get { yield break; }
                protected set { }
            }

            public string Ipv4Address { get; private set; }

            public override bool IsRomCompatible(IProgramDescription programDescription)
            {
                return false;
            }

            public bool ResolveIpAddress(string resolvedIpv4Address)
            {
                Ipv4Address = resolvedIpv4Address;
                return true;
            }

            public void Connect()
            {
                RaisePeripheralAttached(this, Ipv4Address);
            }

            public void Disconnect()
            {
                RaisePeripheralDetached(this, Ipv4Address);
            }
        }
    }
}
