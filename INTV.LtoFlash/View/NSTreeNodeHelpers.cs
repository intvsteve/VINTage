// <copyright file="NSTreeNodeHelpers.cs" company="INTV Funhouse">
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
//

#if __UNIFIED__
using AppKit;
using Foundation;
using ObjCRuntime;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
using MonoMac.ObjCRuntime;
#endif // __UNIFIED__

namespace INTV.LtoFlash.View
{
    /// <summary>
    /// Bindings for methods that are either missing, or done incorrectly, for NSTreeNode and NSTreeController.
    /// </summary>
    /// <remarks>TODO: Put into INTV.Shared to be generally available.</remarks>
    internal static class NSTreeNodeHelpers
    {
#if __UNIFIED__
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern System.IntPtr IntPtr_objc_msgSend(System.IntPtr receiver, System.IntPtr selector);
        [System.Runtime.InteropServices.DllImport("/usr/lib/libobjc.dylib", EntryPoint = "objc_msgSend")]
        private static extern void void_objc_msgSend_IntPtr(System.IntPtr receiver, System.IntPtr selector, System.IntPtr arg1);
#endif // __UNIFIED__

        private static System.IntPtr selRepresentedObjectHandle = Selector.GetHandle("representedObject");

        /// <summary>
        /// Gets the represented object in the <see cref="NSTreeNode"/>.
        /// </summary>
        /// <param name="node">The node whose represented object is desired.</param>
        /// <returns>The represented object, which must be a <see cref="NSObject"/>.</returns>
        /// <remarks>The MonoMac binding for this uses the incorrect return type.</remarks>
        internal static NSObject GetRepresentedObject(this NSTreeNode node)
        {
#if __UNIFIED__
            var representedObject = node.RepresentedObject;
#else
            var representedObject = Runtime.GetNSObject(Messaging.IntPtr_objc_msgSend(node.Handle, selRepresentedObjectHandle));
#endif // __UNIFIED__
            return representedObject;
        }

        private static System.IntPtr selSetContentObjectHandle = Selector.GetHandle("setContent:");

        /// <summary>
        /// Sets the content of the tree controller.
        /// </summary>
        /// <param name="treeController">The tree controller whose content is to be set.</param>
        /// <param name="content">The content of the tree controller.</param>
        /// <remarks>The MonoMac binding uses the incorrect type for the <paramref name="content"/> argument.</remarks>
        internal static void SetContent(this NSTreeController treeController, NSObject content)
        {
#if __UNIFIED__
            treeController.Content = content;
#else
            Messaging.void_objc_msgSend_IntPtr(treeController.Handle, selSetContentObjectHandle, content.Handle);
#endif // __UNIFIED__
        }
    }
}
