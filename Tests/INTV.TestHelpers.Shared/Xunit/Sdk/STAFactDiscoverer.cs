// <copyright file="STAFactDiscoverer.cs" company="N/A">
// Copyright (c) 2015 All Rights Reserved
// <author>AArnott</author>
// </copyright>

using System.Collections.Generic;
using System.Linq;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace INTV.TestHelpers.Shared.Xunit.Sdk
{
    /// <summary>
    /// Discoverer for the single-thread apartment fact attribute (STAFactAttribute).
    /// </summary>
    /// <remarks>Barely modified version of AArnott's version here:
    /// https://github.com/xunit/samples.xunit/blob/master/STAExamples/WpfFactDiscoverer.cs
    /// </remarks>
    public class STAFactDiscoverer : IXunitTestCaseDiscoverer
    {
        private readonly FactDiscoverer _factDiscoverer;

        /// <summary>
        /// Initializes a new instance of <see cref="STAFactDiscoverer"/>.
        /// </summary>
        /// <param name="diagnosticMessageSink">The diagnostic message sink for the discoverer.</param>
        public STAFactDiscoverer(IMessageSink diagnosticMessageSink)
        {
            _factDiscoverer = new FactDiscoverer(diagnosticMessageSink);
        }

        /// <inheritdoc/>
        public IEnumerable<IXunitTestCase> Discover(ITestFrameworkDiscoveryOptions discoveryOptions, ITestMethod testMethod, IAttributeInfo factAttribute)
        {
            return _factDiscoverer.Discover(discoveryOptions, testMethod, factAttribute).Select(t => new STATestCase(t));
        }
    }
}
