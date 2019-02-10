using System;

namespace Septerra
{
    public sealed class ProgramArguments
    {
        private readonly String[] _args;
        private Int32 _currentIndex;

        public ProgramArguments(String[] args)
        {
            _args = args ?? new String[0];
        }

        public Boolean TryGetNext(out String value)
        {
            if (_currentIndex >= _args.Length)
            {
                value = null;
                return false;
            }

            value = _args[_currentIndex++];
            return true;
        }
    }
}