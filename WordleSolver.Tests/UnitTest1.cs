using NUnit.Framework;

namespace WordleSolver.Tests
{
    public class WordTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        [TestCase("ABCDE", "FGHIJ", "00000")]
        [TestCase("ABCDE", "FGHIA", "10000")]
        [TestCase("ABCDE", "EDCBA", "11211")]
        [TestCase("ABCDE", "AAAAA", "20000")]
        [TestCase("ABCDE", "EEEEE", "00002")]
        [TestCase("AABBB", "BABAB", "12212")]
        [TestCase("AABBB", "ABABA", "21120")]
        [TestCase("AACCC", "BABAB", "12000")]
        [TestCase("AACCC", "ABABA", "21000")]
        public void CheckAnswer(string guess, string solution, string answer)
        {
            Assert.IsTrue(Word.CheckAnswer(guess, solution, answer));
        }
    }
}