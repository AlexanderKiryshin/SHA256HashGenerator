using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHA256HashGenerator.BlockReaders;
using SHA256HashGenerator.DataClass;
using System.IO;

namespace SHA256HashGeneratorTest
{
    [TestClass]
    public class PartBlockReaderTest
    {
        [TestMethod]
        public void GetPartBlockTest()
        {
            byte[] bytes = new byte[]
            {
                1, 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
            };

            PartBlockReader blockReader = new PartBlockReader(new MemoryStream(bytes), 8, 3);

            PartBlock p1 = (PartBlock)blockReader.GetPartBlock(0, 0);
            PartBlock p2 = (PartBlock)blockReader.GetPartBlock(1, 0);
            PartBlock p3 = (PartBlock)blockReader.GetPartBlock(2, 0);
            PartBlock p4 = (PartBlock)blockReader.GetPartBlock(0, 1);
            PartBlock p5 = (PartBlock)blockReader.GetPartBlock(1, 1);
            PartBlock p6 = (PartBlock)blockReader.GetPartBlock(2, 1);
            PartBlock p7 = (PartBlock)blockReader.GetPartBlock(0, 2);
            PartBlock p8 = (PartBlock)blockReader.GetPartBlock(1, 2);

            Assert.AreEqual(p1.Data.Length, 3);
            Assert.AreEqual(p2.Data.Length, 3);
            Assert.AreEqual(p3.Data.Length, 3);

            Assert.AreEqual(p4.Data.Length, 3);
            Assert.AreEqual(p5.Data.Length, 3);
            Assert.AreEqual(p6.Data.Length, 1);

            Assert.AreEqual(p7.Data.Length, 2);
            Assert.AreEqual(p8.Data.Length, 2);

            Assert.AreEqual(p1.IdFullBlock, 0);
            Assert.AreEqual(p4.IdFullBlock, 0);
            Assert.AreEqual(p7.IdFullBlock, 0);

            Assert.AreEqual(p2.IdFullBlock, 1);
            Assert.AreEqual(p5.IdFullBlock, 1);
            Assert.AreEqual(p8.IdFullBlock, 1);

            Assert.AreEqual(p3.IdFullBlock, 2);
            Assert.AreEqual(p6.IdFullBlock, 2);

            Assert.AreEqual(p6.SizeFullBlock, 4);
            Assert.AreEqual(p7.SizeFullBlock, 8);
            Assert.AreEqual(p8.SizeFullBlock, 8);

        }
    }
}
