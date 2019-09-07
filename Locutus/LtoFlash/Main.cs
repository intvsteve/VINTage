// <copyright file="Main.cs" company="INTV Funhouse">
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

namespace Locutus.View
{
    /// <summary>
    /// This type just wraps the traditional main() method for the Mono application.
    /// </summary>
    public class MainClass
    {
        /// <summary>
        /// The entry point of the program, where the program control starts and ends.
        /// </summary>
        /// <param name="args">The command-line arguments.</param>
        public static void Main(string[] args)
        {
            var dummySettings = new DummySettings();
            dummySettings.JzIntvHack = INTV.JzIntv.Model.DisplayMode.Fullscreen;
            dummySettings.JzIntvUIHack = INTV.JzIntvUI.ViewModel.JzIntvSettingsPageViewModel.EmulatorPathLabel;
            dummySettings.LtoFlashHack = INTV.LtoFlash.Model.EcsStatusFlags.AllFlags;
            dummySettings.IntellicartHack = INTV.Intellicart.ViewModel.SettingsPageViewModel.SerialPortGroupName;
            var applicationInfo = new LtoFlashApplicationInfo() { DummySettings = dummySettings };
            INTV.Shared.Utility.SingleInstanceApplication.RunApplication<Locutus.View.MainWindow>("Locutus-{4B53C351-EE55-46AB-8DE9-C2E4DDD1297A}", applicationInfo, args, "Resources/Images/LTOFlashSplash.png");
        }

        /// <summary>
        /// Stupid hacky way to suppress a warning and get MEF to "work" on Mac. There must be
        /// something not quite working, since merely adding one usage of a given assembly will
        /// get MEF working properly. Possibly some problem w/ the catalog? Works fine in Windows, though.
        /// -- UPDATE --
        /// The problem does not appear to be MEF... The problem seems to be with UI-related resources. I.e. the
        /// build won't put resources into the application bundle unless there's a direct reference to the assembly
        /// containing the InterfaceDefinition. This *may* be addressable by having a run-time injection of the
        /// necessary visual resources into the application bundle in CompositionHelpers - which has been experimentally
        /// verified - but just having a strong reference is OK for now.
        /// </summary>
        private class DummySettings : System.Configuration.ApplicationSettingsBase
        {
            /// <summary>
            /// Gets or sets the jzIntv hack.
            /// </summary>
            internal INTV.JzIntv.Model.DisplayMode JzIntvHack
            {
                get { return (INTV.JzIntv.Model.DisplayMode)this["JzIntvHack"]; }
                set { this["JzIntvHack"] = value; }
            }

            /// <summary>
            /// Gets or sets the jzIntvUI hack.
            /// </summary>
            internal string JzIntvUIHack
            {
                get { return (string)this["JzIntvUIHack"]; }
                set { this["JzIntvUIHack"] = value; }
            }

            /// <summary>
            /// Gets or sets the LTO Flash hack.
            /// </summary>
            internal INTV.LtoFlash.Model.EcsStatusFlags LtoFlashHack
            {
                get { return (INTV.LtoFlash.Model.EcsStatusFlags)this["LtoFlashHack"]; }
                set { this["LtoFlashHack"] = value; }
            }

            /// <summary>
            /// Gets or sets the Intellicart hack.
            /// </summary>
            internal string IntellicartHack
            {
                get { return (string)this["IntellicartHack"]; }
                set { this["IntellicartHack"] = value; }
            }

            /// <inheritdoc />
            public override object this[string index]
            {
                get
                {
                    return null; // base[index];
                }
                set
                {
                    // base[index] = value;
                }
            }
        }
    }
}
