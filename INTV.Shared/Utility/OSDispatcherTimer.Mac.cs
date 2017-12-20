// <copyright file="OSDispatcherTimer.Mac.cs" company="INTV Funhouse">
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

////#define ENABLE_DEBUG_OUTPUT

#if __UNIFIED__
using Foundation;
#else
using MonoMac.Foundation;
#endif // __UNIFIED__

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Mac-specific implementation.
    /// </summary>
    public partial class OSDispatcherTimer
    {
        private NSTimer _timer;

        /// <summary>
        /// Raised when the timer 'ticks'.
        /// </summary>
        public event System.EventHandler Tick;

        /// <summary>
        /// Gets or sets the timer interval.
        /// </summary>
        public System.TimeSpan Interval
        {
            get;
            set;
        }

        /// <summary>
        /// Start the timer.
        /// </summary>
        public void Start()
        {
            DebugOutput("+++++ STARTING TIMER from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
            var runLoop = NSRunLoop.Current;
            _timer = NSTimer.CreateRepeatingTimer(Interval, TimerTick);
            runLoop.AddTimer(_timer, NSRunLoop.NSRunLoopCommonModes);
        }

        /// <summary>
        /// Stop the timer.
        /// </summary>
        public void Stop()
        {
            DebugOutput("!!!!! STOPPING TIMER from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
            _timer.Invalidate();
            _timer.Dispose();
            _timer = null;
        }

        [System.Diagnostics.Conditional("ENABLE_DEBUG_OUTPUT")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }

        /// <summary>
        /// Executes the timer function.
        /// </summary>
        /// <remarks>NOTE: This will be called from the thread the timer was targeted to when it was created.</remarks>
#if __UNIFIED__
        private void TimerTick(NSTimer timer)
#else
        private void TimerTick()
#endif // __UNIFIED__
        {
            OnTimerFire(this, System.EventArgs.Empty);
        }

        private void OnTimerFire(object sender, System.EventArgs e)
        {
            try
            {
                var tick = Tick;
                if (tick != null)
                {
                    DebugOutput("@@@@@@@@@@@@@@@@@@TIMER TICK from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
                    tick(this, e);
                    DebugOutput("@@@@@@@@@@@@@@@@@@TIMER TICK FINISHED from thread: " + NSThread.Current.Handle + ", MAIN: " + NSThread.MainThread.Handle);
                }
            }
            catch (System.Exception exception)
            {
                INTV.Shared.Utility.ErrorReporting.ReportError(ReportMechanism.Console, exception.Message);
            }
        }
    }
}
