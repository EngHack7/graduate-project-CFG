namespace CParser
{
    public sealed class CMatchedData : System.Collections.Generic.IEnumerable<CToken>
    {
        private CToken[] matched;

        public readonly int startIndex;

        public readonly int numMatches;

        public CMatchedData(CToken[] tokens, int startIndex, int numMatches)
        {
            this.startIndex = startIndex;

            this.matched = new CToken[numMatches];

            System.Array.Copy(tokens, startIndex, this.matched, 0, numMatches);

            this.numMatches = numMatches;
        }


        public CToken this[int i]
        {
            get { return this.matched[i]; }
        }

        public System.Collections.Generic.IEnumerator<CToken> GetEnumerator()
        {
            foreach (CToken token in this.matched)
                yield return token;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.matched.GetEnumerator();
        }

        public CToken[] getTokens()
        {
            return (CToken[])this.matched.Clone();
        }
    }
}
