namespace CGrammar
{
    /* base class for all statements
     * BNF:
     statement
	    : labeled_statement
	    | compound_statement
	    | expression_statement
	    | selection_statement
	    | iteration_statement
	    | jump_statement
	    ;
     
     */
    public abstract class CStatement : DrawCFGraph.IDrawable
    {
        public const double fontHeight = 40;

        public const double gap = 5;


        public CStatement nextStatement { get; protected set; }

        private bool nextStatementSet = false;


        public readonly string codeString;

        public CStatement(string codeString, CStatement nextStatement)
        {
            this.codeString = codeString;
            this.nextStatement = nextStatement;
        }

        public virtual void setNextStatement(CStatement nextStatement)
        {
            if (!this.nextStatementSet)
            {
                this.nextStatement = nextStatement;
                this.nextStatementSet = true;
            }
        }

        public override string ToString()
        {
            return "/* Statement: */ " + codeString;
        }

        public abstract double width(DrawCFGraph.TextMeasurer textMeasurer);

        public double height { get; protected set; }

        public DrawCFGraph.TextMeasurer textMeasurer { get; protected set; }

        public bool drawingComplete { get; protected set; }

        public abstract void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer);

        public abstract bool contains(CStatement statement);
    }
}
