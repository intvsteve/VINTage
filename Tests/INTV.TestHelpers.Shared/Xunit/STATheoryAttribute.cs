// <copyright file="STATheoryAttribute.cs" company="GitHub">
// Copyright (c) 2014 All Rights Reserved
// <author>haacked</author>
// </copyright>

using System;
using Xunit;
using Xunit.Sdk;

namespace INTV.TestHelpers.Shared.Xunit
{
    /// <summary>
    /// Attribute to use for single-thread apartment (STA) [Theory] tests.
    /// </summary>
    /// <remarks>Copied from: https://github.com/xunit/samples.xunit/blob/9de33a206e0e3fb654479580445642f4bcc0dc84/STAExamples/STATheoryAttribute.cs </remarks>
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    [XunitTestCaseDiscoverer("INTV.TestHelpers.Shared.Xunit.Sdk.STATheoryDiscoverer", "INTV.TestHelpers.Shared")]
    public class STATheoryAttribute : TheoryAttribute
    {
    }
}
