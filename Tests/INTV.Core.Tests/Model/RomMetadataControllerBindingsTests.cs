// <copyright file="RomMetadataControllerBindingsTests.cs" company="INTV Funhouse">
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
using System.Linq;
using System.Text;
using INTV.Core.Model;
using INTV.Core.Utility;
using Xunit;

namespace INTV.Core.Tests.Model
{
    public class RomMetadataControllerBindingsTests
    {
        private static readonly Dictionary<byte, string> KeyboardKeyMap = new Dictionary<byte, string>()
        {
            { 0xD0, "1/=" },
            { 0xD1, "2/\"" },
            { 0xD2, "3/#" },
            { 0xD3, "4/$" },
            { 0xD4, "5/+" },
            { 0xD5, "6/-" },
            { 0xD6, "7//" },
            { 0xD7, "8/*" },
            { 0xD8, "9/(" },
            { 0xD9, "0/)" },
            { 0xDA, "ESC" },
            { 0xDB, "CTL" },
            { 0xDC, "Q" },
            { 0xDD, "W" },
            { 0xDE, "E" },
            { 0xDF, "R" },
            { 0xE0, "T" },
            { 0xE1, "Y" },
            { 0xE2, "U" },
            { 0xE3, "I" },
            { 0xE4, "O" },
            { 0xE5, "P" },
            { 0xE6, "UP/^" },
            { 0xE7, "A" },
            { 0xE8, "S" },
            { 0xE9, "D" },
            { 0xEA, "F" },
            { 0xEB, "G" },
            { 0xEC, "H" },
            { 0xED, "J" },
            { 0xEE, "K" },
            { 0xEF, "L" },
            { 0xF0, ";/:" },
            { 0xF1, "LEFT/%" },
            { 0xF2, "RIGHT/`" },
            { 0xF3, "Z" },
            { 0xF4, "X" },
            { 0xF5, "C" },
            { 0xF6, "V" },
            { 0xF7, "B" },
            { 0xF8, "N" },
            { 0xF9, "M" },
            { 0xFA, ",/<" },
            { 0xFB, "./>" },
            { 0xFC, "DOWN/?" },
            { 0xFD, "SHIFT" },
            { 0xFE, "SPACE" },
            { 0xFF, "RETURN" },
        };

        public static IEnumerable<object[]> ControllerBindingTheoryData
        {
            get
            {
                var controllerSelectors = Enum.GetValues(typeof(ControllerSelector)).Cast<ControllerSelector>();
                var controllerInputs = Enum.GetValues(typeof(ControllerInput)).Cast<ControllerInput>();
                foreach (var controllerSelector in controllerSelectors)
                {
                    var expectedController = Controller.None;
                    switch (controllerSelector)
                    {
                        case ControllerSelector.Controller0:
                            expectedController = Controller.Controller0;
                            break;
                        case ControllerSelector.Controller1:
                            expectedController = Controller.Controller1;
                            break;
                        case ControllerSelector.Controller2:
                            expectedController = Controller.Controller2;
                            break;
                        case ControllerSelector.Controller3:
                            expectedController = Controller.Controller3;
                            break;
                        case ControllerSelector.AnyController:
                            expectedController = Controller.AnyController;
                            break;
                    }
                    foreach (var controllerInput in controllerInputs)
                    {
                        var controllerDescriptor = (byte)controllerSelector | (byte)controllerInput;
                        var inputDescription = controllerSelector.ToString() + ": " + controllerInput.ToString();
                        yield return new object[] { controllerDescriptor, inputDescription, (byte)expectedController };
                    }
                }

                foreach (var keyboardKey in KeyboardKeyMap)
                {
                    var expectedController = Controller.Keyboard;
                    yield return new object[] { keyboardKey.Key, keyboardKey.Value, (byte)expectedController };
                }
            }
        }

        [Theory]
        [MemberData("ControllerBindingTheoryData")]
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Justification = "This is ensuring the behavior of 'LeaveOpen' in BinaryWriter' works correctly.")]
        public void RomMetadataControllerBinding_DeserializeOneBinding_ProducesExpectedControllerBinding(byte controllerBinding, string controllerDescription, byte expectedController)
        {
            using (var stream = new System.IO.MemoryStream())
            {
                var description = Encoding.UTF8.GetBytes(controllerDescription);
                var controllerBindingsMetadata = new RomMetadataControllerBindings((uint)description.Length + sizeof(byte));
                stream.WriteByte(controllerBinding);
                stream.Write(description, 0, description.Length);
                stream.Seek(0, System.IO.SeekOrigin.Begin);
                using (var reader = new BinaryReader(stream))
                {
                    controllerBindingsMetadata.Deserialize(reader);
                }

                var bindings = controllerBindingsMetadata.Bindings;
                Assert.NotEmpty(bindings);
                Assert.Equal(expectedController, bindings.First().Key);
                Assert.Equal(controllerDescription, bindings.First().Value);
            }
        }

        private enum ControllerSelector : byte
        {
            /// <summary>
            /// Selector bits for controller 0.
            /// </summary>
            Controller0 = 0x80,

            /// <summary>
            /// Selector bits for controller 1.
            /// </summary>
            Controller1 = 0x90,

            /// <summary>
            /// Selector bits for controller 2.
            /// </summary>
            Controller2 = 0xA0,

            /// <summary>
            /// Selector bits for controller 3.
            /// </summary>
            Controller3 = 0xB0,

            /// <summary>
            /// Selector bits for any controller.
            /// </summary>
            AnyController = 0xC0
        }

        private enum ControllerInput : byte
        {
            /// <summary>
            /// Selector bits for keypad key 0.
            /// </summary>
            Keypad0 = 0x00,

            /// <summary>
            /// Selector bits for keypad key 1.
            /// </summary>
            Keypad1 = 0x01,

            /// <summary>
            /// Selector bits for keypad key 2.
            /// </summary>
            Keypad2 = 0x02,

            /// <summary>
            /// Selector bits for keypad key 3.
            /// </summary>
            Keypad3 = 0x03,

            /// <summary>
            /// Selector bits for keypad key 4.
            /// </summary>
            Keypad4 = 0x04,

            /// <summary>
            /// Selector bits for keypad key 5.
            /// </summary>
            Keypad5 = 0x05,

            /// <summary>
            /// Selector bits for keypad key 6.
            /// </summary>
            Keypad6 = 0x06,

            /// <summary>
            /// Selector bits for keypad key 7.
            /// </summary>
            Keypad7 = 0x07,

            /// <summary>
            /// Selector bits for keypad key 8.
            /// </summary>
            Keypad8 = 0x08,

            /// <summary>
            /// Selector bits for keypad key 9.
            /// </summary>
            Keypad9 = 0x09,

            /// <summary>
            /// Selector bits for keypad clear key.
            /// </summary>
            KeypadClear = 0x0A,

            /// <summary>
            /// Selector bits for keypad enter key.
            /// </summary>
            KeypadEnter = 0x0B,

            /// <summary>
            /// Selector bits for top action keys.
            /// </summary>
            TopAction = 0x0C,

            /// <summary>
            /// Selector bits for lower left action key.
            /// </summary>
            LowerLeftAction = 0x0D,

            /// <summary>
            /// Selector bits for lower right action key.
            /// </summary>
            LowerRightAction = 0x0E,

            /// <summary>
            /// Selector bits for controller disc.
            /// </summary>
            Disc = 0x0F,
        }

        /// <summary>
        /// These values enumerate at a high level the supported input devices, and are copied from <see cref="RomMetadataControllerBindings"/>.
        /// </summary>
        private enum Controller : byte
        {
            /// <summary>
            /// Not a valid input.
            /// </summary>
            None,

            /// <summary>
            /// Controller 0, e.g. the left controller on the Master Component.
            /// </summary>
            Controller0,

            /// <summary>
            /// Controller 1, e.g. the right controller on the Master Component.
            /// </summary>
            Controller1,

            /// <summary>
            /// Controller 2, e.g. the left controller on the ECS.
            /// </summary>
            Controller2,

            /// <summary>
            /// Controller 3, e.g. the right controller on the ECS.
            /// </summary>
            Controller3,

            /// <summary>
            /// Any controller. Usually, this means either on the Master Component. In some cases, games running
            /// when the ECS is present may also include the two additional controllers connected to the ECS.
            /// </summary>
            AnyController,

            /// <summary>
            /// The keyboard. This usually implies the ECS keyboard, though it could in theory also mean the ECS Synthesizer.
            /// </summary>
            Keyboard
        }
    }
}
