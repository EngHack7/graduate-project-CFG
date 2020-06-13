namespace CGrammar 
{
    /* An expression
     * 
     * BNF:
     expression
	    : assignment_expression
	    | expression ',' assignment_expression
	    ;
     
     */
    public sealed class CExpression
    {
        public readonly string codeString;

        public CExpression(string codeString)
        {
            this.codeString = codeString;
        }

        public string getDisplayValue()
        {
            return this.codeString.Trim();
        }

        public override string ToString()
        {
            return "/* Expression: */ " + this.codeString;
        }
    }
}
