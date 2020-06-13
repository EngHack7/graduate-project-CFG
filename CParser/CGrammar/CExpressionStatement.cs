namespace CGrammar
{
    /* An expression statement
     * 
     * BNF:
     expression_statement
	    : ';'
	    | expression ';'
	    ;
     
     */
    public sealed class CExpressionStatement : CStatement
    {
        public readonly CExpression expression;

        public CExpressionStatement(CExpression expression, CStatement nextStatement)
            : base(expression.codeString, nextStatement)
        {
            this.expression = expression;
        }


        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            return false;
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            this.textMeasurer = textMeasurer;

            return textMeasurer(this.codeString, fontHeight) + gap * 2;
        }

        public override void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            lineDrawer(x - contentsWidth / 2, y, x + contentsWidth / 2, y);

            lineDrawer(x - contentsWidth / 2, y, x - contentsWidth / 2, y + fontHeight + gap * 2);
            lineDrawer(x + contentsWidth / 2, y, x + contentsWidth / 2, y + fontHeight + gap * 2);

            lineDrawer(x - contentsWidth / 2, y + fontHeight + gap * 2,
                x + contentsWidth / 2, y + fontHeight + gap * 2);

            textDrawer(this.codeString, fontHeight, x - contentsWidth / 2 + gap, y);

            lineDrawer(x, y + fontHeight + gap * 2, x, y + fontHeight + gap * 6);

            this.drawingComplete = true;

            this.height = fontHeight + gap * 6;

            if (this.nextStatement != null && this.textMeasurer != null)
                this.nextStatement.draw(x, y + this.height, this.nextStatement.width(this.textMeasurer),
                    lineDrawer, textDrawer);
        }
    }
}
