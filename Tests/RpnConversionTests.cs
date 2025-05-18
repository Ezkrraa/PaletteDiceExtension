using PaletteDiceExtension.Parser;
using System.Diagnostics;
using System.Linq.Expressions;

namespace Tests;

[TestClass]
public sealed class RpnConversionTests
{
    [TestMethod]
    public void TestSimple()
    {
        TestEqual([2, '*', 3], [2, 3, '*']);
    }

    [TestMethod]
    public void TestMultiArg()
    {
        TestEqual([2, '+', 4, '*', 10], [2, 4, 10, '*', '+']);
    }

    [TestMethod]
    public void TestParentheses()
    {
        TestEqual([3, '+', 4, '*', '(', 2, '-', 1, ')'], [3, 4, 2, 1, '-', '*', '+']);
    }

    private static void TestEqual(List<object> input, List<object> expectedOutput)
    {
        List<object>? output = Parser.INToRPN(input);

        CollectionAssert.AreEqual(expectedOutput, output);
    }
}
