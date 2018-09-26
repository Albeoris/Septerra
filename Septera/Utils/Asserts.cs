using System;
using System.IO;

namespace Septera
{
    public static class Asserts
    {
        public static T Expected<T>(T givenValue, T expectedValue) where T : IComparable<T>
        {
            if (givenValue.CompareTo(expectedValue) != 0)
                throw new InvalidDataException($"The unexpected value [{givenValue}] has occurred. Expected: [{expectedValue}]");
            return givenValue;
        }

        public static T Positive<T>(T givenValue) where T : IComparable<Int32>
        {
            if (givenValue.CompareTo(0) <= 0)
                throw new InvalidDataException($"The unexpected value [{givenValue}] has occurred. Expected positive value.");
            return givenValue;
        }

        public static T InRange<T>(T givenValue, Int32 minValue, Int32 maxValue) where T : IComparable<Int32>
        {
            if (givenValue.CompareTo(minValue) < 0)
                goto @throw;

            if (givenValue.CompareTo(maxValue) > 0)
                goto @throw;

            return givenValue;

            @throw:
            throw new InvalidDataException($"The unexpected value [{givenValue}] has occurred. Expected value in range [{minValue}...{maxValue}].");
        }
    }
}