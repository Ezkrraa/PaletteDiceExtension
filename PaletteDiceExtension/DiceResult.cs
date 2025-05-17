using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaletteDiceExtension;
internal class DiceResult
{
    private static readonly Random r = new();
    internal int Range { get; set; }
    internal int Quantity { get; set; }
    internal int Result { get; set; }
    internal DiceResult(int range, int quantity)
    {
        Result = 0;
        for (int i = 0; i < quantity; i++)
        {
            Result += r.Next(range) + 1;
        }
        this.Range = range;
        this.Quantity = quantity;
    }

    public override string ToString()
    {
        return $"Range: {Range}, Quantity: {Quantity}, Result: {Result}";
    }
}
