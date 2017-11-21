// <copyright file="TextCellInPlaceEditor.Mac.cs" company="INTV Funhouse">
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

#define DO_CLEANUP
////#define USE_APPKIT_CONSTANTS
////#define IMPLEMENTS_IDISPOSABLE
////#define ENABLE_INPLACEEDIT_TRACE

using System.Linq;
#if __UNIFIED__
using AppKit;
using Foundation;
#else
using MonoMac.AppKit;
using MonoMac.Foundation;
#endif // __UNIFIED__
using INTV.Shared.Behavior;
using INTV.Shared.Utility;

namespace INTV.Shared.View
{
    /// <summary>
    /// This class implements IInPlaceEditor as a way to interact with the built-in cell editor
    /// for the NSTableView-based classes.
    /// </summary>
    public class TextCellInPlaceEditor : IInPlaceEditor
#if IMPLEMENTS_IDISPOSABLE
    , IDisposable
#endif // IMPLEMENTS_IDISPOSABLE
    {
#if USE_APPKIT_CONSTANTS
        private const string AppKitLibrary = "/System/Library/Frameworks/AppKit.framework/AppKit";
        static IntPtr AppKit = MonoMac.ObjCRuntime.Dlfcn.dlopen(AppKitLibrary, 1);
#else
        private static readonly NSString NSControlTextDidBeginEditingNotification = new NSString("NSControlTextDidBeginEditingNotification");
        private static readonly NSString NSControlTextDidChangeNotification = new NSString("NSControlTextDidChangeNotification");
        private static readonly NSString NSControlTextDidEndEditingNotification = new NSString("NSControlTextDidEndEditingNotification");
#endif // USE_APPKIT_CONSTANTS
        private static readonly NSString FieldEditorKey = new NSString("NSFieldEditor");

        private static readonly char[] AlwaysAllowedCharacters = new char[]
        {
            (char)8, // delete
            (char)13, // return
            (char)127, // delete
            (char)63272, // backspace
            (char)63234, // left arrow
            (char)63235, // right arrow
        };

        /// <summary>
        /// Initializes a new instance of the in-place editor for a text cell.
        /// </summary>
        /// <param name="owner">The visual owning the element being edited.</param>
        /// <param name="initialValue">The initial value of the edited item.</param>
        /// <param name="editingObject">The entity being edited.</param>
        public TextCellInPlaceEditor(NSView owner)
        {
            ElementOwner = owner;
            MaxLength = -1;
        }

#if IMPLEMENTS_IDISPOSABLE

        ~TextCellInPlaceEditor()
        {
            Dispose(false);
        }

#endif // IMPLEMENTS_IDISPOSABLE

        #region Properties

#if USE_APPKIT_CONSTANTS

        [Field("NSControlTextDidBeginEditingNotification", "AppKit")]
        public static NSString NSControlTextDidBeginEditingNotification
        {
            get
            {
                if (_nsControlTextDidBeginEditingNotification == null)
                {
                    _nsControlTextDidBeginEditingNotification = MonoMac.ObjCRuntime.Dlfcn.GetStringConstant(AppKit, "NSControlTextDidBeginEditingNotification");
                }
                return _nsControlTextDidBeginEditingNotification;
            }
        }
        private static NSString _nsControlTextDidBeginEditingNotification;

        [Field("NSControlTextDidChangeNotification", "AppKit")]
        public static NSString NSControlTextDidChangeNotification
        {
            get
            {
                if (_nsControlTextDidChangeNotification == null)
                {
                    _nsControlTextDidChangeNotification = MonoMac.ObjCRuntime.Dlfcn.GetStringConstant(AppKit, "NSControlTextDidChangeNotification");
                }
                return _nsControlTextDidChangeNotification;
            }
        }
        private static NSString _nsControlTextDidChangeNotification;

        [Field("NSControlTextDidEndEditingNotification", "AppKit")]
        public static NSString NSControlTextDidEndEditingNotification
        {
            get
            {
                if (_nsControlTextDidEndEditingNotification == null)
                {
                    _nsControlTextDidEndEditingNotification = MonoMac.ObjCRuntime.Dlfcn.GetStringConstant(AppKit, "NSControlTextDidEndEditingNotification");
                }
                return _nsControlTextDidEndEditingNotification;
            }
        }
        private static NSString _nsControlTextDidEndEditingNotification;

#endif // USE_APPKIT_CONSTANTS

        #region IInPlaceEditor Properties

        /// <inheritdoc/>
        public NSView EditedElement { get; set; }

        /// <inheritdoc/>
        public NSView ElementOwner { get; set; }

        #endregion // IInPlaceEditor Properties

        /// <summary>
        /// Gets or sets the maximum length allowed for the string being edited.
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Gets or sets a function to call to verify if a character is allowed.
        /// </summary>
        public System.Predicate<char> IsValidCharacter { get; set; }

        /// <summary>
        /// Gets or sets the object being edited.
        /// </summary>
        public object EditingObject { get; set; }

        /// <summary>
        /// Gets or sets the custom key event monitor.
        /// </summary>
        private NSObject KeyMonitor { get; set; }

        /// <summary>
        /// Gets or sets the observer for the NSControlTextDidBeginEditingNotification.
        /// </summary>
        private NSObject EditingStartedObserver { get; set; }

        /// <summary>
        /// Gets or sets the observer for the NSControlTextDidEndEditingNotification.
        /// </summary>
        private NSObject EditingEndedObserver { get; set; }

        /// <summary>
        /// Gets or sets the observer for the NSControlTextDidChangeNotification.
        /// </summary>
        private NSObject EditingTextChanged { get; set; }

        /// <summary>
        /// Gets or sets the live value in the field editor.
        /// </summary>
        private string LiveValue { get; set; }

        /// <summary>
        /// Gets or sets the initial value of the field editor.
        /// </summary>
        public string InitialValue { get; set; }

        /// <summary>
        /// Gets or sets the NSTextStorage we're editing.
        /// </summary>
        /// <remarks>NOTE: DO NOT DISPOSE! This refers to the text storage in the entity
        /// being edited and is not owned by this class, but by the control that contains
        /// the cell being edited.</remarks>
        private NSTextStorage TextStorage { get; set; }

        /// <summary>
        /// Gets or sets the custom NSTextStorageDelegate used to control text entry.
        /// </summary>
        /// <remarks>DO NOT DISPOSE! Once this has been assigned to an NSTextStorage, it
        /// is owned by that NSTextStorage! We cannot assign a <c>null</c> delegate back
        /// to the original storage (enforced by MonoMac), so we just let follow its
        /// natural life cycle.</remarks>
        private LimitedTextEditing TextChecker { get; set; }

        #endregion // Properties

        #region IInPlaceEditor

        /// <inheritdoc/>
        public event System.EventHandler<InPlaceEditorClosedEventArgs> EditorClosed;

        /// <inheritdoc/>
        public void BeginEdit()
        {
            DebugOutput("!$!$!$!$ BEGIN EDIT");
            LiveValue = InitialValue;
            if (KeyMonitor == null)
            {
                DebugOutput("  !$!$!$!$ INSTALLED KEY MONITOR");
                KeyMonitor = NSEvent.AddLocalMonitorForEventsMatchingMask(NSEventMask.KeyDown, LocalKeyEventHandler);
            }
            if (EditingStartedObserver == null)
            {
                DebugOutput("  !$!$!$!$ INSTALLED EDITING STARTED OBSERVER");
                EditingStartedObserver = NSNotificationCenter.DefaultCenter.AddObserver(NSControlTextDidBeginEditingNotification, CellTextEditStarted);
            }
            if (EditingTextChanged == null)
            {
                DebugOutput("  !$!$!$!$ INSTALLED TEXT CHANGED OBSERVER");
                EditingTextChanged = NSNotificationCenter.DefaultCenter.AddObserver(NSControlTextDidChangeNotification, CellTextChanged);
            }
            if (EditingEndedObserver == null)
            {
                DebugOutput("  !$!$!$!$ INSTALLED EDITING ENDED OBSERVER");
                EditingEndedObserver = NSNotificationCenter.DefaultCenter.AddObserver(NSControlTextDidEndEditingNotification, CellTextEditEnded);
            }
        }

        /// <inheritdoc/>
        public void CancelEdit()
        {
            DebugOutput("!$!$!$!$ CANCEL EDIT");
            var textStorage = TextStorage;
            if (textStorage != null)
            {
                var current = TextStorage.Value;
                var length = current == null ? 0 : current.Length;
                if (InitialValue == null)
                {
                    InitialValue = string.Empty;
                }
                textStorage.Replace(new NSRange(0, length), InitialValue);
            }
            var app = NSApplication.SharedApplication;
            if (app != null)
            {
                var mainWindow = app.MainWindow;
                var elementOwner = ElementOwner;
                if ((mainWindow != null) && (elementOwner != null))
                {
                    mainWindow.MakeFirstResponder(elementOwner);
                }
            }
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(false));
            }
        }

        /// <inheritdoc/>
        public void CommitEdit()
        {
            DebugOutput("!$!$!$!$ COMMIT EDIT");
            NSApplication.SharedApplication.MainWindow.MakeFirstResponder(ElementOwner);
            var editorClosed = EditorClosed;
            if (editorClosed != null)
            {
                editorClosed(this, new InPlaceEditorClosedEventArgs(true));
            }
        }

        #endregion // IInPlaceEditor

#if IMPLEMENTS_IDISPOSABLE

        #region IDisposable

        /// <inheritdoc/>
        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void Dispose(bool disposing)
        {
            DebugOutput("~~~~~~~~~~~~~~~~~~~~~~ InPlaceEditor.Dispose(" + disposing + ")" ENTER);
#if DO_CLEANUP
            if (EditingEndedObserver != null)
            {
                DebugOutput("  ~!~!~!~! InPlaceEditor.Dispose: removing end edit observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingEndedObserver);
                EditingEndedObserver.Dispose();
                EditingEndedObserver = null;
            }
            if (EditingTextChanged != null)
            {
                DebugOutput("  ~!~!~!~! InPlaceEditor.Dispose: removing edit text changed observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingTextChanged);
                EditingTextChanged.Dispose();
                EditingTextChanged = null;
            }
            if (EditingStartedObserver != null)
            {
                DebugOutput("  ~!~!~!~! InPlaceEditor.Dispose: removing edit started observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingStartedObserver);
                EditingStartedObserver.Dispose();
                EditingStartedObserver = null;
            }
#endif // DO_CLEANUP
            if (KeyMonitor != null)
            {
                DebugOutput("  ~!~!~!~! InPlaceEditor.Dispose: removing key monitor");
                NSEvent.RemoveMonitor(KeyMonitor);
                KeyMonitor.Dispose();
                KeyMonitor = null;
            }
            DebugOutput("~~~~~~~~~~~~~~~~~~~~~~ InPlaceEditor.Dispose(" + disposing + ") EXIT");
        }

        #endregion // IDisposable

#endif // IMPLEMENTS_IDISPOSABLE

        private NSEvent LocalKeyEventHandler(NSEvent theEvent)
        {
            var anEvent = theEvent;
            DebugOutput("*$*$*$*$ KEY MONITOR CALLED");
            if (!NSApplication.SharedApplication.Windows.Contains(anEvent.Window) || (anEvent.Window.FirstResponder == null) ||
                !(anEvent.Window.FirstResponder is NSView) || (((NSView)anEvent.Window.FirstResponder).GetParent<NSView>(v => v.GetType() == ElementOwner.GetType()) == null))
            {
                CellTextEditEnded(null);
            }
            if (EditingObject != null)
            {
                var modifiers = NSEvent.CurrentModifierFlags;
                if (modifiers.HasFlag(NSEventModifierMask.CommandKeyMask))
                {
                    var supportedShortcutKeys = new[] { ".", "c", "v", "x", "z" };
                    if (!supportedShortcutKeys.Contains(anEvent.Characters))
                    {
#if ENABLE_INPLACEEDIT_TRACE
                        INTV.Shared.Utility.ErrorReporting.ReportError(ReportMechanism.Debug, "*$*$*$*$ shortcut for " + anEvent.Characters + " DISABLED");
#endif // ENABLE_INPLACEEDIT_TRACE
                        anEvent = null;
                    }
                    if ((anEvent != null) && (modifiers == NSEventModifierMask.CommandKeyMask) && (anEvent.Characters == "."))
                    {
                        CancelEdit();
                    }
                }
                else
                {
                    var characters = anEvent.Characters.ToArray();
                    var anyBadCharacters = false;
                    var skipLengthCheck = false;
                    foreach (var c in characters)
                    {
                        if (c == 27)
                        {
                            // Escape key pressed.
                            CancelEdit();
                            return anEvent;
                        }
                        skipLengthCheck |= (MaxLength <= 0) || AlwaysAllowedCharacters.Contains(c);
                        if ((IsValidCharacter != null) && !char.IsControl(c) && !skipLengthCheck)
                        {
                            anyBadCharacters = !IsValidCharacter(c);
                            if (anyBadCharacters)
                            {
                                anEvent = null;
                                DebugOutput("*$*$*$*$ REMOVING CHARACTERS");
                                break;
                            }
                        }
                    }
                    if (!anyBadCharacters && !skipLengthCheck)
                    {
                        if ((MaxLength > 0) && (LiveValue != null) && ((LiveValue.Length + characters.Length) > MaxLength))
                        {
                            // Check if responder is our text editor that has a selection that would allow editing...
                            var textView = anEvent.Window.FirstResponder as NSTextView;
                            if ((EditingObject != null) && (textView != null) && textView.FieldEditor && (textView.SelectedRange.Length > 0))
                            {
                                skipLengthCheck = characters.Length <= textView.SelectedRange.Length;
                            }
                            if (!skipLengthCheck)
                            {
                                anEvent = null;
                                DebugOutput("*$*$*$*$ TOO LONG - NO MORE CHARACTERS");
                            }
                        }
                    }
                    // DebugOutput("*$*$*$*$ Handler got event: " + anEvent.Type + " with characters: " + characters);
                }
            }
            return anEvent;
        }

        private void CellTextEditStarted(NSNotification notification)
        {
            DebugOutput("++++++++++++++++++++++ CellEdit started");
            var editor = notification.UserInfo.ValueForKey(FieldEditorKey) as NSTextView;
            editor.AutomaticTextReplacementEnabled = false;
            TextStorage = editor.TextStorage;
            if (MaxLength > 0)
            {
                DebugOutput("+!+!+!+! STARTING EDIT ON " + EditingObject + " with length " + MaxLength);
                TextChecker = TextStorage.Delegate as LimitedTextEditing;
                if (TextChecker == null)
                {
                    TextChecker = new LimitedTextEditing(this);
                    TextStorage.Delegate = TextChecker;
                    DebugOutput("+!+!+!+! CREATED TEXTCHECKER DELEGATE for " + EditingObject);
                }
                else
                {
                    DebugOutput("+!+!+!+! REUSED TEXTCHECKER DELEGATE ON " + EditingObject);
                }
                TextChecker.PreviousValidValue = InitialValue;
            }
        }

        private void CellTextChanged(NSNotification notification)
        {
            var editor = notification.UserInfo.ValueForKey(FieldEditorKey) as NSTextView;
            LiveValue = editor.Value;
            DebugOutput("  ^!^!^!^! CELL TEXT CHANGED TO: " + editor.Value);
        }

        private void CellTextEditEnded(NSNotification notification)
        {
            DebugOutput("---------------------- CellEdit ended");
#if ENABLE_INPLACEEDIT_TRACE
            if (notification != null)
            {
                var editor = notification.UserInfo.ValueForKey(FieldEditorKey) as NSTextView;
                var value = editor == null ? "<null editor>" : editor.Value;
                DebugOutput("  -!-!-!-! CellEdit ended: Notification field editor value: " + value);
            }
#endif // ENABLE_INPLACEEDIT_TRACE
#if DO_CLEANUP
            if (EditingEndedObserver != null)
            {
                DebugOutput("  -!-!-!-! CellEdit ended: Removing end edit observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingEndedObserver);
                EditingEndedObserver.Dispose();
                EditingEndedObserver = null;
            }
            if (EditingTextChanged != null)
            {
                DebugOutput(" -!-!-!-! CellEdit ended: Removing edit text changed observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingTextChanged);
                EditingTextChanged.Dispose();
                EditingTextChanged = null;
            }
            if (EditingStartedObserver != null)
            {
                DebugOutput("  -!-!-!-! CellEdit ended: Removing edit started observer");
                NSNotificationCenter.DefaultCenter.RemoveObserver(EditingStartedObserver);
                EditingStartedObserver.Dispose();
                EditingStartedObserver = null;
            }
#endif // DO_CLEANUP
            if (KeyMonitor != null)
            {
                DebugOutput("  -!-!-!-! CellEdit ended: Removing key event monitor");
                NSEvent.RemoveMonitor(KeyMonitor);
                KeyMonitor.Dispose();
                KeyMonitor = null;
            }
            EditingObject = null;
            LiveValue = null;
            TextStorage = null; // do NOT dispose this - it refers to the storage maintained by Owner
            DebugOutput("---------------------- CellEdit ended: Finished");
        }

        /// <summary>
        /// This specialization of NSTextStorageDelegate is used to ensure that only the desired
        /// characters are entered into a field, and also can be used to enforce the length of
        /// the string entered into the field.
        /// </summary>
        private class LimitedTextEditing : NSTextStorageDelegate
        {
            /// <summary>
            /// Gets or sets the previous valid value.
            /// </summary>
            internal string PreviousValidValue { get; set; }

            private TextCellInPlaceEditor Owner { get; set; }

            /// <summary>
            /// Initializes a new instance of the <see cref="INTV.Shared.View.TextCellInPlaceEditor+LimitedTextEditing"/> class.
            /// </summary>
            /// <param name="handle">Unmanaged object handle.</param>
            /// <remarks>This should never really be called. If it is, we've got a zombie object.</remarks>
            public LimitedTextEditing(System.IntPtr handle)
                : base(handle)
            {
                DebugOutput("######## TextStorage edit: Zombies!");
            }

            public LimitedTextEditing(TextCellInPlaceEditor textEditor)
            {
                Owner = textEditor;
            }

            /// <inheritdoc/>
            public override void TextStorageWillProcessEditing(NSNotification notification)
            {
                if (Owner == null || Owner.EditingObject == null)
                {
                    DebugOutput("^^^^^^^^^^^^^^^^^^^^^^ TextStorage edit: " + (Owner == null ? "Zombies!" : "No Editing Object"));
                    return;
                }
                DebugOutput("^^^^^^^^^^^^^^^^^^^^^^ TextStorage edit: will process editing");
                var textStorage = notification.Object as NSTextStorage;
                if (textStorage.ChangeInLength >= 0)
                {
                    bool replaceStorageValue = false;
                    var pendingValue = textStorage.Value;
                    var pendingValueLength = textStorage.Value.Length;
                    if (pendingValue.Length > Owner.MaxLength)
                    {
                        DebugOutput("  ^$^$^$^$ TextStorage edit: NEW STRING TOO LONG");
                        replaceStorageValue = true;
                        var editRange = textStorage.EditedRange;
                        // The following block is disabled because it's causing crashes. Most likely I've gotten
                        // the order of operations wrong in trying to modify the edited range, or there's another
                        // bit of state that needs to be managed. Perhaps a custon NSFormatter would be better, but
                        // attempts in that direction have not gone well. Perhaps the discussion here:
                        // http://stackoverflow.com/questions/827014/how-to-limit-nstextfield-text-length-and-keep-it-always-upper-case
                        // will lead to an improved result.
                        ////var mask = textStorage.EditedMask;
                        ////textStorage.Edited((uint)mask, editRange, -editRange.Length);
                        pendingValue = pendingValue.Remove((int)editRange.Location, (int)editRange.Length);
                    }
                    else if (Owner.IsValidCharacter != null)
                    {
                        var characters = pendingValue.ToCharArray().ToList();
                        if (characters.RemoveAll(c => !Owner.IsValidCharacter(c)) > 0)
                        {
                            DebugOutput("  ^$^$^$^$ TextStorage edit: NEW STRING BAD CHARS");
                            replaceStorageValue = true;
                            pendingValue = new string(characters.ToArray());
                        }
                    }
                    if (replaceStorageValue)
                    {
                        DebugOutput("  ^$^$^$^$ TextStorage edit: Replacing with range 0-" + pendingValueLength + ", new pending value is length: " + pendingValue.Length);
                        textStorage.Replace(new NSRange(0, pendingValueLength), pendingValue);
                    }

                    PreviousValidValue = pendingValue;
                }
                DebugOutput("^^^^^^^^^^^^^^^^^^^^^^ TextStorage edit: finished editing");
            }
        }

        [System.Diagnostics.Conditional("ENABLE_INPLACEEDIT_TRACE")]
        private static void DebugOutput(object message)
        {
            System.Diagnostics.Debug.WriteLine(message);
        }
    }
}
