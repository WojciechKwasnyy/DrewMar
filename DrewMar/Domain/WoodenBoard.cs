namespace DrewMar.Domain
{
    internal class WoodenBoard
    {
        public double Length { get; set; }
        public double Width { get; set; }
        public double Thick { get; set; }
        public string Worker { get; set; }
        public double Weight => Length * Width * Thick;
    }
}
