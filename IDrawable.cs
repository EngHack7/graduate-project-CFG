namespace DrawCFGraph
{
    public interface IDrawable
    {
        double width(TextMeasurer textMeasurer);

        double height { get; }

        TextMeasurer textMeasurer { get; }

        bool drawingComplete { get; }

        void draw(double x, double y, double contentsWidth, LineDrawer lineDrawer, TextDrawer textDrawer);
    }
}
