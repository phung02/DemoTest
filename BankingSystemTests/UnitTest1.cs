/*To set up the test project:
1.In Visual Studio, right-click on the solution and select Add > New Project.
2.	Choose Unit Test Project (.NET Framework) and name it BankingSystemTests.
3.	Add a reference to the ConsoleApp1 project in the test project.
4.	Add the above test class to the BankingSystemTests project.
This test method will verify that the BankingSystem class processes the provided queries correctly and produces the expected results.
*/

using Microsoft.VisualStudio.TestTools.UnitTesting;
using ConsoleApp1; // Ensure this namespace matches the namespace of your BankingSystem class
using System.Collections.Generic;

namespace BankingSystemTests
{
    [TestClass]
    public class BankingSystemTests
    {
        [TestMethod]
        public void TestBankingSystem()
        {
            var queries = new string[][]
            {
                new[] { "CREATE_ACCOUNT", "1", "account1" }                
            };

            var expectedResults = new List<string>
            {
                "true",               
            };
            var results = Program.Solution(queries);
            CollectionAssert.AreEqual(expectedResults, results);
        }

        [TestMethod]
        public void TestAddMethod()
        {
            Assert.AreEqual(11, Program.Add(4,7));
        }

        [TestMethod]
        public void TestConcatStringMethod()
        {
            string result1 = Program.ConcatString("hello", "world");
            Assert.AreEqual("hello world", result1);

            string result2 = Program.ConcatString("foo", "bar");
            Assert.AreEqual("foo bar", result2);

            string result3 = Program.ConcatString("C#", "Programming");
            Assert.AreEqual("C# Programming", result3);

            string result4 = Program.ConcatString("Unit", "Test");
            Assert.AreEqual("Unit Test", result4);
        }

        [TestMethod]
        public void TestLetterExitsMethod()
        {
            bool result1 = Program.LetterExits("hello", "e");
            Assert.IsTrue(result1);

            bool result2 = Program.LetterExits("hello", "a");
            Assert.IsFalse(result2);

            bool result3 = Program.LetterExits("world", "o");
            Assert.IsTrue(result3);

            bool result4 = Program.LetterExits("world", "z");
            Assert.IsFalse(result4);
        }
    }
}
