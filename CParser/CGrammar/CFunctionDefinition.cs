namespace CGrammar
{
    public sealed class CFunctionDefinition : DrawCFGraph.IDrawable
    {
        public readonly string functionName;

        public readonly CCompoundStatement functionBody;

        public CFunctionDefinition(string functionName, CCompoundStatement functionBody)
        {
            this.functionName = functionName;
            this.functionBody = functionBody;
        }

        public double width(DrawCFGraph.TextMeasurer textMeasurer)
        {
            double functionBodyWidth = this.functionBody.width(textMeasurer),
                textWidth = textMeasurer("function: " + this.functionName, CStatement.fontHeight);

            this.textMeasurer = textMeasurer;

            return functionBodyWidth > textWidth ? functionBodyWidth : textWidth;
        }

        public double height { get; private set; }

        public bool drawingComplete { get; private set; }

        public DrawCFGraph.TextMeasurer textMeasurer { get; private set; }

        public void draw(double x, double y, double contentsWidth,
            DrawCFGraph.LineDrawer lineDrawer, DrawCFGraph.TextDrawer textDrawer)
        {
            if (this.textMeasurer != null)
            {
                textDrawer("function: " + this.functionName, CStatement.fontHeight,
                    x - textMeasurer("function: " + this.functionName, CStatement.fontHeight) / 2, y);

                this.drawingComplete = true;

                this.functionBody.draw(x, y + CStatement.fontHeight + CStatement.gap * 2,
                    this.functionBody.width(this.textMeasurer), lineDrawer, textDrawer);

                this.height = CStatement.fontHeight + CStatement.gap * 2 + this.functionBody.height;
            }
        }
    }
}
