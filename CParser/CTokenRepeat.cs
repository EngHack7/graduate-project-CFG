namespace CParser
{
    class CTokenRepeat : IMatchable
    {
        public delegate bool MatchCompleteChecker(CToken[] tokens, int startIndex, int numMatched);


        private IMatchable content;

        public readonly MatchCompleteChecker matchCompleteChecker;

        public readonly int repeatLowerBound;

        public int repeatUpperBound { get; private set; }

        public readonly bool infiniteRepeat;

        public readonly bool greedyMatch;

        public CTokenRepeat(IMatchable content, bool greedyMatch, int repeatLowerBound, int repeatUpperBound,
            MatchCompleteChecker matchCompleteChecker = null)
        {
            this.content = content;
            this.repeatLowerBound = repeatLowerBound;
            this.repeatUpperBound = repeatUpperBound;

            this.greedyMatch = greedyMatch;
            this.matchCompleteChecker = matchCompleteChecker;
        }

        public CTokenRepeat(IMatchable content, bool greedyMatch,
            int repeatLowerBound = 0, MatchCompleteChecker matchCompleteChecker = null)
            : this(content, greedyMatch, repeatLowerBound, 0, matchCompleteChecker)
        {
            this.infiniteRepeat = true;
        }


        private CTokenSequence getRepeatSequence(int numRepeat)
        {
            if (numRepeat == 0)
                return new CTokenSequence(CToken.emptyToken);

            else if (numRepeat > 0)
                return new CTokenSequence(
                    System.Linq.Enumerable.ToArray(System.Linq.Enumerable.Repeat(this.content, numRepeat))
                );

            throw new System.ArgumentOutOfRangeException("numRepeat", "numRepeat should be non-negative!");
        }

        private MatchResult matchNext(CToken[] tokens, int startIndex, IMatchable currSequence, int numRepeat)
        {
            MatchResult currResult = currSequence.match(tokens, startIndex);
            int nextNumRepeat = this.greedyMatch ? numRepeat - 1 : numRepeat + 1;

            if (!currResult.matchComplete)
                return new MatchResult(currResult.numMatches, false,
                        (tokensArr, tokensStartIndex) => matchNext(tokensArr, tokensStartIndex, currResult, numRepeat));

            if (nextNumRepeat < this.repeatLowerBound || nextNumRepeat > this.repeatUpperBound)
                return currResult;

            return new MatchResult(currResult.numMatches, currResult.isMatch, false,
                    (tokensArr, tokensStartIndex) => matchNext(tokensArr, tokensStartIndex,
                        getRepeatSequence(nextNumRepeat), nextNumRepeat));
        }

        public MatchResult match(CToken[] tokens, int startIndex)
        {
            int numRepeat = this.greedyMatch ?
                (this.infiniteRepeat ? tokens.Length - startIndex : this.repeatUpperBound)
                :
                this.repeatLowerBound;

            if (this.infiniteRepeat)
                this.repeatUpperBound = tokens.Length - startIndex;

            MatchResult currResult = matchNext(tokens, startIndex, getRepeatSequence(numRepeat), numRepeat);

            while (((!currResult.isMatch && this.matchCompleteChecker == null) || 
                (this.matchCompleteChecker != null && !this.matchCompleteChecker(tokens, startIndex, currResult.numMatches))) 
                && !currResult.matchComplete)
                currResult = currResult.match(tokens, startIndex);

            return currResult;
        }
    }
}
