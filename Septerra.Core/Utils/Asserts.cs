using System;
using System.IO;

namespace Septerra.Core
{
    public static class Asserts
    {
        public static T NotNull<T>(T givenValue) where T : class
        {
            if (givenValue is null)
                throw new InvalidDataException($"The unexpected [null] has occurred.");
            return givenValue;
        }

        public static T[] HasElements<T>(T[] givenValue, Int32 minCount)
        {
            NotNull(givenValue);

            if (givenValue.Length < minCount)
                throw new InvalidDataException($"The unexpected number of elements [{givenValue.Length}] has occurred. MinCount: [{minCount}]");

            return givenValue;
        }

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

        public static T InRange<T>(T givenValue, UInt32 minValue, UInt32 maxValue) where T : IComparable<UInt32>
        {
            if (givenValue.CompareTo(minValue) < 0)
                goto @throw;

            if (givenValue.CompareTo(maxValue) > 0)
                goto @throw;

            return givenValue;

            @throw:
            throw new InvalidDataException($"The unexpected value [{givenValue}] has occurred. Expected value in range [{minValue}...{maxValue}].");
        }
        
        public static String FileExists(String filePath)
        {
            if (!File.Exists(filePath))
                throw new FileNotFoundException(filePath);

            return filePath;
        }

        public static String DirectoryExists(String directoryPath)
        {
            if (!Directory.Exists(directoryPath))
                throw new DirectoryNotFoundException(directoryPath);

            return directoryPath;
        }
    }
}