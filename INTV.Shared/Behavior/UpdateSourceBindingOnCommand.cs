// <copyright file="UpdateSourceBindingOnCommand.cs" company="INTV Funhouse">
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

using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace INTV.Shared.Behavior
{
    /// <summary>
    /// Implements an attached behavior that will force the source of a binding to update when a command executes.
    /// A typical scenario in which this is needed is when executing a 'commit' command on the 'ENTER' key of
    /// a single-line control. The 'ENTER' key will change the value, but the 'value changed' event is usually
    /// not emitted until the underlying TextControl loses focus.
    /// </summary>
    public static class UpdateSourceBindingOnCommand
    {
        #region UpdateSourceOnCommandProperty

        /// <summary>
        /// This attached property stores an attached property to update when a command executes.
        /// It uses the CommandManager to add proxy CanExecute and Execute handlers that will
        /// be called when the control's command fires. It will work for controls that implement
        /// the ICommandSource interface.
        /// </summary>
        public static readonly DependencyProperty UpdateSourceOnCommandProperty = DependencyProperty.RegisterAttached("UpdateSourceOnCommand", typeof(DependencyProperty), typeof(UpdateSourceBindingOnCommand), new PropertyMetadata(UpdateSourceOnCommandPropertyChangedCallBack));

        /// <summary>
        /// Property setter for the UpdateSourceOnCommand attached property.
        /// </summary>
        /// <param name="visual">The visual that upon which to store the property.</param>
        /// <param name="property">The attached property to update when the command executes.</param>
        public static void SetUpdateSourceOnCommand(this UIElement visual, DependencyProperty property)
        {
            visual.SetValue(UpdateSourceOnCommandProperty, property);
        }

        /// <summary>
        /// Property getter for the UpdateSourceOnCommand attached property.
        /// </summary>
        /// <param name="visual">The visual from which to retrieve the property.</param>
        /// <returns>The value of the property.</returns>
        private static DependencyProperty GetUpdateSourceOnCommand(this UIElement visual)
        {
            return visual.GetValue(UpdateSourceOnCommandProperty) as DependencyProperty;
        }

        #endregion // UpdateSourceOnCommandProperty

        private static readonly DependencyProperty CommitPhaseProperty = DependencyProperty.RegisterAttached("CommitPhase", typeof(CommitPhase), typeof(UpdateSourceBindingOnCommand));

        private static void SetCommitPhase(this UIElement visual, CommitPhase commitPhase)
        {
            visual.SetValue(CommitPhaseProperty, commitPhase);
        }

        private static CommitPhase GetCommitPhase(this UIElement visual)
        {
            return (CommitPhase)visual.GetValue(CommitPhaseProperty);
        }

        private static void UpdateSourceOnCommandPropertyChangedCallBack(DependencyObject o, DependencyPropertyChangedEventArgs e)
        {
            if (o is ICommandSource)
            {
                var control = o as UIElement;
                if (control != null)
                {
                    CommandManager.RemoveExecutedHandler(control, OnCommandExecuted);
                    CommandManager.RemoveCanExecuteHandler(control, CanExecuteRoutedEventHandler);
                    CommandManager.AddCanExecuteHandler(control, CanExecuteRoutedEventHandler);
                    CommandManager.AddExecutedHandler(control, OnCommandExecuted);
                    control.PreviewKeyDown += HandlePreviewKeyDown;
                    control.PreviewTextInput += HandlePreviewTextInput;
                    control.IsKeyboardFocusWithinChanged += HandleIsKeyboardFocusWithinChanged;
                }
            }
        }

        private static void HandlePreviewKeyDown(object sender, KeyEventArgs e)
        {
            var control = sender as UIElement;
            if (control != null)
            {
                if ((e.Key == Key.Escape) || (e.Key == Key.Cancel))
                {
                    SetCommitPhase(control, CommitPhase.Cancel);
                }
                else if (e.Key == Key.Tab)
                {
                    SetCommitPhase(control, Keyboard.Modifiers.HasFlag(ModifierKeys.Shift) ? CommitPhase.CommitPrevious : CommitPhase.CommitNext);
                }
            }
        }

        private static void HandlePreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            var control = sender as UIElement;
            if (INTV.Shared.View.TextBlockEditorAdorner.GetRestrictToGromCharacters(control))
            {
                var updateText = true;
                foreach (var c in e.Text)
                {
                    updateText = INTV.Core.Model.Grom.Characters.Contains(c);
                    if (!updateText)
                    {
                        break;
                    }
                }
                if (!updateText)
                {
                    e.Handled = true; // do not allow unsupported (non-GROM) character(s)
                }
            }
        }

        private static void HandleIsKeyboardFocusWithinChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var control = sender as FrameworkElement;
            var commandSource = sender as ICommandSource;
            if ((control != null) && !(bool)e.OldValue && (bool)e.NewValue)
            {
                SetCommitPhase(control, CommitPhase.None);
            }
            else if ((commandSource != null) && (bool)e.OldValue && !(bool)e.NewValue)
            {
                OnCommandExecuted(sender, null);
            }
        }

        private static void OnCommandExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            var commandSource = sender as ICommandSource;
            if (commandSource != null)
            {
                // First, update the source.
                var control = sender as FrameworkElement;
                if (control != null)
                {
                    var commitPhase = GetCommitPhase(control);
                    if (commitPhase != CommitPhase.DoingCommit)
                    {
                        SetCommitPhase(control, CommitPhase.DoingCommit);
                        var command = commandSource.Command;
                        var parameter = commandSource.CommandParameter;
                        var target = commandSource.CommandTarget;
                        if (command.CanExecute(parameter))
                        {
                            var bindingExpression = control.GetBindingExpression(control.GetUpdateSourceOnCommand());
                            if (bindingExpression != null)
                            {
                                if (commitPhase == CommitPhase.Cancel)
                                {
                                    bindingExpression.UpdateTarget();
                                }
                                else
                                {
                                    bindingExpression.UpdateSource();
                                }
                            }
                            if (control.IsFocused)
                            {
                                if (commitPhase == CommitPhase.CommitNext)
                                {
                                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Next));
                                }
                                else if (commitPhase == CommitPhase.CommitPrevious)
                                {
                                    control.MoveFocus(new TraversalRequest(FocusNavigationDirection.Previous));
                                }
                                else
                                {
                                    Keyboard.Focus(null);
                                }
                            }

                            // Now, execute the command.
                            if (commitPhase == CommitPhase.None)
                            {
                                command.Execute(parameter);
                            }
                        }
                    }
                }
            }
        }

        private static void CanExecuteRoutedEventHandler(object sender, CanExecuteRoutedEventArgs e)
        {
            var commandSource = sender as ICommandSource;
            if (commandSource != null)
            {
                var command = commandSource.Command;
                var parameter = commandSource.CommandParameter;
                var target = commandSource.CommandTarget;
                e.CanExecute = command.CanExecute(parameter);
            }
        }

        /// <summary>
        /// The phases of a command that updates a source binding when executed.
        /// </summary>
        private enum CommitPhase
        {
            /// <summary>
            /// No edit operation.
            /// </summary>
            None,

            /// <summary>
            /// Cancel the edit.
            /// </summary>
            Cancel,

            /// <summary>
            /// Commit the edit.
            /// </summary>
            Commit,

            /// <summary>
            /// Commit edit and move focus to next visual in tab order.
            /// </summary>
            CommitNext,

            /// <summary>
            /// Commit edit and move focus to previous visual in tab order.
            /// </summary>
            CommitPrevious,

            /// <summary>
            /// A commit operation has started.
            /// </summary>
            DoingCommit
        }
    }
}
