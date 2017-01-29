using System.Security.Cryptography;

namespace SHA256HashGenerator.DataClass
{
    public class SHA256Data
    {
        public long BlockSize { get; set; }
        public int CurrentCalculated { get; set; }
        public SHA256Managed Sha256 { get; set; }
    }
}
