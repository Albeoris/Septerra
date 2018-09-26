using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Septera
{
    public sealed class IdxContent : IEnumerable<IGrouping<DbPackage, IdxEntry>>
    {
        private readonly IReadOnlyList<DbPackage> _packages;
        private readonly Dictionary<DbPackage, IdxEntry[]> _dic;

        public IdxContent(IReadOnlyList<DbPackage> packages, Dictionary<DbPackage, IdxEntry[]> dic)
        {
            _packages = packages ?? throw new ArgumentNullException(nameof(packages));
            _dic = dic ?? throw new ArgumentNullException(nameof(dic));
        }

        public Int32 PackageCount => _packages.Count;

        public IEnumerator<IGrouping<DbPackage, IdxEntry>> GetEnumerator()
        {
            foreach (var pacakge in _packages)
            {
                Grouping grouping = new Grouping(pacakge, _dic[pacakge]);
                yield return grouping;
            }
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        private sealed class Grouping : IGrouping<DbPackage, IdxEntry>
        {
            public DbPackage Key { get; }

            private readonly IdxEntry[] _entries;

            public Grouping(DbPackage package, IdxEntry[] entries)
            {
                Key = package;
                _entries = entries;
            }

            public IEnumerator<IdxEntry> GetEnumerator()
            {
                foreach (IdxEntry item in _entries)
                    yield return item;
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return GetEnumerator();
            }
        }
    }
}