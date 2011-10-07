using System.Linq;
using Bio;

namespace BiodexExcel
{
    public static class SequenceExtensions
    {
        public static string ConvertToString(this ISequence sequence)
        {
            return new string(sequence.ToArray().Select(a => (char)a).ToArray());
        }
    }
}
