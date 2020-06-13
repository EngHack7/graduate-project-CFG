namespace CParser
{
    public class CTokenAlternative : IMatchable
    {
        private IMatchable[] contents;

        public CTokenAlternative(params IMatchable[] containedTokens)
        {
            this.contents = new IMatchable[containedTokens.Length];

            System.Array.Copy(containedTokens, this.contents, containedTokens.Length);
        }

        private MatchResult matchNext(CToken[] tokens, int startIndex, IMatchable[] matchers, int matchersStartIndex)
        {
            MatchResult currResult = matchers[matchersStartIndex].match(tokens, startIndex);
            Matcher getNextMatch = null;
            bool matchComplete = false;

            matchers[matchersStartIndex] = currResult;

            if (currResult.matchComplete && matchersStartIndex < this.contents.Length - 1)
                getNextMatch = (tokenCollection, tokenStartIndex) =>
                    matchNext(tokenCollection, tokenStartIndex, (IMatchable[])matchers.Clone(), matchersStartIndex + 1);
            else if (!currResult.matchComplete)
                getNextMatch = (tokenCollection, tokenStartIndex) =>
                    matchNext(tokenCollection, tokenStartIndex, (IMatchable[])matchers.Clone(), matchersStartIndex);
            else if (currResult.matchComplete && matchersStartIndex == this.contents.Length - 1)
                matchComplete = true;

            return new MatchResult(currResult.numMatches, matchComplete, getNextMatch);
        }

        public MatchResult match(CToken[] tokens, int startIndex)
        {
            MatchResult currResult = matchNext(tokens, startIndex, (IMatchable[])this.contents.Clone(), 0);

            while (!currResult.isMatch && !currResult.matchComplete)
                currResult = currResult.match(tokens, startIndex);

            return currResult;
        }
    }
}
