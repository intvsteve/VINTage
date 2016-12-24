// <copyright file="ReflectionHelpers.cs" company="INTV Funhouse">
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
using System.Collections.Generic;
using System.ComponentModel;
using System.Reflection;

namespace INTV.Shared.Utility
{
    /// <summary>
    /// Some useful methods for doing evil... er, reflection stuff.
    /// </summary>
    /// <remarks>Event handler removal adapted from: http://www.codeproject.com/Articles/103542/Removing-Event-Handlers-using-Reflection</remarks>
    public static class ReflectionHelpers
    {
        /// <summary>
        /// Gets event handlers attached to an object.
        /// </summary>
        /// <param name="o">The object whose event handling delegates for a particular event are desired.</param>
        /// <param name="eventName">The name of the event whose handlers are requested.</param>
        /// <returns>An enumerable containing the event handling delegates.</returns>
        /// <remarks>PROTOTYPE CODE: Not well-vetted or tested.</remarks>
        public static IEnumerable<Delegate> GetEventHandlers(this object o, string eventName)
        {
            Delegate[] invocationList = null;
            if ((o != null) && !string.IsNullOrEmpty(eventName))
            {
                var type = o.GetType();
                var eventFields = GetTypeEventFields(type);
                foreach (var fieldInfo in eventFields)
                {
                    if (!string.IsNullOrEmpty(eventName) && (string.Compare(eventName, fieldInfo.Name, true) != 0))
                    {
                        continue;
                    }
                    var eventField = fieldInfo;
                    var eventInfo = type.GetEvent(eventField.Name, AllBindings);
                    if (eventInfo != null)
                    {
                        var value = eventField.GetValue(o);
                        var handler = value as Delegate;
                        if (handler != null)
                        {
                            invocationList = handler.GetInvocationList();
                        }
                    }
                }
            }
            return invocationList;
        }

        /// <summary>
        /// Removes all event handlers from an object.
        /// </summary>
        /// <param name="o">The object from which all event handlers are to be disconnected.</param>
        public static void RemoveAllEventHandlers(this object o)
        {
            RemoveEventHandler(o, null);
        }

        /// <summary>
        /// Removes event handlers for a particular event.
        /// </summary>
        /// <param name="o">The object from which event handlers are to be disconnected.</param>
        /// <param name="eventName">The name of the event from which handlers are to be disconnected. If <c>null</c>, all event handlers
        /// for all events are disconnected.</param>
        public static void RemoveEventHandler(this object o, string eventName)
        {
            if (o == null)
            {
                return;
            }

            var type = o.GetType();
            var eventFields = GetTypeEventFields(type);
            EventHandlerList staticEventHandlers = null;

            foreach (var fieldInfo in eventFields)
            {
                if (!string.IsNullOrEmpty(eventName) && (string.Compare(eventName, fieldInfo.Name, true) != 0))
                {
                    continue;
                }

                // After hours and hours of research and trial and error, it turns out that
                // STATIC Events have to be treated differently from INSTANCE Events...
                if (fieldInfo.IsStatic)
                {
                    if (staticEventHandlers == null)
                    {
                        staticEventHandlers = GetStaticEventHandlerList(type, o);
                    }

                    object index = fieldInfo.GetValue(o);
                    var eventHandler = staticEventHandlers[index];
                    if (eventHandler == null)
                    {
                        continue;
                    }

                    var delegates = eventHandler.GetInvocationList();
                    if (delegates == null)
                    {
                        continue;
                    }

                    var eventInfo = type.GetEvent(fieldInfo.Name, AllBindings);
                    foreach (var handler in delegates)
                    {
                        eventInfo.RemoveEventHandler(o, handler);
                    }
                }
                else
                {
                    var eventInfo = type.GetEvent(fieldInfo.Name, AllBindings);
                    if (eventInfo != null)
                    {
                        var value = fieldInfo.GetValue(o);
                        var handler = value as Delegate;
                        if (handler != null)
                        {
                            foreach (var del in handler.GetInvocationList())
                            {
                                eventInfo.RemoveEventHandler(o, del);
                            }
                        }
                    }
                }
            }
        }

        private static System.Reflection.BindingFlags AllBindings
        {
            get { return BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static; }
        }

        private static List<FieldInfo> GetTypeEventFields(Type type)
        {
            var fieldInfos = new List<FieldInfo>();
            BuildEventFields(type, fieldInfos);
            return fieldInfos;
        }

        private static void BuildEventFields(Type type, List<FieldInfo> fieldInfos)
        {
            // Type.GetEvent(s) gets all Events for the type AND it's ancestors.
            // Type.GetField(s) gets only Fields for the exact type.
            //  (BindingFlags.FlattenHierarchy only works on PROTECTED & PUBLIC
            //   doesn't work because Fields are PRIVATE)

            // NEW version of this routine uses .GetEvents and then uses .DeclaringType
            // to get the correct ancestor type so that we can get the FieldInfo.
            foreach (var eventInfo in type.GetEvents(AllBindings))
            {
                var declaringType = eventInfo.DeclaringType;
                var fieldInfo = declaringType.GetField(eventInfo.Name, AllBindings);
                if (fieldInfo != null)
                {
                    fieldInfos.Add(fieldInfo);
                }
            }
        }

        private static EventHandlerList GetStaticEventHandlerList(Type type, object o)
        {
            var methodInfo = type.GetMethod("get_Events", AllBindings);
            return (EventHandlerList)methodInfo.Invoke(o, new object[] { });
        }
    }
}
