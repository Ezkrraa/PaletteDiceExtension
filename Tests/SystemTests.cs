using PaletteDiceExtension.Parser;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests
{
    [TestClass]
    public sealed class SystemTests
    {
        [TestMethod]
        public void TestParser()
        {
            int? result = Parser.ParseExpression("2 * 3");
            Assert.AreEqual(6, result);
            result = Parser.ParseExpression("2+3");
            Assert.AreEqual(5, result);
            result = Parser.ParseExpression("1D2");
            Assert.IsTrue(result >= 1 && result <= 2);

            result = Parser.ParseExpression("(1+1)D1");
            Assert.AreEqual(2, result);

            result = Parser.ParseExpression("100D1");
            Assert.AreEqual(100, result);
            result = Parser.ParseExpression("10D10");
            Assert.IsTrue(result >= 10 && result <= 100);
            for (int i = 0; i < 1_000; i++)
            {
                result = Parser.ParseExpression("2D4");
                Assert.IsTrue(result >= 2 && result <= 8);
                result = Parser.ParseExpression("2D4+8");
                Assert.IsTrue(result >= 10 && result <= 16);
            }
        }
    }
}
