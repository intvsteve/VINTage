// <copyright file="CommandLineArgument.cs" company="INTV Funhouse">
// Copyright (c) 2012-2016 All Rights Reserved
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
using INTV.Core.Model.Program;

namespace INTV.JzIntv.Model
{
    public enum CommandLineArgument
    {
        /// <summary>Command line arguments are fully specified by the string associated with this pseudo-argument.</summary>
        Custom,

        /// <summary>Command line argument to the EXEC ROM.</summary>
        ExecPath,

        /// <summary>Command line argument to the GROM ROM.</summary>
        GromPath,

        /// <summary>Command line argument to the ECS ROM.</summary>
        EcsPath,

        /// <summary>Command line argument indicating whether to enable ECS emulation.</summary>
        EnableEcs,

        /// <summary>Command line argument indicating whether to enable Intellivoice emulation.</summary>
        EnableIntellivoice,

        /// <summary>Command line argument for voice data filtering window.</summary>
        VoiceFilterWindow,

        /// <summary>Command line argument for file to record voice data to.</summary>
        VoiceRecordFileName,

        /// <summary>Command line argument indicating emulator display size.</summary>
        DisplaySize,

        /// <summary>Command line argument indicating whether to run emulator display windowed or full screen.</summary>
        Fullscreen,

        /// <summary>Command line argument for setting prescale behavior.</summary>
        Prescale,

        /// <summary>Command line argument to use a custom color palette.</summary>
        AlternateColorPalette,

        /// <summary>Command line argument for setting audio data sample rate.</summary>
        AudioRate,

        /// <summary>Command line argument for file to record audio data to.</summary>
        AudioFileName,

        /// <summary>Command line argument for audio data filter window.</summary>
        AudioFilterWindow,

        /// <summary>Command line argument for audio data buffer size.</summary>
        AudioBufferSize,

        /// <summary>Command line argument for size of audio buffer.</summary>
        AudioBufferCount,

        /// <summary>Command line argument for minimum hardware tick count for audio.</summary>
        AudioMinimumHardwareTickCount,

        /// <summary>Command line argument for AVI sample rate (ms).</summary>
        AviRate,

        /// <summary>Command line argument for path to keyboard hackfile.</summary>
        KeyboardHackFile,

        /// <summary>Command line argument for initial keyboard mapping to use.</summary>
        KeyboardMap,

        /// <summary>Command line argument describing joystick configuration for joystick 0.</summary>
        Joystick0Configuration,

        /// <summary>Command line argument describing joystick configuration for joystick 1.</summary>
        Joystick1Configuration,

        /// <summary>Command line argument describing joystick configuration for joystick 2.</summary>
        Joystick2Configuration,

        /// <summary>Command line argument describing joystick configuration for joystick 3.</summary>
        Joystick3Configuration,

        /// <summary>Command line argument describing LPT port for INTV2PC for Master Component.</summary>
        Intv2PCMaster,

        /// <summary>Command line argument describing LPT port for INTV2PC for ECS.</summary>
        Intv2PCEcs,

        /// <summary>Command line argument for Classic Game Controller configuration for Master Component.</summary>
        ClassicGameControllerMaster,

        /// <summary>Command line argument for Classic Game Controller configuration forECS.</summary>
        ClassicGameControllerEcs,

        /// <summary>Command line argument to set cache flags for Intellicart emulation.</summary>
        IntellicartRomEmulationFlags,

        /// <summary>Command line argument to enable debugger.</summary>
        EnableDebugger,

        /// <summary>Command line argument to provide symbols to debugger.</summary>
        DebuggerSymbolFilePath,

        /// <summary>Command line argument to provide source listing to debugger.</summary>
        DebuggerSourceListingMapPath,

        /// <summary>Command line argument to provide a script to the debugger.</summary>
        DebuggerScriptPath,

        /// <summary>Command line argument to randomize memory when starting emulator.</summary>
        DebuggerRandomizeMemoryAtStartup,

        /// <summary>The rate control setting (a.k.a. Macho mode).</summary>
        RateControl,

        /// <summary>Command line argument to provide additional locations to search for ROMs.</summary>
        RomSearchPath,

        /// <summary>Command line argument for quiet mode, which suppresses stdout.</summary>
        QuietMode,

        /// <summary>Command line argument to enable GUI mode, which supports some commands to be provided via stdin.</summary>
        GuiMode,

        /// <summary>Command line argument to enable JLP emulation.</summary>
        Jlp,

        /// <summary>Command line argument to provide path to JLP flash data file, which also enables JLP emulation. Deprecated -- use Jlp and JlpFlash arguments instead.</summary>
        JlpSaveGamePath,

        /// <summary>Command line argument to provide path to JLP flash data file.</summary>
        JlpFlash,

        /// <summary>Command line argument to enable LUIGI format support.</summary>
        Locutus,

        /// <summary>Command line argument for enabling ECS link support. (?)</summary>
        FileIO,

        /// <summary>Command line argument for adjusting how 'busy wait' behaves.</summary>
        NoBusyWait,

        /// <summary>Command line argument for a demo file.</summary>
        DemoFile,

        /// <summary>Command line argument to enable mouse support in the emulator.</summary>
        EnableMouse,

        /// <summary>Command line argument to emulate behavior on a PAL system.</summary>
        PalEmulation,

        /// <summary>Command line argument to delay launching the emulation.</summary>
        StartDelay,

        /// <summary>Command line argument to provide custom supplemental arguments to the emulator.</summary>
        AdditionalArguments
    }

    /// <summary>
    /// Helper methods for the CommandLineArgument type.
    /// </summary>
    public static class CommandLineArgumentHelpers
    {
        private static readonly Dictionary<CommandLineArgument, Tuple<string, Type>> ShortArgumentStrings = new Dictionary<CommandLineArgument, Tuple<string, Type>>()
        {
            { CommandLineArgument.Custom, new Tuple<string, Type>("{0}", typeof(string)) },
            { CommandLineArgument.ExecPath, new Tuple<string, Type>("-e \"{0}\"", typeof(string)) },
            { CommandLineArgument.GromPath, new Tuple<string, Type>("-g \"{0}\"", typeof(string)) },
            { CommandLineArgument.EcsPath, new Tuple<string, Type>("-E \"{0}\"", typeof(string)) },
            { CommandLineArgument.EnableEcs, new Tuple<string, Type>("-s{0}", typeof(bool)) },
            { CommandLineArgument.EnableIntellivoice, new Tuple<string, Type>("-v{0}", typeof(bool)) },
            { CommandLineArgument.VoiceFilterWindow, new Tuple<string, Type>("-W{0}", typeof(int)) }, // -1 is default, otherwise seems to be not checked
            { CommandLineArgument.VoiceRecordFileName, new Tuple<string, Type>("-V{0}", typeof(string)) }, // NOTE! This must not contain spaces! (??)
            { CommandLineArgument.DisplaySize, new Tuple<string, Type>("-z{0}", typeof(DisplayResolution)) }, // TODO The emulator also supports custom resolutions here! This could be a string that is properly formatted.
            { CommandLineArgument.Fullscreen, new Tuple<string, Type>("-f{0}", typeof(DisplayMode)) },
            { CommandLineArgument.Prescale, new Tuple<string, Type>("--prescale={0}", typeof(PrescaleMode)) }, // NOTE: This could be determined by invoking jzIntvy w/ --prescale=-1 and parsing results
            { CommandLineArgument.AlternateColorPalette, new Tuple<string, Type>("--gfx-palette=\"{0}\"", typeof(string)) },
            { CommandLineArgument.AudioRate, new Tuple<string, Type>("-a{0}", typeof(SampleRate)) }, // Requires 4000 <= rate <= 96000
            { CommandLineArgument.AudioFileName, new Tuple<string, Type>("-F{0}", typeof(string)) }, // NOTE! This must not contain spaces!
            { CommandLineArgument.AudioFilterWindow, new Tuple<string, Type>("-w{0}", typeof(int)) }, // NOTE: Should be >= 1; -1 is default
            { CommandLineArgument.AudioBufferSize, new Tuple<string, Type>("-B{0}", typeof(int)) }, // TODO Check ranges?
            { CommandLineArgument.AudioBufferCount, new Tuple<string, Type>("-C{0}", typeof(int)) }, // TODO Check ranges?
            { CommandLineArgument.AudioMinimumHardwareTickCount, new Tuple<string, Type>("-M{0}", typeof(int)) }, // NOTE: This value is apparently not validated
            { CommandLineArgument.AviRate, new Tuple<string, Type>("--avirate={0:0.00}", typeof(double)) }, // TODO Check ranges?
            { CommandLineArgument.KeyboardHackFile, new Tuple<string, Type>("--kbdhackfile={0}", typeof(string)) }, // NOTE! This path MUST NOT be quoted, and cannot contain spaces!
            { CommandLineArgument.KeyboardMap, new Tuple<string, Type>("-m{0}", typeof(KeyboardMap)) },
            { CommandLineArgument.Joystick0Configuration, new Tuple<string, Type>("--js0=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick1Configuration, new Tuple<string, Type>("--js1=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick2Configuration, new Tuple<string, Type>("--js2=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick3Configuration, new Tuple<string, Type>("--js3=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Intv2PCMaster, new Tuple<string, Type>("-i{0}", typeof(Intv2PcPort)) },
            { CommandLineArgument.Intv2PCEcs, new Tuple<string, Type>("-I{0}", typeof(Intv2PcPort)) },
            { CommandLineArgument.ClassicGameControllerMaster, new Tuple<string, Type>("--cgc0={0}", typeof(string)) }, // TODO Determine what is valid
            { CommandLineArgument.ClassicGameControllerEcs, new Tuple<string, Type>("--cgc1={0}", typeof(string)) }, // TODO Determine what is valid
            { CommandLineArgument.IntellicartRomEmulationFlags, new Tuple<string, Type>("-c{0}", typeof(IntellicartCacheOption)) },
            { CommandLineArgument.EnableDebugger, new Tuple<string, Type>("-d", null) },
            { CommandLineArgument.DebuggerSymbolFilePath, new Tuple<string, Type>("-S\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerSourceListingMapPath, new Tuple<string, Type>("--src-map=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerScriptPath, new Tuple<string, Type>("--script=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerRandomizeMemoryAtStartup, new Tuple<string, Type>("--rand-mem", null) },
            { CommandLineArgument.RateControl, new Tuple<string, Type>("-r{0:0.00}", typeof(double)) },
            { CommandLineArgument.RomSearchPath, new Tuple<string, Type>("-p \"{0}\"", typeof(string)) },
            { CommandLineArgument.QuietMode, new Tuple<string, Type>("-q", null) },
            { CommandLineArgument.GuiMode, new Tuple<string, Type>("--gui-mode", null) },
            { CommandLineArgument.Jlp, new Tuple<string, Type>("-J{0}", typeof(JlpFeatures)) }, // NOTE: Only "compatibilty" flags used
            { CommandLineArgument.JlpSaveGamePath, new Tuple<string, Type>("--jlp-savegame=\"{0}\"", typeof(string)) }, // NOTE Also sets JLP=3
            { CommandLineArgument.JlpFlash, new Tuple<string, Type>("-j\"{0}\"", typeof(string)) },
            { CommandLineArgument.Locutus, new Tuple<string, Type>("--locutus", null) },
            { CommandLineArgument.FileIO, new Tuple<string, Type>("--file-io=\"{0}\"", typeof(string)) }, // TODO Check if this is correct
            { CommandLineArgument.NoBusyWait, new Tuple<string, Type>("-9", null) },
            { CommandLineArgument.DemoFile, new Tuple<string, Type>("-D\"{0}\"", typeof(string)) }, // TODO Check if this is correct
            { CommandLineArgument.EnableMouse, new Tuple<string, Type>("--enable-mouse", null) },
            { CommandLineArgument.PalEmulation, new Tuple<string, Type>("-P", null) },
            { CommandLineArgument.StartDelay, new Tuple<string, Type>("--start-delay={0:0.000}", typeof(float)) }, // TODO Check -- start delay in milliseconds?
            { CommandLineArgument.AdditionalArguments, new Tuple<string, Type>("{0}", typeof(string)) },
        };

        private static readonly Dictionary<CommandLineArgument, Tuple<string, Type>> LongArgumentStrings = new Dictionary<CommandLineArgument, Tuple<string, Type>>()
        {
            { CommandLineArgument.Custom, new Tuple<string, Type>("{0}", typeof(string)) },
            { CommandLineArgument.ExecPath, new Tuple<string, Type>("--execimg=\"{0}\"", typeof(string)) },
            { CommandLineArgument.GromPath, new Tuple<string, Type>("--gromimg=\"{0}\"", typeof(string)) },
            { CommandLineArgument.EcsPath, new Tuple<string, Type>("--ecsimg=\"{0}\"", typeof(string)) },
            { CommandLineArgument.EnableEcs, new Tuple<string, Type>("--ecs={0}", typeof(bool)) },
            { CommandLineArgument.EnableIntellivoice, new Tuple<string, Type>("--voice={0}", typeof(bool)) },
            { CommandLineArgument.VoiceFilterWindow, new Tuple<string, Type>("--voicewindow={0}", typeof(int)) }, // -1 is default, otherwise seems to be not checked
            { CommandLineArgument.VoiceRecordFileName, new Tuple<string, Type>("--voicefiles=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DisplaySize, new Tuple<string, Type>("--displaysize={0}", typeof(DisplayResolution)) }, // TODO The emulator also supports custom resolutions here! This could be a string that is properly formatted...
            { CommandLineArgument.Fullscreen, new Tuple<string, Type>("--fullscreen={0}", typeof(DisplayMode)) },
            { CommandLineArgument.Prescale, new Tuple<string, Type>("--prescale={0}", typeof(PrescaleMode)) }, // NOTE: This could be determined by invoking jzIntvy w/ --prescale=-1 and parsing results
            { CommandLineArgument.AlternateColorPalette, new Tuple<string, Type>("--gfx-palette=\"{0}\"", typeof(string)) },
            { CommandLineArgument.AudioRate, new Tuple<string, Type>("--audiorate={0}", typeof(SampleRate)) }, // Requires 4000 <= rate <= 96000
            { CommandLineArgument.AudioFileName, new Tuple<string, Type>("--audiofile=\"{0}\"", typeof(string)) },
            { CommandLineArgument.AudioFilterWindow, new Tuple<string, Type>("--audiowindow={0}", typeof(int)) }, // NOTE: Should be >= 1; -1 is default
            { CommandLineArgument.AudioBufferSize, new Tuple<string, Type>("--audiobufsize={0}", typeof(int)) },
            { CommandLineArgument.AudioBufferCount, new Tuple<string, Type>("--audiobufcnt={0}", typeof(int)) },
            { CommandLineArgument.AudioMinimumHardwareTickCount, new Tuple<string, Type>("--audiomintick={0}", typeof(int)) },
            { CommandLineArgument.AviRate, new Tuple<string, Type>("--avirate={0:0.00}", typeof(double)) },
            { CommandLineArgument.KeyboardHackFile, new Tuple<string, Type>("--kbdhackfile={0}", typeof(string)) }, // NOTE! This path MUST NOT be quoted, and cannot contain spaces!
            { CommandLineArgument.KeyboardMap, new Tuple<string, Type>("--kbdmap={0}", typeof(KeyboardMap)) },
            { CommandLineArgument.Joystick0Configuration, new Tuple<string, Type>("--js0=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick1Configuration, new Tuple<string, Type>("--js1=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick2Configuration, new Tuple<string, Type>("--js2=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Joystick3Configuration, new Tuple<string, Type>("--js3=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Intv2PCMaster, new Tuple<string, Type>("--intv2pc0={0}", typeof(Intv2PcPort)) },
            { CommandLineArgument.Intv2PCEcs, new Tuple<string, Type>("--intv2pc1={0}", typeof(Intv2PcPort)) },
            { CommandLineArgument.ClassicGameControllerMaster, new Tuple<string, Type>("--cgc0={0}", typeof(string)) },
            { CommandLineArgument.ClassicGameControllerEcs, new Tuple<string, Type>("--cgc1={0}", typeof(string)) },
            { CommandLineArgument.IntellicartRomEmulationFlags, new Tuple<string, Type>("--icartcache={0}", typeof(IntellicartCacheOption)) },
            { CommandLineArgument.EnableDebugger, new Tuple<string, Type>("--debugger", null) },
            { CommandLineArgument.DebuggerSymbolFilePath, new Tuple<string, Type>("--sym-file=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerSourceListingMapPath, new Tuple<string, Type>("--src-map=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerScriptPath, new Tuple<string, Type>("--script=\"{0}\"", typeof(string)) },
            { CommandLineArgument.DebuggerRandomizeMemoryAtStartup, new Tuple<string, Type>("--rand-mem", null) },
            { CommandLineArgument.RateControl, new Tuple<string, Type>("--ratecontrol={0:0.00}", typeof(double)) },
            { CommandLineArgument.RomSearchPath, new Tuple<string, Type>("--rom-path=\"{0}\"", typeof(string)) },
            { CommandLineArgument.QuietMode, new Tuple<string, Type>("--quiet", null) },
            { CommandLineArgument.GuiMode, new Tuple<string, Type>("--gui-mode", null) },
            { CommandLineArgument.Jlp, new Tuple<string, Type>("--jlp={0}", typeof(JlpFeatures)) },
            { CommandLineArgument.JlpSaveGamePath, new Tuple<string, Type>("--jlp-savegame=\"{0}\"", typeof(string)) },
            { CommandLineArgument.JlpFlash, new Tuple<string, Type>("--jlp-flash=\"{0}\"", typeof(string)) },
            { CommandLineArgument.Locutus, new Tuple<string, Type>("--locutus", null) },
            { CommandLineArgument.FileIO, new Tuple<string, Type>("--file-io==\"{0}\"", typeof(string)) },
            { CommandLineArgument.NoBusyWait, new Tuple<string, Type>("--nobusywait", null) },
            { CommandLineArgument.DemoFile, new Tuple<string, Type>("--demofile==\"{0}\"", typeof(string)) },
            { CommandLineArgument.EnableMouse, new Tuple<string, Type>("--enable-mouse", null) },
            { CommandLineArgument.PalEmulation, new Tuple<string, Type>("--pal", null) },
            { CommandLineArgument.StartDelay, new Tuple<string, Type>("--start-delay={0:0.000}", typeof(float)) }, // TODO Check -- start delay in milliseconds?
            { CommandLineArgument.AdditionalArguments, new Tuple<string, Type>("{0}", typeof(string)) },
        };

        /// <summary>
        /// Builds the given command line arguments into a string to supply to a command line.
        /// </summary>
        /// <param name="arguments">A dictionary of arguments and their values.</param>
        /// <param name="romFilePath">The ROM to run in jzIntv.</param>
        /// <param name="useLongFormat">If <c>true</c>, use the long-form of command line arguments; otherwise use the short form, if available.</param>
        /// <returns>The command line arguments represented as a string suitable for launching the emulator process.</returns>
        public static string BuildCommandLineArguments(this IDictionary<CommandLineArgument, object> arguments, string romFilePath, bool useLongFormat)
        {
            var commandLineBuilder = new System.Text.StringBuilder();

            foreach (var argument in arguments)
            {
                var argumentFormatData = useLongFormat ? LongArgumentStrings : ShortArgumentStrings;
                var argumentFormat = argumentFormatData[argument.Key];
                if (argumentFormat.Item2 != null)
                {
                    var value = argument.Value;
                    if ((value is Enum) || (value is bool))
                    {
                        value = Convert.ToInt32(value); // Unless overridden below, just pass integer values for enum types or bools.
                    }

                    switch (argument.Key)
                    {
                        case CommandLineArgument.DisplaySize:
                            if (argument.Value is DisplayResolution)
                            {
                                var resolution = (DisplayResolution)argument.Value;
                                value = useLongFormat ? resolution.ToLongCommandLineArgumentString() : resolution.ToShortCommandLineArgumentString();
                            }
                            break;
                        default:
                            break;
                    }
                    commandLineBuilder.Append(" ").AppendFormat(argumentFormat.Item1, value);
                }
                else
                {
                    commandLineBuilder.Append(" ").Append(argumentFormat.Item1);
                }
            }

            if (!string.IsNullOrEmpty(romFilePath))
            {
                commandLineBuilder.AppendFormat(" \"{0}\"", romFilePath);
            }

            return commandLineBuilder.ToString();
        }

        /// <summary>
        /// Checks the command line argument string static data structures to ensure nothing has gone missing.
        /// </summary>
        [System.Diagnostics.Conditional("DEBUG")]
        public static void CheckCommandLineArgumentStrings()
        {
            var commandLineArguments = Enum.GetValues(typeof(CommandLineArgument)).Cast<CommandLineArgument>();

            // Verify short version of command line arguments.
            foreach (var commandLineArgument in commandLineArguments)
            {
                var commandLineArgumentString = ShortArgumentStrings[commandLineArgument].Item1;
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(commandLineArgumentString), "Bad short command line argument string for: " + commandLineArgument);
                var commandLineArgumentType = ShortArgumentStrings[commandLineArgument].Item2;
                if (commandLineArgumentType != null)
                {
                    System.Diagnostics.Debug.Assert(commandLineArgumentString.Contains("{0"), "Bad format string in short command line argument data for argument: " + commandLineArgument);
                }
            }

            // Verify long version of command line arguments.
            foreach (var commandLineArgument in commandLineArguments)
            {
                var commandLineArgumentString = LongArgumentStrings[commandLineArgument].Item1;
                System.Diagnostics.Debug.Assert(!string.IsNullOrWhiteSpace(commandLineArgumentString), "Bad long command line argument string for: " + commandLineArgument);
                var commandLineArgumentType = ShortArgumentStrings[commandLineArgument].Item2;
                if ((commandLineArgumentType != null) && (commandLineArgument != CommandLineArgument.Custom) && (commandLineArgument != CommandLineArgument.AdditionalArguments))
                {
                    System.Diagnostics.Debug.Assert(commandLineArgumentString.Contains("={0") || commandLineArgumentString.Contains("=\"{0"), "Bad format string in long command line argument data for argument: " + commandLineArgument);
                }
            }
        }
    }
}
