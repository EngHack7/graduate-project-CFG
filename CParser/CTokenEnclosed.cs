namespace CParser
{
    class CTokenEnclosed : IMatchable
    {
        public readonly CToken tokenStart;

        public readonly CToken tokenEnd;

        public CTokenEnclosed(CToken tokenStart, CToken tokenEnd)
        {
            this.tokenStart = tokenStart;
            this.tokenEnd = tokenEnd;
        }


        public MatchResult match(CToken[] tokens, int startIndex)
        {
            int i, countStartTokens = 0, countEndTokens = 0;

            if (tokens[startIndex].tokenType != this.tokenStart.tokenType)
                return new MatchResult(0);

            for (i = startIndex; i < tokens.Length; i++)
            {
                if (tokens[i].tokenType == this.tokenStart.tokenType)
                    countStartTokens++;
                else if (tokens[i].tokenType == this.tokenEnd.tokenType)
                    countEndTokens++;

                if (countStartTokens == countEndTokens)
                    return new MatchResult(i - startIndex + 1);
            }

            return new MatchResult(0);
        }
    }
}
