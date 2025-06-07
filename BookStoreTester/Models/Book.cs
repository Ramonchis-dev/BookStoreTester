namespace BookStoreTester.Models
{
    public class Book
    {
        public int Index { get; set; }
        public string ISBN { get; set; } = string.Empty;
        public string Title { get; set; } = string.Empty;
        public List<string> Authors { get; set; } = new();
        public string Publisher { get; set; } = string.Empty;
        public int Likes { get; set; }
        public List<Review> Reviews { get; set; } = new();
        public string CoverImageUrl { get; set; } = string.Empty;
    }

    public class Review
    {
        public string Text { get; set; } = string.Empty;
        public string Author { get; set; } = string.Empty;
        public double Rating { get; set; }
    }

    public enum SupportedLocale
    {
        EnglishUS,
        German,
        Japanese,
        French,
        Spanish
    }
}