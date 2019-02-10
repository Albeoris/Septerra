using System;

namespace Septerra.Core.DB
{
    public struct DbCachedDataId
    {
        public readonly Int32 Value;

        public DbCachedDataId(Int32 id)
        {
            Value = id;
        }

        public override Int32 GetHashCode() => Value.GetHashCode();
        public override Boolean Equals(Object other) => other is DbCachedDataId o && Value.Equals(o.Value);
        public override String ToString() => Value.ToString();
    }
}