﻿//---------------------------------------------------------------------
// <copyright file="ODataBatchReaderStreamTests.cs" company="Microsoft">
//      Copyright (C) Microsoft Corporation. All rights reserved. See License.txt in the project root for license information.
// </copyright>
//---------------------------------------------------------------------

namespace Microsoft.Test.OData.TDD.Tests.Reader
{
    using System;
    using System.IO;
    using System.Text;
    using FluentAssertions;
    using Microsoft.OData.Core;
    using Microsoft.VisualStudio.TestTools.UnitTesting;
    using ErrorStrings = Microsoft.OData.Core.Strings;

    [TestClass]
    public class ODataBatchReaderStreamTests
    {
        [TestMethod]
        public void ReadFirstNonEmptyLineShouldThrowOnEndOfInput()
        {
            Action readAtEndOfInput = () => CreateBatchReaderStream("").ReadFirstNonEmptyLine();
            readAtEndOfInput.ShouldThrow<ODataException>().WithMessage(ErrorStrings.ODataBatchReaderStream_UnexpectedEndOfInput);
        }

        [TestMethod]
        public void ReadFirstNonEmptyLineShouldSkipEmptyLines()
        {
            const string input = @"


First non-empty line";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [TestMethod]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCRLF()
        {
            const string input = "First non-empty line\r\n";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [TestMethod]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingLF()
        {
            const string input = "First non-empty line\n";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [TestMethod]
        public void ReadFirstNonEmptyLineShouldNotIncludeTrailingCR()
        {
            const string input = "First non-empty line\r";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        [TestMethod]
        public void ReadFirstNonEmptyLineShouldNotIncludeSecondLine()
        {
            const string input = @"First non-empty line
Second line";
            CreateBatchReaderStream(input).ReadFirstNonEmptyLine().Should().Be("First non-empty line");
        }

        private static ODataBatchReaderStream CreateBatchReaderStream(string inputString)
        {
            var underlyingStream = new MemoryStream(Encoding.UTF8.GetBytes(inputString));
            var inputContext = new ODataRawInputContext(
                ODataFormat.Batch, 
                underlyingStream, 
                Encoding.UTF8, 
                new ODataMessageReaderSettings(),
                false, 
                true, 
                null, 
                null, 
                ODataPayloadKind.Batch);
            var batchStream = new ODataBatchReaderStream(inputContext, "batch_862fb28e-dc50-4af1-aad5-9608647761d1", Encoding.UTF8);
            return batchStream;
        }
    }
}
