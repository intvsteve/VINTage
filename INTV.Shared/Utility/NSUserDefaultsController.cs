// <copyright file="NSUserDefaultsController.cs" company="INTV Funhouse">
// Copyright (c) 2014-2015 All Rights Reserved
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
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Implement bindings to the NSUserDefaultsController class in Cocoa. This isn't done yet in MonoMac, AFIAK.
    /// </summary>
    [Register ("NSUserDefaultsController")]
    public class NSUserDefaultsController : NSController
    {
        private static IntPtr class_ptr;
        private static Selector selSharedUserDefaultsController;
        private static Selector selInitWithDefaults;
        private static Selector selDefaults;
        private static Selector selInitialValues;
        private static Selector selSetInitialValues;
        private static Selector selAppliesImmediately;
        private static Selector selSetAppliesImmediately;
        private static Selector selHasUnappliedChanges;
        private static Selector selValues;
        private static Selector selRevert;
        private static Selector selRevertToInitialValues;
        private static Selector selSave;

        private static NSUserDefaultsController _sharedUserDefaultsController;

        private NSUserDefaults _defaults;
        private NSObject _values;
        private NSDictionary _initialValues;

        static NSUserDefaultsController()
        {
            NSUserDefaultsController.selSharedUserDefaultsController = new Selector("sharedUserDefaultsController");
            NSUserDefaultsController.selInitWithDefaults = new Selector("initWithDefaults:initialValues:");
            NSUserDefaultsController.selDefaults = new Selector("defaults");
            NSUserDefaultsController.selInitialValues = new Selector("initialValues");
            NSUserDefaultsController.selSetInitialValues = new Selector("setInitialValues:");
            NSUserDefaultsController.selAppliesImmediately = new Selector("appliesImmediately");
            NSUserDefaultsController.selSetAppliesImmediately = new Selector("setAppliesImmediately:");
            NSUserDefaultsController.selHasUnappliedChanges = new Selector("hasUnappliedChanges");
            NSUserDefaultsController.selValues = new Selector("values");
            NSUserDefaultsController.selRevert = new Selector("revert:");
            NSUserDefaultsController.selRevertToInitialValues = new Selector("revertToInitialValues:");
            NSUserDefaultsController.selSave = new Selector("save:");
            NSUserDefaultsController.class_ptr = Class.GetHandle("NSUserDefaultsController");
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="INTV.Shared.Utility.NSUserDefaultsController"/> class.
        /// </summary>
        /// <param name="defaults">The defaults to use.</param>
        /// <param name="initialValues">Initial values.</param>
        [Export("initWithDefaults:initialValues:")]
        public NSUserDefaultsController(NSUserDefaults defaults, NSDictionary initialValues)
            : base(NSObjectFlag.Empty)
        {
            IntPtr defaultsHandle = IntPtr.Zero;
            if (defaults != null)
            {
                defaultsHandle = defaults.Handle;
            }
            if (initialValues == null)
            {
                throw new ArgumentNullException ("initialValues");
            }
            if (this.IsDirectBinding)
            {
                base.Handle = Messaging.IntPtr_objc_msgSend_IntPtr_IntPtr(base.Handle, NSUserDefaultsController.selInitWithDefaults.Handle, defaultsHandle, initialValues.Handle);
            }
            else
            {
                base.Handle = Messaging.IntPtr_objc_msgSendSuper_IntPtr_IntPtr(base.SuperHandle, NSUserDefaultsController.selInitWithDefaults.Handle, defaultsHandle, initialValues.Handle);
            }
        }

        /// <summary>
        /// Called when created from unmanaged code.
        /// </summary>
        /// <param name="handle">Native pointer to NSView.</param>
        public NSUserDefaultsController(IntPtr handle)
            : base(handle)
        {
        }

        /// <summary>
        /// The shared user defaults controller instance.
        /// </summary>
        public static NSUserDefaultsController SharedUserDefaultsController
        {
            [Export("sharedUserDefaultsController")]
            get
            {
                NSUserDefaultsController nsUserDefaultsController = (NSUserDefaultsController)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend (NSUserDefaultsController.class_ptr, NSUserDefaultsController.selSharedUserDefaultsController.Handle));
                NSUserDefaultsController._sharedUserDefaultsController = nsUserDefaultsController;
                return _sharedUserDefaultsController;
            }
        }

        /// <summary>
        /// Gets the class handle.
        /// </summary>
        public override IntPtr ClassHandle
        {
            get
            {
                return NSUserDefaultsController.class_ptr;
            }
        }

        /// <summary>
        /// Gets the default settings.
        /// </summary>
        public NSUserDefaults Defaults
        {
            [Export("defaults")]
            get
            {
                NSUserDefaults defaults;
                if (this.IsDirectBinding)
                {
                    defaults = (NSUserDefaults)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(base.Handle, NSUserDefaultsController.selDefaults.Handle));
                }
                else
                {
                    defaults = (NSUserDefaults)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, NSUserDefaultsController.selDefaults.Handle));
                }
                _defaults = defaults;
                return _defaults;
            }
        }

        /// <summary>
        /// Gets the values stored in the settings.
        /// </summary>
        public NSObject Values
        {
            [Export("values")]
            get
            {
                NSObject values;
                if (this.IsDirectBinding)
                {
                    values = (NSObject)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(base.Handle, NSUserDefaultsController.selValues.Handle));
                }
                else
                {
                    values = (NSObject)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper(base.SuperHandle, NSUserDefaultsController.selValues.Handle));
                }
                _values = values;
                return _values;
            }
        }

        /// <summary>
        /// Gets or sets whether the changes should immediately take effect.
        /// </summary>
        public bool AppliesImmediately
        {
            [Export("appliesImmediately")]
            get
            {
                bool result;
                if (this.IsDirectBinding)
                {
                    result = Messaging.bool_objc_msgSend(base.Handle, NSUserDefaultsController.selAppliesImmediately.Handle);
                }
                else
                {
                    result = Messaging.bool_objc_msgSendSuper(base.SuperHandle, NSUserDefaultsController.selAppliesImmediately.Handle);
                }
                return result;
            }
            [Export("setAppliesImmediately:")]
            set
            {
                if (this.IsDirectBinding)
                {
                    Messaging.void_objc_msgSend_bool(base.Handle, NSUserDefaultsController.selSetAppliesImmediately.Handle, value);
                }
                else
                {
                    Messaging.void_objc_msgSendSuper_bool(base.SuperHandle, NSUserDefaultsController.selSetAppliesImmediately.Handle, value);
                }
            }
        }

        /// <summary>
        /// Gets whether or not there are unapplied changes to the settings.
        /// </summary>
        public bool HasUnappliedChanges
        {
            [Export("hasUnappliedChanges")]
            get
            {
                bool result;
                if (this.IsDirectBinding)
                {
                    result = Messaging.bool_objc_msgSend(base.Handle, NSUserDefaultsController.selHasUnappliedChanges.Handle);
                }
                else
                {
                    result = Messaging.bool_objc_msgSendSuper(base.SuperHandle, NSUserDefaultsController.selHasUnappliedChanges.Handle);
                }
                return result;
            }
        }

        /// <summary>
        /// Gets or sets the initial (default) values of the settings.
        /// </summary>
        public NSDictionary InitialValues
        {
            [Export("initialValues")]
            get
            {
                NSDictionary initialValues;
                if (this.IsDirectBinding)
                {
                    initialValues = (NSDictionary)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend (base.Handle, NSUserDefaultsController.selInitialValues.Handle));
                }
                else
                {
                    initialValues = (NSDictionary)Runtime.GetNSObject(Messaging.IntPtr_objc_msgSendSuper (base.SuperHandle, NSUserDefaultsController.selInitialValues.Handle));
                }
                _initialValues = initialValues;
                return _initialValues;
            }
            [Export("setInitialValues:")]
            set
            {
                if (this.IsDirectBinding)
                {
                    Messaging.void_objc_msgSend_IntPtr (base.Handle, NSUserDefaultsController.selSetInitialValues.Handle, value.Handle);
                }
                else
                {
                    Messaging.void_objc_msgSendSuper_IntPtr (base.SuperHandle, NSUserDefaultsController.selSetInitialValues.Handle, value.Handle);
                }
                _initialValues = value;
            }
        }

        /// <summary>
        /// Reverts changes to the settings.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        [Export("revert:")]
        public void Revert(NSObject sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException ("sender");
            }
            if (this.IsDirectBinding)
            {
                Messaging.void_objc_msgSend_IntPtr(this.Handle, NSUserDefaultsController.selRevert.Handle, sender.Handle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper_IntPtr(this.SuperHandle, NSUserDefaultsController.selRevert.Handle, sender.Handle);
            }
        }

        /// <summary>
        /// Reverts all values to their defaults.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        [Export("revertToInitialValues:")]
        public void RevertToInitialValues(NSObject sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException ("sender");
            }
            if (this.IsDirectBinding)
            {
                Messaging.void_objc_msgSend_IntPtr(this.Handle, NSUserDefaultsController.selRevertToInitialValues.Handle, sender.Handle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper_IntPtr(this.SuperHandle, NSUserDefaultsController.selRevertToInitialValues.Handle, sender.Handle);
            }
        }

        /// <summary>
        /// Save the settings.
        /// </summary>
        /// <param name="sender">The sender of the message.</param>
        [Export("save:")]
        public void Save(NSObject sender)
        {
            if (sender == null)
            {
                throw new ArgumentNullException ("sender");
            }
            if (this.IsDirectBinding)
            {
                Messaging.void_objc_msgSend_IntPtr(this.Handle, NSUserDefaultsController.selSave.Handle, sender.Handle);
            }
            else
            {
                Messaging.void_objc_msgSendSuper_IntPtr(this.SuperHandle, NSUserDefaultsController.selSave.Handle, sender.Handle);
            }
        }
    }
}
