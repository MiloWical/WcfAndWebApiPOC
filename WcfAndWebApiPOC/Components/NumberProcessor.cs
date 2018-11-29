using System;

namespace Components
{
    public class NumberProcessor : INumberProcessor
    {
        public int Sum(int[] values)
        {
            var sum = 0;

            for (var i = 0; i < values.Length; i++)
                sum += values[i];

            return sum;
        }

        public int Product(int[] values)
        {
            var product = 1;

            for (var i = 0; i < values.Length; i++)
                product *= values[i];

            return product;
        }

        public int AbsoluteValue(int value)
        {
            return Math.Abs(value);
        }
    }
}
