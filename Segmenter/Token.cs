namespace Symanto.Jieba.Net.Core
{
    public class Token
    {
        public string Word { get; set; }
        public int StartIndex { get; set; }
        public int EndIndex { get; set; }

        public Token(string word, int startIndex, int endIndex)
        {
            Word = word;
            StartIndex = startIndex;
            EndIndex = endIndex;
        }

        public override string ToString()
        {
            return $"[{Word}, ({StartIndex}, {EndIndex})]";
        }
    }
}