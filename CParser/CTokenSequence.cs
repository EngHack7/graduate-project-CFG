namespace CParser
{
    public class CTokenSequence : IMatchable
    {
        private IMatchable[] contents;

        public CTokenSequence(params IMatchable[] containedTokens)
        {
            this.contents = (IMatchable[])containedTokens.Clone();
        }


        private MatchResult matchNext(CToken[] tokens, int startIndex,
            IMatchable[] matchersSequence, int matchersSequenceStartIndex)
        {
            MatchResult currResult, nextResult;

            do
            {
                matchersSequence[matchersSequenceStartIndex] = currResult = 
                    matchersSequence[matchersSequenceStartIndex].match(tokens, startIndex);

                nextResult = null;

                if (currResult.isMatch && matchersSequenceStartIndex < matchersSequence.Length - 1)
                    do
                    {
                        nextResult = nextResult == null ? matchNext(tokens, startIndex + currResult.numMatches,
                            (IMatchable[])matchersSequence.Clone(), matchersSequenceStartIndex + 1)
                            :
                            nextResult.match(tokens, startIndex + currResult.numMatches);

                        if (nextResult.isMatch)
                        {
                            if (!nextResult.matchComplete)
                                return new MatchResult(currResult.numMatches + nextResult.numMatches, false,
                                    (tokensArr, tokensStartIndex) =>
                                        matchNext(tokensArr, tokensStartIndex,
                                            (IMatchable[])matchersSequence.Clone(), matchersSequenceStartIndex + 1));
                            else if (!currResult.matchComplete)
                                return new MatchResult(currResult.numMatches + nextResult.numMatches, false,
                                    (tokensArr, tokensStartIndex) =>
                                        matchNext(tokensArr, tokensStartIndex,
                                            (IMatchable[])matchersSequence.Clone(), matchersSequenceStartIndex));
                            else if (nextResult.matchComplete)
                                return new MatchResult(currResult.numMatches + nextResult.numMatches);
                        }

                    } while (!nextResult.matchComplete);
                else if (matchersSequenceStartIndex == matchersSequence.Length - 1)
                    return currResult;

            } while (!currResult.matchComplete);

            return new MatchResult(0); /* failed to match anything! */
        }

        public MatchResult match(CToken[] tokens, int startIndex)
        {
            return matchNext(tokens, startIndex, (IMatchable[])this.contents.Clone(), 0);
        }
    }
}
