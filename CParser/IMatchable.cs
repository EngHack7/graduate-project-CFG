namespace CParser
{
    public interface IMatchable
    {
        MatchResult match(CToken[] tokens, int startIndex);
    }
}
