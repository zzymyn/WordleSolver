using System.Diagnostics;

namespace WordleSolver
{
    [DebuggerDisplay("{DebuggerDisplay,nq}")]
    public partial struct Word : IEquatable<Word>
    {
        public char C0;
        public char C1;
        public char C2;
        public char C3;
        public char C4;

        private string DebuggerDisplay => $"\"{C0}{C1}{C2}{C3}{C4}\"";

        public Word(char c0, char c1, char c2, char c3, char c4)
        {
            C0 = c0;
            C1 = c1;
            C2 = c2;
            C3 = c3;
            C4 = c4;
        }

        public Word(string txt)
        {
            if (txt == null) throw new ArgumentNullException(nameof(txt));
            if (txt.Length != 5) throw new ArgumentException($"Word '{txt}' does not contain 5 letters.");
            C0 = char.ToUpperInvariant(txt[0]);
            C1 = char.ToUpperInvariant(txt[1]);
            C2 = char.ToUpperInvariant(txt[2]);
            C3 = char.ToUpperInvariant(txt[3]);
            C4 = char.ToUpperInvariant(txt[4]);
        }

        public static implicit operator Word(string txt)
        {
            return new Word(txt);
        }

        public static implicit operator string(Word w)
        {
            return w.ToString();
        }

        public static bool operator ==(Word left, Word right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Word left, Word right)
        {
            return !(left == right);
        }

        public override string ToString()
        {
            return $"{C0}{C1}{C2}{C3}{C4}";
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(C0, C1, C2, C3, C4);
        }

        public override bool Equals(object? obj)
        {
            return obj is Word word && Equals(word);
        }

        public bool Equals(Word other)
        {
            return C0 == other.C0 &&
                   C1 == other.C1 &&
                   C2 == other.C2 &&
                   C3 == other.C3 &&
                   C4 == other.C4;
        }
    }
}
