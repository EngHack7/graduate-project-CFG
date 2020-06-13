namespace CGrammar
{
    public sealed class CSwitchStatement : CStatement
    {
        public readonly CExpression condition;

        public readonly CStatement switchStatement;

        private CLabeledStatement[] jumpTargets;

        public CSwitchStatement(CExpression condition, CStatement switchStatement,
            CLabeledStatement[] jumpTargets, CStatement nextStatement)
            : base(condition != null ? "switch: " + condition.codeString : "",
                new CLabeledStatement("switch break target", null, nextStatement != null ? nextStatement :
                    new CExpressionStatement(new CExpression(";"), null)))
        {
            this.condition = condition;

            this.switchStatement = switchStatement;

            this.switchStatement.setNextStatement(null);

            this.jumpTargets = (CLabeledStatement[])jumpTargets.Clone();
        }


        public override bool contains(CStatement statement)
        {
            if (statement == this)
                return true;

            if (this.switchStatement != null && this.switchStatement.contains(statement))
                return true;

            return false;
        }

        public override double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            double conditionWidth = textMeasurer(this.codeString, fontHeight) + gap * 16,
                statementWidth = this.switchStatement.width(textMeasurer);

            this.textMeasurer = textMeasurer;

            return conditionWidth > statementWidth ? conditionWidth : statementWidth;
        }

        public override void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            lineDrawer(x, y + fontHeight + gap * 2, x, y + fontHeight + gap * 5);

            if (this.textMeasurer != null)
            {
                double conditionWidth = this.textMeasurer(this.codeString, fontHeight);

                lineDrawer(x - conditionWidth / 2 - gap, y, x + conditionWidth / 2 + gap, y);

                lineDrawer(x - conditionWidth / 2 - gap, y, x - conditionWidth / 2 - gap, y + fontHeight + gap * 2);
                lineDrawer(x + conditionWidth / 2 + gap, y, x + conditionWidth / 2 + gap, y + fontHeight + gap * 2);

                lineDrawer(x - conditionWidth / 2 - gap, y + fontHeight + gap * 2,
                    x + conditionWidth / 2 + gap, y + fontHeight + gap * 2);

                textDrawer(this.codeString, fontHeight, x - conditionWidth / 2, y);

                this.height = fontHeight + gap * 5;


                if (this.switchStatement != null)
                {
                    this.switchStatement.draw(x, y + this.height, this.switchStatement.width(this.textMeasurer),
                        lineDrawer, textDrawer);

                    this.height += this.switchStatement.height;
                }

                /* jumps to cases */

                lineDrawer(x - conditionWidth / 2 - gap, y + fontHeight / 2 + gap,
                    x - conditionWidth / 2 - gap * 8, y + fontHeight / 2 + gap);

                foreach (CLabeledStatement caseTarget in this.jumpTargets)
                    if (caseTarget != null)
                        lineDrawer(x - conditionWidth / 2 - gap * 8, y + fontHeight / 2 + gap,
                            caseTarget.jumpTargetX, caseTarget.jumpTargetY);
            }

            this.drawingComplete = true;

            if (this.nextStatement != null)
                this.nextStatement.draw(x, y + this.height, this.nextStatement.width(this.textMeasurer),
                    lineDrawer, textDrawer);
        }
    }
}
