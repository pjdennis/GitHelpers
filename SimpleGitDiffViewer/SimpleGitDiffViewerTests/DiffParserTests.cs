using System;
using System.Collections.Generic;
using System.IO;
using GitHelpers.Diff;
using NUnit.Framework;

namespace GitHelpers.Tests.Diff
{
    // Git Command: git diff --name-status -M -z origin\master master
    // Resulted in the contents of sample-diff-output.bin

    [TestFixture]
    public class DiffParserTests
    {
        [Test]
        public void TestParse()
        {
            var expected
                = new[]
                          {
                              "Renamed App for Source Control Spikes/App for Source Control Spikes/Program.cs to App for Source Control Spikes/App for Source Control Spikes/Program2.cs",
                              "Renamed README_AGAIN.txt to README_AGAIN_RENAMED.txt",
                              "Added bar.txt",
                              "Deleted foo.txt",
                              "Added meld.bat",
                              "Modified new_file.txt",
                              "Modified new_file2.txt",
                              "Renamed new_file3.txt to new_file3_new_name.txt",
                              "Modified new_new_new/file.txt"
                          };

            var actual = ParseDiffOutput();

            Assert.AreEqual(expected, actual);
        }

        private string[] ParseDiffOutput()
        {
            using (var file = File.OpenRead("../../sample-diff-output.bin"))
            {
                var helper = new Helper();
                DiffParser.Parse(file, helper);
                return helper.Result;
            }
        }

        private class Helper : OperationVisitor
        {
            private readonly List<string> _result = new List<string>();

            public void Visit(SingleFileOperation operation)
            {
                _result.Add(string.Concat(operation.OperationType, " ", operation.FileName));
            }

            public void Visit(TwoFileOperation operation)
            {
                _result.Add(string.Concat(operation.OperationType, " ", operation.FromFileName, " to ", operation.ToFileName));
            }

            public string[] Result { get { return _result.ToArray(); } }
        }
    }
}
