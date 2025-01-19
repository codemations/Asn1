#if !NET6_0_OR_GREATER
namespace Codemations.Asn1
{
    internal struct HashCode
    {
        private int _hash = 17;

        public HashCode()
        {
        }

        public void Add<T>(T value)
        {
            unchecked
            {
                _hash = _hash * 31 + value?.GetHashCode() ?? 0;
            }
        }

        public readonly int ToHashCode()
        {
            return _hash;
        }
    }
}
#endif
