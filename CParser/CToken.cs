namespace CParser
{
    public class CToken : IMatchable
    {
        public static readonly CTokenAlternative anyToken;

        public static readonly CToken emptyToken = new CEmptyToken();

        /* The token's text */
        public readonly string tokenCode;

        public readonly CTokenType tokenType;

        /* 1-based offsets for error messages */
        public readonly int lineNumber;

        public readonly int startSym;

        static CToken()
        {
            System.Collections.Generic.List<CToken> allTokens = new System.Collections.Generic.List<CToken>();

            foreach (CTokenType tokenType in System.Enum.GetValues(typeof(CTokenType)))
                allTokens.Add(new CToken(tokenType));

            anyToken = new CTokenAlternative(allTokens.ToArray());
        }

        public static CTokenAlternative anyTokenBesides(params CTokenType[] exceptions)
        {
            System.Collections.Generic.List<CToken> allTokens = new System.Collections.Generic.List<CToken>();

            foreach (CTokenType tokenType in System.Enum.GetValues(typeof(CTokenType)))
                if (!System.Linq.Enumerable.Contains(exceptions, tokenType))
                    allTokens.Add(new CToken(tokenType));

            return new CTokenAlternative(allTokens.ToArray());
        }

        public CToken(string tokenCode, int lineIndex = -1, int startSymIndex = -1)
        {
            this.tokenCode = tokenCode;

            if (CLexer.tokenExists(tokenCode))
                this.tokenType = CLexer.getTokenType(tokenCode);

            this.lineNumber = lineIndex;
            this.startSym = startSymIndex;
        }

        public CToken(string tokenCode, CTokenType tokenType, int lineIndex = -1, int startSymIndex = -1)
        {
            this.tokenCode = tokenCode;

            this.tokenType = tokenType;

            this.lineNumber = lineIndex + 1;    //output should be 1-based
            this.startSym = startSymIndex + 1;  //output should be 1-based
        }

        public CToken(CTokenType tokenType, int lineIndex = -1, int startSymIndex = -1)
        {
            this.tokenType = tokenType;

            this.lineNumber = lineIndex + 1;    //output should be 1-based
            this.startSym = startSymIndex + 1;  //output should be 1-based
        }


        public MatchResult match(CToken[] tokens, int startIndex)
        {
            return new MatchResult(startIndex < tokens.Length && this.tokenType == tokens[startIndex].tokenType ? 1 : 0);
        }

        public override string ToString()
        {
            return tokenCode;
        }
    }

    public sealed class CEmptyToken : CToken, IMatchable
    {
        new public MatchResult match(CToken[] tokens, int startIndex)
        {
            return new MatchResult(0, true, true, null);
        }

        public CEmptyToken() : base("") { }
    }
}
