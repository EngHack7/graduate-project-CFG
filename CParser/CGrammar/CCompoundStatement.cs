namespace CGrammar
{
    /* A compound statement
     * 
     * BNF:
     compound_statement
	    : '{' '}'
	    | '{' statement_list '}'
	    | '{' declaration_list '}'
	    | '{' declaration_list statement_list '}'
	    ;
     
     */
    public sealed class CCompoundStatement : CStatement, System.Collections.Generic.IEnumerable<CStatement>
    {
        /* declarations treated as initialization statements and included in the statement list */
        private CStatement[] statementList;

        public CCompoundStatement(CStatement[] statementList, CStatement nextStatement)
            : base("{ /* ... */ }", nextStatement)
        {
            this.statementList = (CStatement[])statementList.Clone();
        }


        public CStatement this[int i]
        {
            get { return this.statementList[i]; }
        }

        public System.Collections.Generic.IEnumerator<CStatement> GetEnumerator()
        {
            foreach (CStatement token in this.statementList)
                yield return token;
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.statementList.GetEnumerator();
        }

        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            foreach (CStatement currStatement in this.statementList)
                if (currStatement != null && currStatement.contains(statement))
                    return true;

            return false;
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            double maxWidth = 0;

            this.textMeasurer = textMeasurer;

            foreach (CStatement statement in this.statementList)
            {
                if (statement != null)
                {
                    double currWidth = statement.width(textMeasurer);

                    if (currWidth > maxWidth)
                        maxWidth = currWidth;
                }
            }

            return maxWidth + gap * 6;
        }

        public override void draw(double x, double y, double contentsWidth, 
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            lineDrawer(x - contentsWidth / 2, y, x + contentsWidth / 2, y);

            lineDrawer(x, y, x, y + fontHeight + gap * 2);

            lineDrawer(x - contentsWidth / 2, y, x - contentsWidth / 2, y + fontHeight + gap * 2);
            lineDrawer(x + contentsWidth / 2, y, x + contentsWidth / 2, y + fontHeight + gap * 2);

            this.height = fontHeight + gap * 2;

            if (this.textMeasurer != null && this.statementList.Length > 0 && this.statementList[0] != null)
                this.statementList[0].draw(x, y + this.height,
                    this.statementList[0].width(this.textMeasurer), lineDrawer, textDrawer);

            foreach (CStatement currStatement in this.statementList)
                if (currStatement != null && currStatement.drawingComplete)
                    this.height += currStatement.height;

            lineDrawer(x - contentsWidth / 2, y + this.height,
                x - contentsWidth / 2, y + this.height - fontHeight - gap * 2);
            lineDrawer(x + contentsWidth / 2, y + this.height,
                x + contentsWidth / 2, y + this.height - fontHeight - gap * 2);

            lineDrawer(x - contentsWidth / 2, y + this.height,
                x + contentsWidth / 2, y + this.height);

            lineDrawer(x, y + this.height, x, y + this.height + gap * 4);

            this.height += gap * 4;

            this.drawingComplete = true;

            if (this.nextStatement != null && this.textMeasurer != null)
                this.nextStatement.draw(x, y + this.height, this.nextStatement.width(this.textMeasurer),
                    lineDrawer, textDrawer);
        }
    }
}
