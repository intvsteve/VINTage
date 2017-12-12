// <copyright file="OSDispatcherTimer.Gtk.cs" company="INTV Funhouse">
// Copyright (c) 2017 All Rights Reserved
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

// During part of the development on an Ubuntu 16.04.2 virtual machine (in VirtualBox, Mac OS X 10.8.5, MacBook Pro),
// there were some difficulties first noticed when Gtk.Application.Timeout() was used as the basis for
// this timer implementation, combined with using it to pulse a Gtk.ProgressBar. Performace was much more
// reliable using a System.Threading.Timer to execute the callback, and then use Gtk.Application.Invoke()
// to marshal the call to the main thread. The observed behavior was that the thread pool thread(s) used for
// serial IO would seem to be blocked when Gtk.Application.Timeout() was used.
// However, when these problems were happening, the wooly-headed ninny-muggins developer who was working on the
// code *finally* noticed that VirtualBox was misconfigured to use a USB 1.1 OHCI USB controller, rather than
// a USB 2.0 EHCI controller. After fixing the USB settings in the VM, performance became much more reliable.
// Just in case later development encounters more seemingly timer-related trouble, this can be changed.
////#define USE_SYSTEM_THREADING_TIMER

////#define ENABLE_DEBUG_OUTPUT

using System;

#if USE_SYSTEM_THREADING_TIMER
using BaseClass = System.IDisposable;
#else
using BaseClass = System.Object;
#endif // USE_SYSTEM_THREADING_TIMER

namespace INTV.Shared.Utility
{
    /// <summary>
    /// GTK-specific implementation.
    /// </summary>
    public partial class OSDispatcherTimer : BaseClass
    {
#if USE_SYSTEM_THREADING_TIMER
        private System.Threading.Timer _timer;
#else
        private uint _timer;
#endif // USE_SYSTEM_THREADING_TIMER
        private bool _callbackOnMainThread;

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.OSDispatcherTimer"/> class.
        /// </summary>
        /// <remarks>The timer Tick will be called on the main thread. To have the callback execute on a thread-pool thread, use the other constructor.</remarks>
        public OSDispatcherTimer()
            : this(true)
        {
        }

#if USE_SYSTEM_THREADING_TIMER
        ~OSDispatcherTimer()
        {
            Dispose(false);
        }
#endif // USE_SYSTEM_THREADING_TIMER

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.OSDispatcherTimer"/> class.
        /// </summary>
        /// <param name="callbackOnMainThread">If set to <c>true</c> the timer callback executes on the main thread. If <c>false</c>, timer tick executes on a thread pool thread.</param>
    public OSDispatcherTimer(bool callbackOnMainThread)
        {
#if USE_SYSTEM_THREADING_TIMER
            _timer = new System.Threading.Timer(TimerTick);
#else
            if (!callbackOnMainThread)
            {
                DebugOutput("OSDispatcherTimer: Callbacks always happen on main thread!");
            }
#endif // USE_SYSTEM_THREADING_TIMER
            _callbackOnMainThread = callbackOnMainThread;
        }

        /// <summary>
        /// Raised when the timer 'ticks'.
        /// </summary>
        public event System.EventHandler Tick;

        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        public System.TimeSpan Interval { get; set; }

        /// <summary>
        /// Start the timer.
        /// </summary>
        public void Start()
        {
#if USE_SYSTEM_THREADING_TIMER
            DebugOutput("+++++ STARTING TIMER from thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
            _timer.Change(Interval, Interval);
#else
            if (_timer == 0)
            {
                DebugOutput("+++++ STARTING TIMER from thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
                _timer = GLib.Timeout.Add(System.Convert.ToUInt32(Interval.TotalMilliseconds), TimerTick);
            }
            else
            {
                DebugOutput("+++++ TIMER already started... Thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
            }
#endif // USE_SYSTEM_THREADING_TIMER
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
#if USE_SYSTEM_THREADING_TIMER
            _timer.Change(System.Threading.Timeout.Infinite, System.Threading.Timeout.Infinite);
#else
            DebugOutput("!!!!! STOPPING TIMER from thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
            _timer = 0;
#endif // USE_SYSTEM_THREADING_TIMER
        }

#if USE_SYSTEM_THREADING_TIMER

        #region IDisposable

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            if (disposing)
            {
                if (_timer != null)
                {
                    _timer.Dispose();
                    _timer = null;
                }
            }
        }

        #endregion // IDisposable

        private void TimerTick(object state)
        {
            if (_callbackOnMainThread)
            {
                Gtk.Application.Invoke((s, e) => OnTimerFire(this, System.EventArgs.Empty));
            }
            else
            {
                OnTimerFire(this, System.EventArgs.Empty);
            }
        }
#else
        private bool TimerTick()
        {
            bool run = _timer != 0;
            if (run)
            {
                OnTimerFire(this, System.EventArgs.Empty);
            }
            return run;
        }
#endif // USE_SYSTEM_THREADING_TIMER

        private void OnTimerFire(object sender, System.EventArgs e)
        {
            try
            {
                var tick = Tick;
                if (tick != null)
                {
                    DebugOutput("@@@@@@@@@@@@@@@@@@TIMER TICK from thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
                    tick(this, e);
                    DebugOutput("@@@@@@@@@@@@@@@@@@TIMER TICK FINISHED from thread: " + System.Threading.Thread.CurrentThread.ManagedThreadId + ", MAIN: " + OSDispatcher.IsMainThread);
                }
            }
            catch (System.Exception exception)
            {
                INTV.Shared.Utility.ErrorReporting.ReportError(ReportMechanism.Console, exception.Message);
            }
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
