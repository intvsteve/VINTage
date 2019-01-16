// <copyright file="PropertyChangedNotifierTests.cs" company="INTV Funhouse">
// Copyright (c) 2018 All Rights Reserved
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
using INTV.Core.ComponentModel;
using Xunit;

namespace INTV.Core.Tests.ComponentModel
{
    public class PropertyChangedNotifierTests
    {
        [Fact]
        public void INotifyPropertyChanged_DoNotChangeValueOfUpdateProperty_ChangedEventNotCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.UpdatableIntProperty = NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue;

            Assert.False(called);
        }

        [Fact]
        public void INotifyPropertyChanged_ChangeValueOfUpdateProperty_ChangedEventCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.UpdatableIntProperty = NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue + 1;

            Assert.True(called);
        }

        [Fact]
        public void INotifyPropertyChanged_DoNotChangeValueOfPostUpdateActionProperty_ChangedEventNotCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPostUpdateActionValue, notifier.ExecutedPostUpdateAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PostUpdateActionProperty = NotifiesOfPropertyChanges.InitialPostUpdateActionPropertyValue;

            Assert.False(called);
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPostUpdateActionValue, notifier.ExecutedPostUpdateAction);
        }

        [Fact]
        public void INotifyPropertyChanged_ChangeValueOfPostUpdateProperty_ChangedEventCalledAndPostUpdateActionExecuted()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPostUpdateActionValue, notifier.ExecutedPostUpdateAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PostUpdateActionProperty = NotifiesOfPropertyChanges.InitialPostUpdateActionPropertyValue + 1;

            Assert.True(called);
            Assert.Equal(NotifiesOfPropertyChanges.InitialPostUpdateActionPropertyValue + 1, notifier.ExecutedPostUpdateAction);
        }

        [Fact]
        public void INotifyPropertyChanged_DoNotChangeValueOfPreUpdateActionProperty_ChangedEventNotCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPreUpdateActionValue, notifier.ExecutedPreUpdateAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PreUpdateActionProperty = NotifiesOfPropertyChanges.InitialPreUpdateActionPropertyValue;

            Assert.False(called);
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPreUpdateActionValue, notifier.ExecutedPreUpdateAction);
        }

        [Fact]
        public void INotifyPropertyChanged_ChangeValueOfPreUpdateProperty_ChangedEventCalledAndPreUpdateActionExecuted()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.Equal(NotifiesOfPropertyChanges.InitialExecutedPreUpdateActionValue, notifier.ExecutedPreUpdateAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PreUpdateActionProperty = NotifiesOfPropertyChanges.InitialPreUpdateActionPropertyValue + 1;

            Assert.True(called);
            Assert.Equal(NotifiesOfPropertyChanges.InitialPreUpdateActionPropertyValue + 1, notifier.ExecutedPreUpdateAction);
        }

        [Fact]
        public void INotifyPropertyChanged_DoNotChangeValueOfAssignProperty_ChangedEventNotCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.AssignProperty = NotifiesOfPropertyChanges.InitialAssignPropertyValue;

            Assert.False(called);
        }

        [Fact]
        public void INotifyPropertyChanged_ChangeValueOfAssignProperty_ChangedEventCalledAndValueChanged()
        {
            var notifier = new NotifiesOfPropertyChanges();

            var called = 0;
            notifier.PropertyChanged += (s, e) => ++called;
            notifier.AssignProperty = NotifiesOfPropertyChanges.InitialAssignPropertyValue + 1;
            notifier.ChangeMe = null;
            notifier.ChangeMe = "I'm baaaack!";

            Assert.Equal(3, called);
            Assert.Equal(NotifiesOfPropertyChanges.InitialAssignPropertyValue + 1, notifier.AssignProperty);
        }

        [Fact]
        public void INotifyPropertyChanged_DoNotChangeValueOfPostAssignProperty_ChangedEventAndPostAssignActionNotCalled()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.False(notifier.ExecutedPostAssignAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PostAssignActionProperty = NotifiesOfPropertyChanges.InitialPostAssignActionPropertyValue;

            Assert.False(called);
            Assert.False(notifier.ExecutedPostAssignAction);
        }

        [Fact]
        public void INotifyPropertyChanged_ChangeValueOfPostAssignProperty_ChangedEventAndPostgAssignActionCalledAndValueChanged()
        {
            var notifier = new NotifiesOfPropertyChanges();
            Assert.False(notifier.ExecutedPostAssignAction);

            var called = false;
            notifier.PropertyChanged += (s, e) => called = true;
            notifier.PostAssignActionProperty = NotifiesOfPropertyChanges.InitialPostAssignActionPropertyValue + 1;

            Assert.True(called);
            Assert.True(notifier.ExecutedPostAssignAction);
            Assert.Equal(NotifiesOfPropertyChanges.InitialPostAssignActionPropertyValue + 1, notifier.PostAssignActionProperty);
        }

        [Fact]
        public void INotifyPropertyChanged_RaisePropertyChanges_PropertyChangeEventsFire()
        {
            var notifier = new NotifiesOfPropertyChanges();
            var update1 = false;
            var update2 = false;
            var update3 = false;
            var update4 = false;

            notifier.PropertyChanged += (s, e) =>
                {
                    Assert.Equal("UpdatableIntProperty", e.PropertyName);
                    switch (notifier.UpdatableIntProperty)
                    {
                        case NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue + 1:
                            update1 = true;
                            break;
                        case NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue + 2:
                            update2 = true;
                            break;
                        case NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue + 3:
                            update3 = true;
                            break;
                        case NotifiesOfPropertyChanges.InitialUpdatableIntPropertyValue + 4:
                            update4 = true;
                            break;
                        default:
                            Assert.True(false, "Unexpected value for notifier.UpdatableIntProperty: " + notifier.UpdatableIntProperty);
                            break;
                    }
                };

            var postRaiseEvent = false;
            notifier.RaisePropertyChangedEventsForUpdatableIntProperty((_, v) => postRaiseEvent = true);

            Assert.True(update1);
            Assert.True(update2);
            Assert.True(update3);
            Assert.True(update4);
            Assert.True(postRaiseEvent);
        }

        private class NotifiesOfPropertyChanges : PropertyChangedNotifier
        {
            public const int InitialUpdatableIntPropertyValue = 1;
            public const int InitialPostUpdateActionPropertyValue = 2;
            public const int InitialPreUpdateActionPropertyValue = 3;
            public const int InitialAssignPropertyValue = 4;
            public const int InitialPostAssignActionPropertyValue = 5;

            public const int InitialExecutedPostUpdateActionValue = -1;
            public const int InitialExecutedPreUpdateActionValue = -2;
            public const bool InitialExecutedPostAssignActionValue = false;

            public NotifiesOfPropertyChanges()
            {
                _updatableIntProperty = InitialUpdatableIntPropertyValue;
                _postUpdateActionProperty = InitialPostUpdateActionPropertyValue;
                _preUpdateActionProperty = InitialPreUpdateActionPropertyValue;
                _assignProperty = InitialAssignPropertyValue;
                _postAssignActionProperty = InitialPostAssignActionPropertyValue;

                ExecutedPostUpdateAction = InitialExecutedPostUpdateActionValue;
                ExecutedPreUpdateAction = InitialExecutedPreUpdateActionValue;
                ExecutedPostAssignAction = InitialExecutedPostAssignActionValue;

                _changeMe = "boo";
            }

            public int UpdatableIntProperty
            {
                get
                {
                    return _updatableIntProperty;
                }

                set
                {
                    UpdateProperty("UpdatableIntProperty", value, _updatableIntProperty);
                }
            }
            private int _updatableIntProperty;

            public int PostUpdateActionProperty
            {
                get
                {
                    return _postUpdateActionProperty;
                }

                set
                {
                    UpdateProperty("PostUpdateActionProperty", value, _postUpdateActionProperty, (_, v) => ExecutedPostUpdateAction = v);
                }
            }
            private int _postUpdateActionProperty;

            public int ExecutedPostUpdateAction { get; private set; }

            public int PreUpdateActionProperty
            {
                get
                {
                    return _preUpdateActionProperty;
                }

                set
                {
                    UpdateProperty("PreUpdateActionProperty", (_, v) => ExecutedPreUpdateAction = v, value, _preUpdateActionProperty);
                }
            }
            private int _preUpdateActionProperty;

            public int ExecutedPreUpdateAction { get; private set; }

            public int AssignProperty
            {
                get
                {
                    return _assignProperty;
                }

                set
                {
                    AssignAndUpdateProperty("AssignProperty", value, ref _assignProperty);
                }
            }
            private int _assignProperty;

            public int PostAssignActionProperty
            {
                get
                {
                    return _postAssignActionProperty;
                }

                set
                {
                    AssignAndUpdateProperty("PostAssignActionProperty", value, ref _postAssignActionProperty, (_, v) => ExecutedPostAssignAction = true);
                }
            }
            private int _postAssignActionProperty;

            public bool ExecutedPostAssignAction { get; private set; }

            public string ChangeMe
            {
                get { return _changeMe; }
                set { AssignAndUpdateProperty("ChangeMe", value, ref _changeMe); }
            }
            private string _changeMe;

            public void RaisePropertyChangedEventsForUpdatableIntProperty(Action<string, int> postChangeAction)
            {
                ++_updatableIntProperty;
                RaisePropertyChanged("UpdatableIntProperty");

                ++_updatableIntProperty;
                RaisePropertyChanged<int>("UpdatableIntProperty");

                ++_updatableIntProperty;
                RaisePropertyChanged(this, "UpdatableIntProperty");

                ++_updatableIntProperty;
                RaisePropertyChanged("UpdatableIntProperty", postChangeAction, _updatableIntProperty);
            }
        }
    }
}
