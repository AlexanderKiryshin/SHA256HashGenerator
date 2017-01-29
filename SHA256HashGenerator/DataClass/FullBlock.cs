namespace SHA256HashGenerator.DataClass
{
    public class FullBlock : Block
    {
        public int Size { get; set; }
        public byte[] Data { get; set; }
    }
}