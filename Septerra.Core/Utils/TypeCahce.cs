using System;

namespace Septerra.Core
{
    public static class TypeCahce<T>
    {
        public static readonly Type Type = typeof(T);
    }
}