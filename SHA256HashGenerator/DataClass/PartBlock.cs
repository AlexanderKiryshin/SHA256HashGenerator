
namespace SHA256HashGenerator.DataClass
{
    public class PartBlock : Block
    {
        public int Size { get; set; }
        public byte[] Data { get; set; }
        //номер целого блока которому принадлежит
        public int IdFullBlock { get; set; }
        public int SizeFullBlock { get; set; }
    }
}