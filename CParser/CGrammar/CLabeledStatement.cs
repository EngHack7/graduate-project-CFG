namespace CGrammar
{
    /* A statement transferring execution to another point in the program. 
     * Used for jump statements like goto, break, continue or return
     * BNF:
    labeled_statement
	    : IDENTIFIER ':' statement
	    | CASE constant_expression ':' statement
	    | DEFAULT ':' statement
	    ;
     
     */
    public sealed class CLabeledStatement : CStatement
    {
        public readonly CExpression caseExpression;
        public readonly CExpression returnedExpression; /* for return */

        public CStatement labeledStatement { get; private set; }
        private bool labeledStatementSet = false;

        private System.Collections.Generic.List<CJumpStatement> jumpOrigins = 
            new System.Collections.Generic.List<CJumpStatement>();
        private bool jumpOrigintSet = false;

        public double jumpTargetX { get; private set; }

        public double jumpTargetY { get; private set; }

        public CLabeledStatement(string codeString, CExpression caseExpression, CStatement labeledStatement)
            : base(caseExpression != null ? "case " + caseExpression.codeString : codeString,
                labeledStatement != null ? labeledStatement.nextStatement : null)
        {
            this.labeledStatement = labeledStatement;
            this.caseExpression = caseExpression;
        }


        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            if (this.labeledStatement != null && this.labeledStatement.contains(statement))
                return true;

            return false;
        }

        public void addJumpOrigin(CJumpStatement targetStatement)
        {
            this.jumpOrigins.Add(targetStatement);
            this.jumpOrigintSet = true;
        }

        public override void setNextStatement(CStatement nextStatement)
        {
            this.labeledStatement.setNextStatement(nextStatement);
        }

        public void setLabeledStatement(CStatement labeledStatement)
        {
            if (!this.labeledStatementSet)
            {
                this.labeledStatement = labeledStatement;
                this.labeledStatementSet = true;
            }
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            double labelWidth = textMeasurer(this.codeString, fontHeight) + gap * 6,
                labeledWidth = this.labeledStatement.width(textMeasurer);

            this.textMeasurer = textMeasurer;

            return labelWidth > labeledWidth ? labelWidth : labeledWidth;
        }

        public override void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            if (this.textMeasurer != null)
            {
                double labelWidth = textMeasurer(this.codeString, fontHeight);

                lineDrawer(x - labelWidth / 2 - gap, y, x + labelWidth / 2 + 3 * gap, y);

                textDrawer(this.codeString, fontHeight, x - labelWidth / 2, y);

                lineDrawer(x - labelWidth / 2 - gap, y + fontHeight + gap * 2,
                    x + labelWidth / 2 + 3 * gap, y + fontHeight + gap * 2);


                lineDrawer(x + labelWidth / 2 + 3 * gap, y, x + labelWidth / 2 + 3 * gap, y + fontHeight + gap * 2);

                this.jumpTargetX = x - labelWidth / 2 - 3 * gap;
                this.jumpTargetY = y + fontHeight / 2 + gap;
                lineDrawer(x - labelWidth / 2 - gap, y, this.jumpTargetX, this.jumpTargetY);

                lineDrawer(this.jumpTargetX, this.jumpTargetY, x - labelWidth / 2 - gap, y + fontHeight + gap * 2);

                this.height = fontHeight + gap * 2;

                lineDrawer(x, y + this.height, x, y + this.height + gap * 4);

                this.height += gap * 4;

                if (this.jumpOrigintSet) /* some jumps expect this label to be drawn! */
                    foreach (CJumpStatement jumpOrigin in this.jumpOrigins)
                    {
                        double delta1 = System.Math.Abs(this.jumpTargetX - jumpOrigin.jumpFromX1),
                                delta2 = System.Math.Abs(this.jumpTargetX - jumpOrigin.jumpFromX2);

                        if (delta1 < delta2)
                            lineDrawer(jumpOrigin.jumpFromX1, jumpOrigin.jumpFromY, this.jumpTargetX, this.jumpTargetY);
                        else
                            lineDrawer(jumpOrigin.jumpFromX2, jumpOrigin.jumpFromY, this.jumpTargetX, this.jumpTargetY);
                    }

                this.drawingComplete = true;

                if (this.labeledStatement != null)
                {
                    this.labeledStatement.draw(x, y + this.height, this.labeledStatement.width(this.textMeasurer),
                        lineDrawer, textDrawer);

                    this.height += this.labeledStatement.height;
                }
            }
        }
    }
}
