// <copyright file="RomMetadataControllerBindings.cs" company="INTV Funhouse">
// Copyright (c) 2016-2017 All Rights Reserved
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

namespace INTV.Core.Model
{
    /// <summary>
    /// Class for extracting controller binding information 
    /// </summary>
    public class RomMetadataControllerBindings : RomMetadataBlock
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Core.Model.RomMetadataControllerBindings"/> class.
        /// </summary>
        /// <param name="length">Length of the payload, in bytes.</param>
        public RomMetadataControllerBindings(uint length)
            : base(length, RomMetadataIdTag.ControllerBindings)
        {
        }

        #region Properties

        #endregion // Properties

        #region RomMetadataBlock

        /// <inheritdoc/>
        protected override uint DeserializePayload(INTV.Core.Utility.BinaryReader reader)
        {
            var descriptions = new Dictionary<Controller, string>();

            var bytesParsed = 0u;
            var descriptionBuffer = new List<byte>();
            var currentController = Controller.None;
            while (bytesParsed < Length)
            {
                var value = reader.ReadByte();
                ++bytesParsed;
                var input = (InputSource)value;
                var controller = GetController(input);
                if (controller == Controller.None)
                {
                    descriptionBuffer.Add(value);
                }
                else
                {
                    // Finished previous.
                    if (currentController != Controller.None)
                    {
                        // Put the description into the map.
                        descriptions[currentController] = System.Text.Encoding.UTF8.GetString(descriptionBuffer.ToArray(), 0, descriptionBuffer.Count).Trim('\0');
                    }
                    currentController = controller;
                    descriptionBuffer.Clear();
                }
            }
            return Length;
        }

        #endregion // RomMetadataBlock

        private static Controller GetController(InputSource input)
        {
            var controller = Controller.None;
            if ((input >= InputSource.Controller0First) && (input <= InputSource.Controller0Last))
            {
                controller = Controller.Controller0;
            }
            else if ((input >= InputSource.Controller1First) && (input <= InputSource.Controller1Last))
            {
                controller = Controller.Controller1;
            }
            else if ((input >= InputSource.Controller2First) && (input <= InputSource.Controller2Last))
            {
                controller = Controller.Controller2;
            }
            else if ((input >= InputSource.Controller3First) && (input <= InputSource.Controller3Last))
            {
                controller = Controller.Controller3;
            }
            else if ((input >= InputSource.KeyboardFirst) && (input <= InputSource.KeyboardLast))
            {
                controller = Controller.Keyboard;
            }
            return controller;
        }

        /// <summary>
        /// These values enumerate at a high level the supported input devices.
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

        /// <summary>
        /// These values describe controller input sources.
        /// </summary>
        private enum InputSource : byte
        {
            /// <summary>
            /// No input source.
            /// </summary>
            None = 0x00,

            //// NOTE: Values 0x08 - 0x1F are reserved.

            #region Constituent Parts

            #region Controller Selectors

            /// <summary>
            /// Bits to indicate controller 0 inputs.
            /// </summary>
            Controller0Bits = 0x80,

            /// <summary>
            /// Bits to indicate controller 1 inputs.
            /// </summary>
            Controller1Bits = 0x90,

            /// <summary>
            /// Bits to indicate controller 2 inputs.
            /// </summary>
            Controller2Bits = 0xA0,

            /// <summary>
            /// Bits to indicate controller 3 inputs.
            /// </summary>
            Controller3Bits = 0xB0,

            /// <summary>
            /// Bits to indicate 'any' controller inputs.
            /// </summary>
            ControllerAnyBits = 0xC0,

            /// <summary>
            /// The first keyboard input bits.
            /// </summary>
            FirstKeyboardInputBits = 0xD0,

            /// <summary>
            /// The last keyboard input bits.
            /// </summary>
            LastKeyboardInputBits = 0xFF,

            /// <summary>
            /// Mask to apply to get the "controller selector".
            /// </summary>
            /// <remarks>This cannot be used unconditionally. Note that keyboard keys occupy all values
            /// in the range 0xD0 - 0xFF.</remarks>
            ControllerSelectorMask = 0xF0,

            #endregion // Controller Selectors

            #region Input Selectors

            /// <summary>
            /// The bits that are set for keypad digit 0.
            /// </summary>
            Key0Bits = 0x00,

            /// <summary>
            /// The bits that are set for keypad digit 1.
            /// </summary>
            Key1Bits = 0x01,

            /// <summary>
            /// The bits that are set for keypad digit 2.
            /// </summary>
            Key2Bits = 0x02,

            /// <summary>
            /// The bits that are set for keypad digit 3.
            /// </summary>
            Key3Bits = 0x03,

            /// <summary>
            /// The bits that are set for keypad digit 4.
            /// </summary>
            Key4Bits = 0x04,

            /// <summary>
            /// The bits that are set for keypad digit 5.
            /// </summary>
            Key5Bits = 0x05,

            /// <summary>
            /// The bits that are set for keypad digit 6.
            /// </summary>
            Key6Bits = 0x06,

            /// <summary>
            /// The bits that are set for keypad digit 7.
            /// </summary>
            Key7Bits = 0x07,

            /// <summary>
            /// The bits that are set for keypad digit 8.
            /// </summary>
            Key8Bits = 0x08,

            /// <summary>
            /// The bits that are set for keypad digit 9.
            /// </summary>
            Key9Bits = 0x09,

            /// <summary>
            /// The bits that are set for keypad clear key.
            /// </summary>
            KeyClearBits = 0x0A,

            /// <summary>
            /// The bits that are set for keypad enter key.
            /// </summary>
            KeyEnterBits = 0x0B,

            /// <summary>
            /// The bits that are set for top action buttons.
            /// </summary>
            ActionTopBits = 0x0C,

            /// <summary>
            /// The bits that are set for lower left action button.
            /// </summary>
            ActionLowerLeftBits = 0x0D,

            /// <summary>
            /// The bits that are set for lower right action button.
            /// </summary>
            ActionLowerRightBits = 0x0E,

            /// <summary>
            /// The bits that are set for direction disc input.
            /// </summary>
            DiscBits = 0x0F,

            /// <summary>
            /// Mask to apply to get "input selector".
            /// </summary>
            InputSelectorMask = 0x0F,

            #endregion // Input Selectors

            #endregion // Constituent Parts

            #region Controller 0: 0x80 - 0x8F

            /// <summary>
            /// Sentinel value for first input for controller 0.
            /// </summary>
            Controller0First = Controller0Bits,

            /// <summary>
            /// Sentinel value for last input for controller 0.
            /// </summary>
            Controller0Last = Controller0Bits | InputSelectorMask,

            /// <summary>
            /// Controller 0, keypad digit 0.
            /// </summary>
            Controller0Key0 = Controller0Bits | Key0Bits,

            /// <summary>
            /// Controller 0, keypad digit 1.
            /// </summary>
            Controller0Key1 = Controller0Bits | Key1Bits,

            /// <summary>
            /// Controller 0, keypad digit 2.
            /// </summary>
            Controller0Key2 = Controller0Bits | Key2Bits,

            /// <summary>
            /// Controller 0, keypad digit 3.
            /// </summary>
            Controller0Key3 = Controller0Bits | Key3Bits,

            /// <summary>
            /// Controller 0, keypad digit 4.
            /// </summary>
            Controller0Key4 = Controller0Bits | Key4Bits,

            /// <summary>
            /// Controller 0, keypad digit 5.
            /// </summary>
            Controller0Key5 = Controller0Bits | Key5Bits,

            /// <summary>
            /// Controller 0, keypad digit 6.
            /// </summary>
            Controller0Key6 = Controller0Bits | Key6Bits,

            /// <summary>
            /// Controller 0, keypad digit 7.
            /// </summary>
            Controller0Key7 = Controller0Bits | Key7Bits,

            /// <summary>
            /// Controller 0, keypad digit 8.
            /// </summary>
            Controller0Key8 = Controller0Bits | Key8Bits,

            /// <summary>
            /// Controller 0, keypad digit 9.
            /// </summary>
            Controller0Key9 = Controller0Bits | Key9Bits,

            /// <summary>
            /// Controller 0, keypad clear key.
            /// </summary>
            Controller0KeyClear = Controller0Bits | KeyClearBits,

            /// <summary>
            /// Controller 0, keypad enter key.
            /// </summary>
            Controller0KeyEnter = Controller0Bits | KeyEnterBits,

            /// <summary>
            /// Controller 0, top action buttons.
            /// </summary>
            Controller0ActionTop = Controller0Bits | ActionTopBits,

            /// <summary>
            /// Controller 0, lower left action button.
            /// </summary>
            Controller0ActionLowerLeft = Controller0Bits | ActionLowerLeftBits,

            /// <summary>
            /// Controller 0, lower right action button.
            /// </summary>
            Controller0ActionLowerRight = Controller0Bits | ActionLowerRightBits,

            /// <summary>
            /// Controller 0, direction disc input.
            /// </summary>
            Controller0Disc = Controller0Bits | DiscBits,

            #endregion // Controller 0: 0x80 - 0x8F

            #region Controller 1: 0x90 - 0x9F

            /// <summary>
            /// Sentinel value for first input for controller 1.
            /// </summary>
            Controller1First = Controller1Bits,

            /// <summary>
            /// Sentinel value for last input for controller 1.
            /// </summary>
            Controller1Last = Controller1Bits | InputSelectorMask,

            /// <summary>
            /// Controller 1, keypad digit 0.
            /// </summary>
            Controller1Key0 = Controller1Bits | Key0Bits,

            /// <summary>
            /// Controller 1, keypad digit 1.
            /// </summary>
            Controller1Key1 = Controller1Bits | Key1Bits,

            /// <summary>
            /// Controller 1, keypad digit 2.
            /// </summary>
            Controller1Key2 = Controller1Bits | Key2Bits,

            /// <summary>
            /// Controller 1, keypad digit 3.
            /// </summary>
            Controller1Key3 = Controller1Bits | Key3Bits,

            /// <summary>
            /// Controller 1, keypad digit 4.
            /// </summary>
            Controller1Key4 = Controller1Bits | Key4Bits,

            /// <summary>
            /// Controller 1, keypad digit 5.
            /// </summary>
            Controller1Key5 = Controller1Bits | Key5Bits,

            /// <summary>
            /// Controller 1, keypad digit 6.
            /// </summary>
            Controller1Key6 = Controller1Bits | Key6Bits,

            /// <summary>
            /// Controller 1, keypad digit 7.
            /// </summary>
            Controller1Key7 = Controller1Bits | Key7Bits,

            /// <summary>
            /// Controller 1, keypad digit 8.
            /// </summary>
            Controller1Key8 = Controller1Bits | Key8Bits,

            /// <summary>
            /// Controller 1, keypad digit 9.
            /// </summary>
            Controller1Key9 = Controller1Bits | Key9Bits,

            /// <summary>
            /// Controller 1, keypad clear key.
            /// </summary>
            Controller1KeyClear = Controller1Bits | KeyClearBits,

            /// <summary>
            /// Controller 1, keypad enter key.
            /// </summary>
            Controller1KeyEnter = Controller1Bits | KeyEnterBits,

            /// <summary>
            /// Controller 1, top action buttons.
            /// </summary>
            Controller1ActionTop = Controller1Bits | ActionTopBits,

            /// <summary>
            /// Controller 1, lower left action button.
            /// </summary>
            Controller1ActionLowerLeft = Controller1Bits | ActionLowerLeftBits,

            /// <summary>
            /// Controller 1, lower right action button.
            /// </summary>
            Controller1ActionLowerRight = Controller1Bits | ActionLowerRightBits,

            /// <summary>
            /// Controller 1, direction disc input.
            /// </summary>
            Controller1Disc = Controller1Bits | DiscBits,

            #endregion // Controller 1 : 0x90 - 0x9F

            #region Controller 2: 0xA0 - 0xAF

            /// <summary>
            /// Sentinel value for first input for controller 2.
            /// </summary>
            Controller2First = Controller2Bits,

            /// <summary>
            /// Sentinel value for last input for controller 2.
            /// </summary>
            Controller2Last = Controller2Bits | InputSelectorMask,

            /// <summary>
            /// Controller 2, keypad digit 0.
            /// </summary>
            Controller2Key0 = Controller2Bits | Key0Bits,

            /// <summary>
            /// Controller 2, keypad digit 1.
            /// </summary>
            Controller2Key1 = Controller2Bits | Key1Bits,

            /// <summary>
            /// Controller 2, keypad digit 2.
            /// </summary>
            Controller2Key2 = Controller2Bits | Key2Bits,

            /// <summary>
            /// Controller 2, keypad digit 3.
            /// </summary>
            Controller2Key3 = Controller2Bits | Key3Bits,

            /// <summary>
            /// Controller 2, keypad digit 4.
            /// </summary>
            Controller2Key4 = Controller2Bits | Key4Bits,

            /// <summary>
            /// Controller 2, keypad digit 5.
            /// </summary>
            Controller2Key5 = Controller2Bits | Key5Bits,

            /// <summary>
            /// Controller 2, keypad digit 6.
            /// </summary>
            Controller2Key6 = Controller2Bits | Key6Bits,

            /// <summary>
            /// Controller 2, keypad digit 7.
            /// </summary>
            Controller2Key7 = Controller2Bits | Key7Bits,

            /// <summary>
            /// Controller 2, keypad digit 8.
            /// </summary>
            Controller2Key8 = Controller2Bits | Key8Bits,

            /// <summary>
            /// Controller 2, keypad digit 9.
            /// </summary>
            Controller2Key9 = Controller2Bits | Key9Bits,

            /// <summary>
            /// Controller 2, keypad clear key.
            /// </summary>
            Controller2KeyClear = Controller2Bits | KeyClearBits,

            /// <summary>
            /// Controller 2, keypad enter key.
            /// </summary>
            Controller2KeyEnter = Controller2Bits | KeyEnterBits,

            /// <summary>
            /// Controller 2, top action buttons.
            /// </summary>
            Controller2ActionTop = Controller2Bits | ActionTopBits,

            /// <summary>
            /// Controller 2, lower left action button.
            /// </summary>
            Controller2ActionLowerLeft = Controller2Bits | ActionLowerLeftBits,

            /// <summary>
            /// Controller 2, lower right action button.
            /// </summary>
            Controller2ActionLowerRight = Controller2Bits | ActionLowerRightBits,

            /// <summary>
            /// Controller 2, direction disc input.
            /// </summary>
            Controller2Disc = Controller2Bits | DiscBits,

            #endregion // Controller 2: 0xA0 - 0xAF

            #region Controller 3: 0xB0 - 0xBF

            /// <summary>
            /// Sentinel value for first input for controller 3.
            /// </summary>
            Controller3First = Controller3Bits,

            /// <summary>
            /// Sentinel value for last input for controller 3.
            /// </summary>
            Controller3Last = Controller3Bits | InputSelectorMask,

            /// <summary>
            /// Controller 3, keypad digit 0.
            /// </summary>
            Controller3Key0 = Controller3Bits | Key0Bits,

            /// <summary>
            /// Controller 3, keypad digit 1.
            /// </summary>
            Controller3Key1 = Controller3Bits | Key1Bits,

            /// <summary>
            /// Controller 3, keypad digit 2.
            /// </summary>
            Controller3Key2 = Controller3Bits | Key2Bits,

            /// <summary>
            /// Controller 3, keypad digit 3.
            /// </summary>
            Controller3Key3 = Controller3Bits | Key3Bits,

            /// <summary>
            /// Controller 3, keypad digit 4.
            /// </summary>
            Controller3Key4 = Controller3Bits | Key4Bits,

            /// <summary>
            /// Controller 3, keypad digit 5.
            /// </summary>
            Controller3Key5 = Controller3Bits | Key5Bits,

            /// <summary>
            /// Controller 3, keypad digit 6.
            /// </summary>
            Controller3Key6 = Controller3Bits | Key6Bits,

            /// <summary>
            /// Controller 3, keypad digit 7.
            /// </summary>
            Controller3Key7 = Controller3Bits | Key7Bits,

            /// <summary>
            /// Controller 3, keypad digit 8.
            /// </summary>
            Controller3Key8 = Controller3Bits | Key8Bits,

            /// <summary>
            /// Controller 3, keypad digit 9.
            /// </summary>
            Controller3Key9 = Controller3Bits | Key9Bits,

            /// <summary>
            /// Controller 3, keypad clear key.
            /// </summary>
            Controller3KeyClear = Controller3Bits | KeyClearBits,

            /// <summary>
            /// Controller 3, keypad enter key.
            /// </summary>
            Controller3KeyEnter = Controller3Bits | KeyEnterBits,

            /// <summary>
            /// Controller 3, top action buttons.
            /// </summary>
            Controller3ActionTop = Controller3Bits | ActionTopBits,

            /// <summary>
            /// Controller 3, lower left action button.
            /// </summary>
            Controller3ActionLowerLeft = Controller3Bits | ActionLowerLeftBits,

            /// <summary>
            /// Controller 3, lower right action button.
            /// </summary>
            Controller3ActionLowerRight = Controller3Bits | ActionLowerRightBits,

            /// <summary>
            /// Controller 3, direction disc input.
            /// </summary>
            Controller3Disc = Controller3Bits | DiscBits,

            #endregion // Controller 3: 0xB0 - 0xBF

            #region Any Controller (Controller 0, 1, 2, or 3): 0xC0 - 0xCF

            /// <summary>
            /// Sentinel value for first input for any controller.
            /// </summary>
            ControllerAnyFirst = ControllerAnyBits,

            /// <summary>
            /// Sentinel value for last input for any controller.
            /// </summary>
            ControllerAnyLast = ControllerAnyBits | InputSelectorMask,

            /// <summary>
            /// Any controller, keypad digit 0.
            /// </summary>
            ControllerAnyKey0 = ControllerAnyBits | Key0Bits,

            /// <summary>
            /// Any controller, keypad digit 1.
            /// </summary>
            ControllerAnyKey1 = ControllerAnyBits | Key1Bits,

            /// <summary>
            /// Any controller, keypad digit 2.
            /// </summary>
            ControllerAnyKey2 = ControllerAnyBits | Key2Bits,

            /// <summary>
            /// Any controller, keypad digit 3.
            /// </summary>
            ControllerAnyKey3 = ControllerAnyBits | Key3Bits,

            /// <summary>
            /// Any controller, keypad digit 4.
            /// </summary>
            ControllerAnyKey4 = ControllerAnyBits | Key4Bits,

            /// <summary>
            /// Any controller, keypad digit 5.
            /// </summary>
            ControllerAnyKey5 = ControllerAnyBits | Key5Bits,

            /// <summary>
            /// Any controller, keypad digit 6.
            /// </summary>
            ControllerAnyKey6 = ControllerAnyBits | Key6Bits,

            /// <summary>
            /// Any controller, keypad digit 7.
            /// </summary>
            ControllerAnyKey7 = ControllerAnyBits | Key7Bits,

            /// <summary>
            /// Any controller, keypad digit 8.
            /// </summary>
            ControllerAnyKey8 = ControllerAnyBits | Key8Bits,

            /// <summary>
            /// Any controller, keypad digit 9.
            /// </summary>
            ControllerAnyKey9 = ControllerAnyBits | Key9Bits,

            /// <summary>
            /// Any controller, keypad clear key.
            /// </summary>
            ControllerAnyKeyClear = ControllerAnyBits | KeyClearBits,

            /// <summary>
            /// Any controller, keypad enter key.
            /// </summary>
            ControllerAnyKeyEnter = ControllerAnyBits | KeyEnterBits,

            /// <summary>
            /// Any controller, top action buttons.
            /// </summary>
            ControllerAnyActionTop = ControllerAnyBits | ActionTopBits,

            /// <summary>
            /// Any controller, lower left action button.
            /// </summary>
            ControllerAnyActionLowerLeft = ControllerAnyBits | ActionLowerLeftBits,

            /// <summary>
            /// Any controller, lower right action button.
            /// </summary>
            ControllerAnyActionLowerRight = ControllerAnyBits | ActionLowerRightBits,

            /// <summary>
            /// Any controller, direction disc input.
            /// </summary>
            ControllerAnyDisc = ControllerAnyBits | DiscBits,

            #endregion // Any Controller (Controller 0, 1, 2, or 3): 0xC0 - 0xCF

            #region ECS Keyboard: 0xD0 - 0xFF

            /// <summary>
            /// Sentinel value for first input for ECS keyboard.
            /// </summary>
            KeyboardFirst = FirstKeyboardInputBits,

            /// <summary>
            /// Sentinel value for last input for ECS keyboard.
            /// </summary>
            KeyboardLast = FirstKeyboardInputBits | LastKeyboardInputBits,

            #endregion // ECS Keyboard: 0xD0 - 0xFF
        }
    }
}
