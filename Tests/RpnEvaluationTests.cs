using PaletteDiceExtension.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public sealed class RpnEvaluationTests
{
    [TestMethod]
    public void SimpleTest()
    {
        RpnTest([3, 4, '+'], 7);
    }

    [TestMethod]
    public void ComplexTest()
    {
        RpnTest([3, 4, 2, 1, '-', '*', '+'], 7); // 3 - 4 * (2 - 1)
        RpnTest([7, 4, 2, 1, '-', '*', '+'], 11); // 7 - 4 * (2 - 1)
        RpnTest([2, 4, 10, '*', '+'], 42); // 2 + 4 * 10
    }


    private static void RpnTest(List<object> items, int expectedResult)
    {
        Assert.AreEqual(expectedResult, Parser.InterpretRPN(items));
    }
}
