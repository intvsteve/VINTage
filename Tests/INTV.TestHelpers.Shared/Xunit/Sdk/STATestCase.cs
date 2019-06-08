﻿// <copyright file="STATestCase.cs" company="GitHub">
// Copyright (c) 2015 All Rights Reserved
// <author>haacked, bradwilson, AArnott</author>
// </copyright>

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace INTV.TestHelpers.Shared.Xunit.Sdk
{
    /// <summary>
    /// Wraps test cases for Xunit [Fact] and [Theory] to execute the test on a single-thread apartment (STA) thread.
    /// </summary>
    /// <remarks>Copied from: https://github.com/xunit/samples.xunit/blob/9de33a206e0e3fb654479580445642f4bcc0dc84/STAExamples/STATestCase.cs
    /// Modified based on code here: https://github.com/xunit/samples.xunit/blob/master/STAExamples/WpfTestCase.cs </remarks>
    [Serializable]
    [DebuggerDisplay(@"\{ class = {TestMethod.TestClass.Class.Name}, method = {TestMethod.Method.Name}, display = {DisplayName}, skip = {SkipReason} \}")]
    public class STATestCase : LongLivedMarshalByRefObject, IXunitTestCase
    {
        /// <summary>
        /// Creates a new test case that wraps the given test case.
        /// </summary>
        /// <param name="testCase">The test case to wrap.</param>
        public STATestCase(IXunitTestCase testCase)
        {
            TestCase = testCase;
        }

        /// <summary>
        /// Initializes a new instance via the deserializer.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Called by the deserializer", error: true)]
        public STATestCase()
        {
        }

        #region ITestCase

        /// <inheritdoc />
        public string DisplayName
        {
            get { return TestCase.DisplayName; }
        }

        /// <inheritdoc />
        public string SkipReason
        {
            get { return TestCase.SkipReason; }
        }

        /// <inheritdoc />
        public ISourceInformation SourceInformation
        {
            get { return TestCase.SourceInformation; }
            set { TestCase.SourceInformation = value; }
        }

        /// <inheritdoc />
        public ITestMethod TestMethod
        {
            get { return TestCase.TestMethod; }
        }

        /// <inheritdoc />
        public object[] TestMethodArguments
        {
            get { return TestCase.TestMethodArguments; }
        }

        /// <inheritdoc />
        public Dictionary<string, List<string>> Traits
        {
            get { return TestCase.Traits; }
        }

        /// <inheritdoc />
        public string UniqueID
        {
            get { return TestCase.UniqueID; }
        }

        #endregion // ITestCase

        #region IXunitTestCase

        /// <inheritdoc />
        public IMethodInfo Method
        {
            get { return TestCase.Method; }
        }

        #endregion // IXunitTestCase

        private IXunitTestCase TestCase { get; set; }

        #region IXunitTestCase

        /// <inheritdoc />
        /// <remarks>If a different case requiring a dispatcher arises, look here:
        /// https://github.com/xunit/samples.xunit/blob/master/STAExamples/WpfTestCase.cs </remarks>
        public Task<RunSummary> RunAsync(IMessageSink diagnosticMessageSink, IMessageBus messageBus, object[] constructorArguments, ExceptionAggregator aggregator, CancellationTokenSource cancellationTokenSource)
        {
            var taskCompletionSource = new TaskCompletionSource<RunSummary>();

            var thread = new Thread(() =>
            {
                try
                {
                    var testCaseTask = TestCase.RunAsync(diagnosticMessageSink, messageBus, constructorArguments, aggregator, cancellationTokenSource);
                    taskCompletionSource.SetResult(testCaseTask.Result);
                }
                catch (Exception e)
                {
                    taskCompletionSource.SetException(e);
                }
            });

            thread.SetApartmentState(ApartmentState.STA);
            thread.Start();

            return taskCompletionSource.Task;
        }

        #endregion // IXunitTestCase

        #region IXunitSerializable

        /// <inheritdoc />
        public void Deserialize(IXunitSerializationInfo info)
        {
            TestCase = info.GetValue<IXunitTestCase>("InnerTestCase");
        }

        /// <inheritdoc />
        public void Serialize(IXunitSerializationInfo info)
        {
            info.AddValue("InnerTestCase", TestCase);
        }

        #endregion // IXunitSerializable
    }
}
