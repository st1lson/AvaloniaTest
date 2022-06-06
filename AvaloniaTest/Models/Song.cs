namespace AvaloniaTest.Models
{
    internal class Song
    {
        public string Name { get; }

        public string Artist { get; }

        public Song(string name, string artist)
        {
            Name = name;
            Artist = artist;
        }
    }
}
