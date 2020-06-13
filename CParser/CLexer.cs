namespace CParser
{
    public static class CLexer
    {
        private static System.Collections.Generic.Dictionary<string, CTokenType> tokensList = 
            new System.Collections.Generic.Dictionary<string, CTokenType>()
        {
            { "auto",	        CTokenType.AUTO },
            { "break",	        CTokenType.BREAK },
            { "case",	        CTokenType.CASE },
            { "char",	        CTokenType.CHAR },
            { "const",	        CTokenType.CONST },
            { "continue",		CTokenType.CONTINUE },
            { "default",		CTokenType.DEFAULT },
            { "do",	            CTokenType.DO },
            { "double",		    CTokenType.DOUBLE },
            { "else",	        CTokenType.ELSE },
            { "enum",	        CTokenType.ENUM },
            { "extern",		    CTokenType.EXTERN },
            { "float",	        CTokenType.FLOAT },
            { "for",	        CTokenType.FOR },
            { "goto",	        CTokenType.GOTO },
            { "if",	            CTokenType.IF },
            { "int",	        CTokenType.INT },
            { "long",	        CTokenType.LONG },
            { "register",		CTokenType.REGISTER },
            { "return",		    CTokenType.RETURN },
            { "short",	        CTokenType.SHORT },
            { "signed",		    CTokenType.SIGNED },
            { "sizeof",		    CTokenType.SIZEOF },
            { "static",		    CTokenType.STATIC },
            { "struct",		    CTokenType.STRUCT },
            { "switch",		    CTokenType.SWITCH },
            { "typedef",		CTokenType.TYPEDEF },
            { "union",	        CTokenType.UNION },
            { "unsigned",		CTokenType.UNSIGNED },
            { "void",	        CTokenType.VOID },
            { "volatile",		CTokenType.VOLATILE },
            { "while",	        CTokenType.WHILE },

            { "[a-zA-Z_]([a-zA-Z_]|\\d)*",	CTokenType.IDENTIFIER },

            { "0[xX][a-fA-F0-9]+(u|U|l|L)*?",	CTokenType.CONSTANT },
            { "0\\d+(u|U|l|L)*?",	CTokenType.CONSTANT },
            { "\\d+(u|U|l|L)*?",	CTokenType.CONSTANT },
            { "L?'(\\.|[^\\'])+'",	CTokenType.CONSTANT },

            { "\\d+[Ee][+-]?\\d+(f|F|l|L)?",	CTokenType.CONSTANT },
            { "\\d*\\.\\d+([Ee][+-]?\\d+)?(f|F|l|L)?",	CTokenType.CONSTANT },
            { "\\d+\\.\\d*([Ee][+-]?\\d+)?(f|F|l|L)?",	CTokenType.CONSTANT },

            { "L?\"(\\.|[^\\\"])*\"",	CTokenType.STRING_LITERAL },

            { "...",	        CTokenType.ELLIPSIS },
            { ">>=",	        CTokenType.RIGHT_ASSIGN },
            { "<<=",	        CTokenType.LEFT_ASSIGN },
            { "+=",	            CTokenType.ADD_ASSIGN },
            { "-=",	            CTokenType.SUB_ASSIGN },
            { "*=",	            CTokenType.MUL_ASSIGN },
            { "/=",	            CTokenType.DIV_ASSIGN },
            { "%=",	            CTokenType.MOD_ASSIGN },
            { "&=",	            CTokenType.AND_ASSIGN },
            { "^=",	            CTokenType.XOR_ASSIGN },
            { "|=",	            CTokenType.OR_ASSIGN },
            { ">>",	            CTokenType.RIGHT_OP },
            { "<<",	            CTokenType.LEFT_OP },
            { "++",	            CTokenType.INC_OP },
            { "--",	            CTokenType.DEC_OP },
            { "->",	            CTokenType.PTR_OP },
            { "&&",	            CTokenType.AND_OP },
            { "||",	            CTokenType.OR_OP },
            { "<=",	            CTokenType.LE_OP },
            { ">=",	            CTokenType.GE_OP },
            { "==",	            CTokenType.EQ_OP },
            { "!=",	            CTokenType.NE_OP },

            { ";",	            CTokenType.SEMICOLON },
            { "{",              CTokenType.LEFT_BRACE },
            { "<%",             CTokenType.LEFT_BRACE },
            { "}",              CTokenType.RIGHT_BRACE },
            { "%>",             CTokenType.RIGHT_BRACE },
            { ",",              CTokenType.COMMA },
            { ":",              CTokenType.COLON },
            { "=",              CTokenType.ASSIGN_OP },
            { "(",              CTokenType.LEFT_PAREN },
            { ")",              CTokenType.RIGHT_PAREN },
            { "[",              CTokenType.LEFT_BRACKET },
            { "<:",             CTokenType.LEFT_BRACKET },
            { "]",              CTokenType.RIGHT_BRACKET },
            { ":>",             CTokenType.RIGHT_BRACKET },
            { ".",              CTokenType.INST_OP },
            { "&",              CTokenType.BIT_AND_OP },
            { "!",              CTokenType.NOT_OP },
            { "~",              CTokenType.BIT_NOT_OP },
            { "-",              CTokenType.SUB_OP },
            { "+",              CTokenType.ADD_OP },
            { "*",              CTokenType.MUL_OP },
            { "/",              CTokenType.DIV_OP },
            { "%",              CTokenType.MOD_OP },
            { "<",              CTokenType.LT_OP },
            { ">",              CTokenType.GT_OP },
            { "^",              CTokenType.XOR_OP },
            { "|",              CTokenType.BIT_OR_OP },
            { "?",              CTokenType.QUERY },

            { "/*",             CTokenType.COMMENT},
            { "//",             CTokenType.COMMENT},
            { "#",              CTokenType.COMMENT} /* Preprocessor directives interpreted as comments! */
        };

        private static char[] whitespace = { ' ', '\t', '\v', '\r', '\n', '\f' };



        public static bool tokenExists(string tokenCode)
        {
            return tokensList.ContainsKey(tokenCode);
        }

        public static CTokenType getTokenType(string tokenCode)
        {
            return tokensList[tokenCode];
        }

        private static bool isWhitespace(char c)
        {
            foreach (char space in whitespace)
                if (c == space)
                    return true;
            return false;
        }

        private static bool isRegex(string key)
        {
            switch (tokensList[key])
            {
                case CTokenType.IDENTIFIER:
                case CTokenType.CONSTANT:
                case CTokenType.STRING_LITERAL:
                    return true;
            }

            return false;
        }

        private static bool getToken(string code, string token, int currCharIndex, 
            ref string resultString)
        {
            string substrdCode = code.Substring(currCharIndex);

            if (isRegex(token) &&
                System.Text.RegularExpressions.Regex.IsMatch(substrdCode, token) &&
                System.Text.RegularExpressions.Regex.Match(substrdCode, token).Index == 0)
            {
                resultString = System.Text.RegularExpressions.Regex.Match(substrdCode, token).Value;
                return true;
            }
            else if (substrdCode.Length >= token.Length && 
                token == code.Substring(currCharIndex, token.Length))
            {
                resultString = token;
                return true;
            }

            return false;
        }

        private static CToken matchToken(string code, ref int currLineIndex, ref int currCharIndex)
        {
            int maxLength = 0;
            CToken currToken = null;
            string currTokenCode = "";

            /* Skip all whitespace */

            for (;
                    currCharIndex < code.Length && isWhitespace(code[currCharIndex]);
                    currCharIndex++)
                if (code[currCharIndex] == '\n')
                    currLineIndex++;

            if (currCharIndex == code.Length)
                return new CToken("", CTokenType.EOF, currLineIndex, currCharIndex);


            /* Check if we landed on a token */

            foreach (string token in tokensList.Keys)
                if (getToken(code, token, currCharIndex, ref currTokenCode))
                    if (currTokenCode.Length > maxLength)
                    {
                        maxLength = currTokenCode.Length;
                        currToken = new CToken(currTokenCode, tokensList[token], currLineIndex, currCharIndex);
                    }

            if (maxLength == 0)
                throw new System.InvalidOperationException("Invalid identifier!");

            currCharIndex += maxLength;

            /* Process comments */

            if (currToken.tokenCode == "/*")
            {
                for (; currCharIndex < code.Length - 1 && code[currCharIndex] != '*' || code[currCharIndex + 1] != '/'; 
                    currCharIndex++)
                    if (code[currCharIndex] == '\n')
                        currLineIndex++;

                if (currCharIndex == code.Length - 1)
                    throw new System.InvalidOperationException("No terminating */ comment!");

                currCharIndex += 2;

                return matchToken(code, ref currLineIndex, ref currCharIndex);
            }
            else if (currToken.tokenCode == "//" || currToken.tokenCode == "#")
            {
                for (; currCharIndex < code.Length && code[currCharIndex] != '\n'; currCharIndex++) ;
                currLineIndex++;

                return matchToken(code, ref currLineIndex, ref currCharIndex);
            }

            return currToken;
        }

        private static string joinLines(string originalCode)
        {
            return originalCode.Replace("\\\r\n", "").Replace("\\\n", "");
        }

        public static CToken[] parseTokens(string code)
        {
            int currLineIndex = 0, currCharIndex = 0;
            System.Collections.Generic.List<CToken> tokenCollection = new System.Collections.Generic.List<CToken>();

            code = joinLines(code);

            while (currCharIndex < code.Length)
                tokenCollection.Add(matchToken(code, ref currLineIndex, ref currCharIndex));

            return tokenCollection.ToArray();
        }
       
    }
}
