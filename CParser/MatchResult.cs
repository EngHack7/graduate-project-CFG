namespace CParser
{
    public sealed class MatchResult : IMatchable
    {
        public readonly int numMatches;

        public readonly bool isMatch;

        public readonly bool matchComplete;

        public readonly Matcher getNextMatch;

        public MatchResult(int numMatches, bool matchComplete, Matcher getNextMatch)
        {
            this.numMatches = numMatches;
            this.isMatch = numMatches > 0;
            this.matchComplete = matchComplete;

            this.getNextMatch = getNextMatch;
        }

        public MatchResult(int numMatches, bool isMatch, bool matchComplete, Matcher getNextMatch)
        {
            this.numMatches = numMatches;
            this.isMatch = isMatch;
            this.matchComplete = matchComplete;

            this.getNextMatch = getNextMatch;
        }

        /* Assumes match has been complete */
        public MatchResult(int numMatches) : this(numMatches, true, null) { }


        public MatchResult match(CToken[] tokens, int startIndex)
        {
            if (!this.matchComplete)
                return this.getNextMatch(tokens, startIndex);

            return this;
        }
    }
}
