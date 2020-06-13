namespace CGrammar
{
    /* A statement transferring execution to another point in the program. 
     * Used for jump statements like goto, break, continue or return
     * BNF:
    jump_statement
	    : GOTO IDENTIFIER ';'
	    | CONTINUE ';'
	    | BREAK ';'
	    | RETURN ';'
	    | RETURN expression ';'
	    ;
     
     */
    public sealed class CJumpStatement : CStatement
    {
        public readonly string targetIdentifier; /* for goto */

        public readonly CExpression returnedExpression; /* for return */

        public CLabeledStatement targetStatement { get; private set; }
        private bool targetStatementSet = false;

        public double jumpFromX1 { get; private set; }
        public double jumpFromX2 { get; private set; }
        public double jumpFromY { get; private set; }

        public CJumpStatement(string codeString, CStatement nextStatement)
            : base(codeString, nextStatement)
        {
            string[] parts = codeString.Split(' ');

            if (parts[0] == "goto" && parts.Length > 1)
                this.targetIdentifier = parts[1];
            else
                this.targetIdentifier = null;

            this.returnedExpression = null;
        }

        public CJumpStatement(CExpression returnedExpression, CStatement nextStatement)
            : base("return " + returnedExpression.codeString, nextStatement)
        {
            this.targetIdentifier = null;
            this.returnedExpression = returnedExpression;
        }


        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            return false;
        }

        public void setTargetStatement(CLabeledStatement targetStatement)
        {
            if (!this.targetStatementSet)
            {
                this.targetStatement = targetStatement;
                this.targetStatementSet = true;
            }
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            this.textMeasurer = textMeasurer;

            return textMeasurer(this.codeString, fontHeight) + gap * 10;
        }

        public override void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            this.jumpFromY = y + fontHeight / 2 + gap;

            this.jumpFromX1 = x - contentsWidth / 2;

            this.jumpFromX2 = x + contentsWidth / 2;

            lineDrawer(this.jumpFromX1 + gap * 4, y, this.jumpFromX2 - gap * 4, y);

            lineDrawer(this.jumpFromX1 + gap * 4, y, this.jumpFromX1 + gap * 4, y + fontHeight + gap * 2);
            lineDrawer(this.jumpFromX2 - gap * 4, y, this.jumpFromX2 - gap * 4, y + fontHeight + gap * 2);


            lineDrawer(this.jumpFromX1, this.jumpFromY, this.jumpFromX1 + gap * 4, this.jumpFromY);

            lineDrawer(this.jumpFromX2, this.jumpFromY, this.jumpFromX2 - gap * 4, this.jumpFromY);

            textDrawer(this.codeString, fontHeight, this.jumpFromX1 + gap * 5, y + gap);

            this.height = fontHeight + gap * 7;

            this.drawingComplete = true;

            if (this.targetStatement != null)
            {
                if (this.targetStatement.drawingComplete)
                {
                    double delta1 = System.Math.Abs(this.targetStatement.jumpTargetX - this.jumpFromX1),
                                delta2 = System.Math.Abs(this.targetStatement.jumpTargetX - this.jumpFromX2);

                    if (delta1 < delta2)
                        lineDrawer(this.jumpFromX1, this.jumpFromY,
                            this.targetStatement.jumpTargetX, this.targetStatement.jumpTargetY);
                    else
                        lineDrawer(this.jumpFromX2, this.jumpFromY,
                            this.targetStatement.jumpTargetX, this.targetStatement.jumpTargetY);
                }
                else
                    this.targetStatement.addJumpOrigin(this);
            }

            if (this.nextStatement != null && this.textMeasurer != null)
                this.nextStatement.draw(x, y + this.height, this.nextStatement.width(this.textMeasurer),
                    lineDrawer, textDrawer);
        }
    }
}
