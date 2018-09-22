// <copyright file="RelayCommandBaseTests.cs" company="INTV Funhouse">
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
    public class RelayCommandBaseTests
    {
        [Fact]
        public void RelayCommandWithNullExecute_ExecuteWithNullParamter_ThrowsNullReferenceException()
        {
            var command = TestCommand.CreateWithExecute(null);

            Assert.Throws<NullReferenceException>(() => command.Execute(null));
        }

        [Fact]
        public void RelayCommandWithValidExecute_ExecuteWithNullParameter_Executes()
        {
            var executed = false;
            var command = TestCommand.CreateWithExecute(_ => executed = true);

            command.Execute(null);

            Assert.True(executed);
        }

        [Fact]
        public void RelayCommandWithValidExecute_ExecuteWithParameter_Executes()
        {
            var parameter = 5;
            var command = TestCommand.CreateWithExecute(p =>
                {
                    int passedParamter = (int)p;
                    parameter = ++passedParamter;
                });

            command.Execute(parameter);

            Assert.Equal(6, parameter);
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndDefaultCanExecute_CanExecuteWithNullParameter_CanExecuteReturnsTrue()
        {
            var command = TestCommand.CreateWithExecute(null);

            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndDefaultCanExecute_CanExecuteWithParameter_CanExecuteReturnsTrue()
        {
            var command = TestCommand.CreateWithExecute(null);

            Assert.True(command.CanExecute(new object()));
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndNullCanExecute_CanExecuteWithNullParamter_CanExecuteReturnsTrue()
        {
            var command = TestCommand.CreateWithExecuteAndCanExecute(null, null);

            Assert.True(command.CanExecute(null));
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndNullCanExecute_CanExecuteWithParameter_CanExecuteReturnsTrue()
        {
            var command = TestCommand.CreateWithExecuteAndCanExecute(null, null);

            Assert.True(command.CanExecute("howdy"));
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndValidCanExecute_CanExecuteWithNullParamter_CanExecuteReturnsExpectedValue()
        {
            var expectedCanExecuteResult = true;
            var command = TestCommand.CreateWithExecuteAndCanExecute(
                onExecute: null,
                canExecute: _ =>
                {
                    expectedCanExecuteResult = false;
                    return expectedCanExecuteResult;
                });

            Assert.Equal(false, command.CanExecute(null));
            Assert.False(expectedCanExecuteResult);
        }

        [Fact]
        public void RelayCommandWithNullExecuteAndValidCanExecute_CanExecuteWithParameter_CanExecuteReturnsReturnsExpectedValue()
        {
            var canExecuteParameter = -1;
            var command = TestCommand.CreateWithExecuteAndCanExecute(
                onExecute: null,
                canExecute: p =>
                {
                    var parameter = (int)p;
                    var canExecute = parameter > 0;
                    canExecuteParameter = 9;
                    return canExecute;
                });

            Assert.False(command.CanExecute(canExecuteParameter));
            Assert.Equal(9, canExecuteParameter);
        }

        [Fact]
        public void RelayCommandWithParameter_ExecuteWithNullParameter_UsesOriginalParameter()
        {
            var parameter = new object();
            var command = TestCommand.CreateWithExecuteAndCanExecuteAndParameterData(
                onExecute: p => Assert.True(object.ReferenceEquals(parameter, p)),
                canExecute: null,
                parameter: parameter);

            command.Execute(null);
        }

        [Fact]
        public void RelayCommandWithParameter_CanExecuteWithNullParameter_UsesOriginalParameter()
        {
            var parameter = new object();
            var command = TestCommand.CreateWithExecuteAndCanExecuteAndParameterData(
                onExecute: null,
                canExecute: p =>
                    {
                        Assert.True(object.ReferenceEquals(parameter, p));
                        return false;
                    },
                parameter: parameter);

            Assert.False(command.CanExecute(null));
        }

        [Fact]
        public void RelayCommandWithParameter_ExecuteWithNonNullParameter_UsesProvidedParameter()
        {
            var parameter = new object();
            var executeParameter = new object();
            var command = TestCommand.CreateWithExecuteAndCanExecuteAndParameterData(
                p => Assert.True(object.ReferenceEquals(executeParameter, p)),
                canExecute: null,
                parameter: parameter);

            command.Execute(executeParameter);
        }

        [Fact]
        public void RelayCommandWithParameter_CanExecuteWithNonNullParameter_UsesProvidedParameter()
        {
            var parameter = new object();
            var canExecuteParameter = new object();
            var command = TestCommand.CreateWithExecuteAndCanExecuteAndParameterData(
                onExecute: null,
                canExecute: p =>
                    {
                        Assert.True(object.ReferenceEquals(canExecuteParameter, p));
                        return false;
                    },
                parameter: parameter);

            Assert.False(command.CanExecute(canExecuteParameter));
        }

        [Fact]
        public void RelayCommand_Clone_ValuesCloneProperly()
        {
            var parameter = "Lt. Cmdr. Data";
            var command = TestCommand.CreateWithExecuteAndCanExecuteAndParameterData(null, null, parameter);
            Assert.Equal(typeof(string), command.PreferredParameterType);

            var copy = TestCommand.Clone(command);

            Assert.Equal(command.UniqueId, copy.UniqueId);
            Assert.Equal(command.PreferredParameterType, copy.PreferredParameterType);
        }

        private class TestCommand : RelayCommandBase
        {
            private TestCommand(Action<object> onExecute)
                : base(onExecute)
            {
            }

            private TestCommand(Action<object> onExecute, Func<object, bool> canExecute)
                : base(onExecute, canExecute)
            {
            }

            private TestCommand(Action<object> onExecute, Func<object, bool> canExecute, object parameter)
                : base(onExecute, canExecute, parameter)
            {
            }

            private TestCommand(TestCommand toClone)
                : base(toClone)
            {
            }

            public static TestCommand CreateWithExecute(Action<object> onExecute, [System.Runtime.CompilerServices.CallerMemberName] string uniqueId = "")
            {
                return new TestCommand(onExecute);
            }

            public static TestCommand CreateWithExecuteAndCanExecute(Action<object> onExecute, Func<object, bool> canExecute, [System.Runtime.CompilerServices.CallerMemberName] string uniqueId = "")
            {
                return new TestCommand(onExecute, canExecute);
            }

            public static TestCommand CreateWithExecuteAndCanExecuteAndParameterData(Action<object> onExecute, Func<object, bool> canExecute, object parameter, [System.Runtime.CompilerServices.CallerMemberName] string uniqueId = "")
            {
                var command = new TestCommand(onExecute, canExecute, parameter) { UniqueId = uniqueId };
                if (parameter != null)
                {
                    command.PreferredParameterType = parameter.GetType();
                }
                return command;
            }

            public static TestCommand Clone(TestCommand toClone)
            {
                return new TestCommand(toClone);
            }

            public void Execute(object parameter)
            {
                OnExecute(parameter);
            }

            public bool CanExecute(object parameter)
            {
                return OnCanExecute(parameter);
            }
        }
    }
}
