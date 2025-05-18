using PaletteDiceExtension.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests;

[TestClass]
public sealed class TokenizerTests
{
    [TestMethod]
    public void TestTokenizer()
    {
        Test("2*3", [2, '*', 3]);
        Test("2*3*(4D2)", [2, '*', 3, '*', '(', 4, 'D', 2, ')']);
    }

    private void Test(string input, List<object>? expectedResult)
    {
        List<object>? result = Parser.Tokenizer(input);
        CollectionAssert.AreEqual(expectedResult, result);
    }
}
