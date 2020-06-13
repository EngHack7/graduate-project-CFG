namespace CGrammar
{
    /* An if(-else) statement
     * BNF:
    selection_statement
	    : IF '(' expression ')' statement
	    | IF '(' expression ')' statement ELSE statement
	    ;
     
     */
    public sealed class CConditionalStatement : CStatement
    {
        public readonly CExpression condition;

        public readonly CStatement ifTrueStatement;

        public readonly CStatement elseStatement = null;

        public CConditionalStatement(CExpression condition, CStatement ifTrueStatement, CStatement elseStatement,
            CStatement nextStatement)
            : base(condition != null ? "if: " + condition.codeString : "", nextStatement)
        {
            this.condition = condition;

            this.ifTrueStatement = ifTrueStatement;
            this.elseStatement = elseStatement;

            this.ifTrueStatement.setNextStatement(null);
            this.elseStatement.setNextStatement(null);
        }

        public CConditionalStatement(CExpression condition, CStatement trueStatement, CStatement nextStatement)
            : base(condition != null ? "if: " + condition.codeString : "", nextStatement)
        {
            this.condition = condition;

            this.ifTrueStatement = trueStatement;

            this.ifTrueStatement.setNextStatement(null);
        }


        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            if (this.ifTrueStatement != null && this.ifTrueStatement.contains(statement))
                return true;
            if (this.elseStatement != null && this.elseStatement.contains(statement))
                return true;

            return false;
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            double width = this.ifTrueStatement.width(textMeasurer) + gap * 6, currWidth,
                conditionWidth = textMeasurer(this.codeString, fontHeight) + gap * 4;

            this.textMeasurer = textMeasurer;

            if (width < (currWidth = textMeasurer("true", fontHeight) + gap * 2))
                width = currWidth;

            if (this.elseStatement != null)
            {
                double elseWidth = this.elseStatement.width(textMeasurer) + gap * 6;

                if (elseWidth < (currWidth = textMeasurer("false", fontHeight) + gap * 4))
                    elseWidth = currWidth;

                width = width > elseWidth ? width * 2 : elseWidth * 2;
            }
            else
                width *= 2;

            return width > conditionWidth ? width : conditionWidth;
        }

        public override void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            lineDrawer(x, y + fontHeight + gap * 2, x, y + fontHeight + gap * 5);

            if (this.textMeasurer != null)
            {
                double conditionWidth = this.textMeasurer(this.codeString, fontHeight),
                    ifTrueWidth = this.ifTrueStatement.width(this.textMeasurer);

                lineDrawer(x - conditionWidth / 2 - gap, y, x + conditionWidth / 2 + gap, y);

                lineDrawer(x - conditionWidth / 2 - gap, y, x - conditionWidth / 2 - gap, y + fontHeight + gap * 2);
                lineDrawer(x + conditionWidth / 2 + gap, y, x + conditionWidth / 2 + gap, y + fontHeight + gap * 2);

                lineDrawer(x - conditionWidth / 2 - gap, y + fontHeight + gap * 2,
                    x + conditionWidth / 2 + gap, y + fontHeight + gap * 2);

                textDrawer(this.codeString, fontHeight, x - conditionWidth / 2, y);

                lineDrawer(x, y + fontHeight + gap * 5, x - gap * 4 - ifTrueWidth / 2, y + fontHeight + gap * 5);

                lineDrawer(x - gap * 4 - ifTrueWidth / 2, y + fontHeight + gap * 5,
                    x - gap * 4 - ifTrueWidth / 2, y + fontHeight + gap * 7);

                textDrawer("true", fontHeight,
                    x - gap * 4 - ifTrueWidth / 2 - this.textMeasurer("true", fontHeight) / 2, y + fontHeight + gap * 6);

                this.height = fontHeight * 2 + gap * 7;

                this.drawingComplete = true;

                /* ... */

                this.ifTrueStatement.draw(x - gap * 4 - ifTrueWidth / 2, y + this.height,
                    this.ifTrueStatement.width(this.textMeasurer), lineDrawer, textDrawer);

                if (!(this.ifTrueStatement is CJumpStatement))
                    lineDrawer(x - gap * 4 - ifTrueWidth / 2, y + this.height + ifTrueStatement.height,
                        x, y + this.height + ifTrueStatement.height);

                if (this.elseStatement != null)
                {
                    double elseWidth = this.elseStatement.width(this.textMeasurer);

                    lineDrawer(x, y + fontHeight + gap * 5, x + gap * 4 + elseWidth / 2, y + fontHeight + gap * 5);

                    lineDrawer(x + gap * 4 + elseWidth / 2, y + fontHeight + gap * 5,
                        x + gap * 4 + elseWidth / 2, y + fontHeight + gap * 7);

                    textDrawer("false", fontHeight,
                        x + gap * 4 + elseWidth / 2 - this.textMeasurer("false", fontHeight) / 2, y + fontHeight + gap * 6);

                    this.elseStatement.draw(x + gap * 4 + elseWidth / 2, y + this.height,
                        this.elseStatement.width(this.textMeasurer), lineDrawer, textDrawer);

                    if (!(this.elseStatement is CJumpStatement))
                        lineDrawer(x + gap * 4 + elseWidth / 2, y + this.height + elseStatement.height,
                            x, y + this.height + elseStatement.height);

                    if (ifTrueStatement.height > elseStatement.height)
                        this.height += ifTrueStatement.height + gap * 4;
                    else
                        this.height += elseStatement.height + gap * 4;
                }
                else
                    this.height += ifTrueStatement.height + gap * 4;

                lineDrawer(x, y + fontHeight + gap * 3, x, y + this.height);

                if (this.nextStatement != null)
                    this.nextStatement.draw(x, y + this.height, this.nextStatement.width(this.textMeasurer),
                        lineDrawer, textDrawer);
            }
        }
    }
}
