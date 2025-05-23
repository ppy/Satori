// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.

using System.IO;
using System.Reflection;
using System.Reflection.Emit;
using System.Linq;
using Microsoft.DotNet.RemoteExecutor;
using Xunit;

namespace System.Diagnostics.TraceSourceTests
{
    public class DefaultTraceListenerClassTests : FileCleanupTestBase
    {
        private class TestDefaultTraceListener : DefaultTraceListener
        {
            private StringWriter _writer;
            public bool ShouldOverrideWriteLine { get; set; } = true;

            public TestDefaultTraceListener()
            {
                _writer = new StringWriter();
                AssertUiEnabled = false;
            }

            public string Output
            {
                get { return _writer.ToString(); }
            }

            public override void Write(string message)
            {
                _writer.Write(message);
            }

            public override void WriteLine(string message)
            {
                if (ShouldOverrideWriteLine)
                {
                    _writer.WriteLine(message);
                }
                else
                {
                    base.WriteLine(message);
                }
            }
        }

        [Fact]
        public void Constructor()
        {
            var listener = new DefaultTraceListener();
            Assert.Equal("Default", listener.Name);
        }

        [Fact]
        public void Fail()
        {
            var listener = new TestDefaultTraceListener();
            listener.Fail("FAIL");
            Assert.Contains("FAIL", listener.Output);
        }

        [Fact]
        public void Fail_WithoutWriteLineOverride()
        {
            var listener = new TestDefaultTraceListener();
            listener.ShouldOverrideWriteLine = false;
            listener.Fail("FAIL");
            Assert.DoesNotContain("FAIL", listener.Output);
        }

        [Fact]
        public void Fail_WithLogFile()
        {
            var listener = new TestDefaultTraceListener();
            string pathToLogFile = GetTestFilePath();

            listener.LogFileName = pathToLogFile;
            listener.ShouldOverrideWriteLine = false;
            listener.Fail("FAIL");

            Assert.True(File.Exists(pathToLogFile));
            Assert.Contains("FAIL", File.ReadAllText(pathToLogFile));
        }

        [Fact]
        public void Fail_LogFileDirectoryNotFound()
        {
            // Exception should be handled by DefaultTraceListener.WriteLine so no need to assert.
            var listener = new TestDefaultTraceListener();
            listener.LogFileName = $"{Guid.NewGuid:N}\\LogFile.txt";

            listener.ShouldOverrideWriteLine = false;
            listener.Fail("FAIL");
        }

        [Fact]
        public void TraceEvent()
        {
            var listener = new TestDefaultTraceListener();
            listener.TraceEvent(new TraceEventCache(), "Test", TraceEventType.Critical, 1);
            Assert.Contains("Test", listener.Output);
        }

        [Fact]
        public void WriteNull()
        {
            var listener = new DefaultTraceListener();
            // implied assert, does not throw
            listener.Write(null);
        }

        [Fact]
        public void WriteLongMessage()
        {
            var listener = new DefaultTraceListener();
            var longString = new string('a', 0x40000);
            listener.Write(longString);
            // nothing to assert, the output is written to Debug.Write
            // this simply provides code-coverage
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public void AssertUiEnabledPropertyTest(bool expectedAssertUiEnabled)
        {
            var listener = new DefaultTraceListener() { AssertUiEnabled = expectedAssertUiEnabled };
            Assert.Equal(expectedAssertUiEnabled, listener.AssertUiEnabled);
        }

        [Theory]
        [InlineData("LogFile")]
        public void LogFileNamePropertyTest(string expectedLogFileName)
        {
            var listener = new DefaultTraceListener();

            //it should be initialized with the default
            Assert.Equal(string.Empty, listener.LogFileName);
            listener.LogFileName = expectedLogFileName;
            Assert.Equal(expectedLogFileName, listener.LogFileName);
        }

        [ConditionalFact(typeof(RemoteExecutor), nameof(RemoteExecutor.IsSupported))]
        public void EntryAssemblyName_Default_IncludedInTrace()
        {
            RemoteExecutor.Invoke(() =>
            {
                var listener = new TestDefaultTraceListener();
                Trace.Listeners.Add(listener);
                Trace.TraceError("hello world");
                Assert.Equal(Assembly.GetEntryAssembly()?.GetName().Name + " Error: 0 : hello world", listener.Output.Trim());
            }).Dispose();
        }

        [ConditionalFact(typeof(RemoteExecutor), nameof(RemoteExecutor.IsSupported))]
        public void EntryAssemblyName_Null_NotIncludedInTrace()
        {
            RemoteExecutor.Invoke(() =>
            {
                MakeAssemblyGetEntryAssemblyReturnNull();

                var listener = new TestDefaultTraceListener();
                Trace.Listeners.Add(listener);
                Trace.TraceError("hello world");
                Assert.Equal("Error: 0 : hello world", listener.Output.Trim());
            }).Dispose();
        }

        /// <summary>
        /// Makes Assembly.GetEntryAssembly() return null using private reflection.
        /// </summary>
        private static void MakeAssemblyGetEntryAssemblyReturnNull()
        {
            typeof(Assembly)
                .GetMethod("SetEntryAssembly", BindingFlags.Public | BindingFlags.Static)
                .Invoke(null, new object[]{ null });

            Assert.Null(Assembly.GetEntryAssembly());
        }
    }
}
