// <copyright file="Emulator.cs" company="INTV Funhouse">
// Copyright (c) 2014-2019 All Rights Reserved
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

////#define MAC_PERF_TEST

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using INTV.Core.ComponentModel;
using INTV.Core.Model.Program;

#if MAC_PERF_TEST
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
#endif // MAC_PERF_TEST

namespace INTV.JzIntv.Model
{
    /// <summary>
    /// Simple model for the jzIntv emulator. Barely more than a wrapper around the Process class.
    /// </summary>
    public class Emulator : ModelBase
    {
        private const int MaxBufferSize = 256;

        private static ConcurrentDictionary<Emulator, ProgramDescription> _launchedInstances = new ConcurrentDictionary<Emulator, ProgramDescription>();

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.JzIntv.Model.Emulator"/> class.
        /// </summary>
        /// <param name="path">Absolute path to the emulator.</param>
        /// <param name="errorReporter">Error reporter delegate. Note that this may be called from any thread.</param>
        public Emulator(string path, Action<Emulator, string, int, Exception> errorReporter)
        {
            Path = path;
            ErrorReporter = errorReporter;
            StdoutBuffer = new FixedSizedConcurrentQueue(MaxBufferSize);
            StderrBuffer = new FixedSizedConcurrentQueue(MaxBufferSize);
        }

        #endregion // Constructors

        #region Properties

        /// <summary>
        /// Gets the instances of this class launched via the user in the application.
        /// </summary>
        public static IEnumerable<Emulator> LaunchedInstances
        {
            get { return _launchedInstances.Keys; }
        }

        /// <summary>
        /// Gets or sets the absolute path to the emulator.
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// Gets the ROM launched in the emulator.
        /// </summary>
        public ProgramDescription Rom { get; private set; }

        private Action<Emulator, string, int, Exception> ErrorReporter { get; set; }

        private Process Process { get; set; }

        private FixedSizedConcurrentQueue StdoutBuffer { get; set; }

        private FixedSizedConcurrentQueue StderrBuffer { get; set; }

        #endregion // Properties

        /// <summary>
        /// Gets all running instances of the emulator. Assumes the emulator's executable name starts with 'jzintv'.
        /// </summary>
        /// <returns>The running instances of jzIntv.</returns>
        public static IEnumerable<Emulator> Instances()
        {
            // NOTE: GetProcessesByName seems *really* slow in macOS 10.12. Slower than 10.8.5 at least.
            // Also, on Mac OS X 10.8.5, this has been found to throw InvalidOperationException on a dev
            // machine on occasion, so let's just catch that exception, shall we?
            var emulators = Enumerable.Empty<Emulator>();
            try
            {
                var instances = System.Diagnostics.Process.GetProcessesByName("jzintv");
                emulators = instances.Select(p => new Emulator(p.StartInfo.FileName, null) { Process = p });
            }
            catch (InvalidOperationException)
            {
            }
            return emulators;
        }

#if MAC_PERF_TEST
        private static void FindRunningInstancesOnMac()
        {
            // This block runs much faster on Mac... HOWEVER, it does not give a full-fledged Process object, which is what we use in the Instances method above.
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            var inst = new List<string>();
            var launchedApps = NSWorkspace.SharedWorkspace.LaunchedApplications;
            foreach (var launchedApp in launchedApps)
            {
                var instanceAlreadyRunning = string.Empty;
                NSObject launchedAppValue;
                //                   if (launchedApp.TryGetValue((Foundation.NSString)"NSApplicationBundleIdentifier", out launchedAppValue))
                {
                    // If bundle identifiers match, then check to see if it's running from another location.
                    var mainBundle = NSBundle.MainBundle;
                    if (/*(mainBundle.BundleIdentifier == (NSString)launchedAppValue) &&*/ launchedApp.TryGetValue((NSString)"NSApplicationPath", out launchedAppValue))
                    {
                        // Found an instance -- check to see if it's an instance from another location.
                        // If so, just activate that one and let this one fizzle out.
                        instanceAlreadyRunning = (NSString)launchedAppValue;
                        if (!instanceAlreadyRunning.EndsWith("/jzintv", StringComparison.OrdinalIgnoreCase))
                        {
                            instanceAlreadyRunning = null;
                        }
                    }
                }
                if (!string.IsNullOrEmpty(instanceAlreadyRunning))
                {
                    inst.Add((NSString)launchedAppValue);
                }
            }
            System.Diagnostics.Debug.WriteLine("NATIVE Took ms to get by name: " + stopwatch.ElapsedMilliseconds);
        }
#endif // MAC_PERF_TEST

        /// <summary>
        /// Launch the emulator to run the given ROM, using the supplied options.
        /// </summary>
        /// <param name="process">The emulator process to start.</param>
        /// <param name="options">The command line options for the emulator.</param>
        /// <param name="program">The program to run in the emulator.</param>
        /// <returns><c>true</c> if the process launched successfully; <c>false</c> otherwise.</returns>
        public bool Launch(Process process, IDictionary<CommandLineArgument, object> options, ProgramDescription program)
        {
            Rom = program;
            var commandLineArguments = options.BuildCommandLineArguments(program.Rom.RomPath.Path, false);
            process.StartInfo.Arguments = commandLineArguments;
            process.StartInfo.FileName = Path;
            process.StartInfo.WorkingDirectory = System.IO.Path.GetDirectoryName(Path);
            process.StartInfo.CreateNoWindow = true;
            process.StartInfo.UseShellExecute = false;
            process.StartInfo.RedirectStandardOutput = true;
            process.StartInfo.RedirectStandardError = true;
            process.EnableRaisingEvents = true;
            process.OutputDataReceived += HandleOutputDataReceived;
            process.ErrorDataReceived += HandleErrorDataReceived;
            process.Exited += HandleProcessExited;

            var started = process.Start();
            process.BeginOutputReadLine();
            process.BeginErrorReadLine();
            Process = process;
            _launchedInstances.TryAdd(this, program);
            System.Diagnostics.Debug.WriteLine("jzIntv Launched with arguments: " + commandLineArguments);
            return started;
        }

        private static bool AppendDataFromBuffer(StringBuilder report, IEnumerable<string> buffer, int maxLinesToReport)
        {
            var reportedAnything = false;
            if (buffer.Any())
            {
                var toProcess = buffer.Where(l => !string.IsNullOrWhiteSpace(l));
                var linesToSkip = System.Math.Max(0, toProcess.Count() - maxLinesToReport);
                foreach (var errorLine in toProcess.Skip(linesToSkip))
                {
                    report.AppendLine(errorLine);
                    reportedAnything = true;
                }
            }
            return reportedAnything;
        }

        private void HandleProcessExited(object sender, EventArgs e)
        {
            ProgramDescription dontCare;
            _launchedInstances.TryRemove(this, out dontCare);
            try
            {
                var process = sender as Process;
                System.Diagnostics.Debug.WriteLine("jzIntv EXIT: " + process.ExitCode);
                if ((process.ExitCode != 0) && (ErrorReporter != null))
                {
                    var errorMessage = new System.Text.StringBuilder().AppendFormat(INTV.jzIntv.Resources.Strings.jzIntvExitedWithError_MessageFormat, ((Rom == null) || (Rom.Rom == null)) ? "<Invalid ROM>" : Rom.Rom.RomPath.Path, process.ExitCode);
                    if (StderrBuffer.Any(l => !string.IsNullOrWhiteSpace(l)) || StdoutBuffer.Any(l => !string.IsNullOrWhiteSpace(l)))
                    {
                        errorMessage.AppendLine();
                    }
                    var maxLinesToReport = 12;
                    if (AppendDataFromBuffer(errorMessage, StderrBuffer, maxLinesToReport))
                    {
                        errorMessage.AppendLine();
                    }
                    else if (AppendDataFromBuffer(errorMessage, StdoutBuffer, maxLinesToReport))
                    {
                        errorMessage.AppendLine();
                    }
                    errorMessage.AppendFormat("Command line arguments:\n{0}", process.StartInfo.Arguments);
                    ErrorReporter(this, errorMessage.ToString(), process.ExitCode, null);
                }
            }
            catch (Exception)
            {
            }
        }

        private void HandleOutputDataReceived(object sender, DataReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("jzIntv STDOUT: " + e.Data);
            StdoutBuffer.Enqueue(e.Data);
        }

        private void HandleErrorDataReceived(object sender, DataReceivedEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine("jzIntv STDERR: " + e.Data);
            StderrBuffer.Enqueue(e.Data);
        }

        /// <summary>
        /// Fixed sized concurrent queue.
        /// </summary>
        /// <remarks>See Stack Overflow:
        /// http://stackoverflow.com/questions/5852863/fixed-size-queue-which-automatically-dequeues-old-values-upon-new-enques\
        /// </remarks>
        private class FixedSizedConcurrentQueue : ConcurrentQueue<string>
        {
            private readonly object _lock = new object();

            public FixedSizedConcurrentQueue(int maxSize)
            {
                MaxSize = maxSize;
            }

            public int MaxSize { get; private set; }

            public new void Enqueue(string value)
            {
                base.Enqueue(value);
                lock (_lock)
                {
                    string dontCare;
                    while (Count > MaxSize)
                    {
                        TryDequeue(out dontCare);
                    }
                }
            }
        }
    }
}
