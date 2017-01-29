using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHA256HashGenerator.BlockReaders;
using System.IO;
using SHA256HashGenerator.DataClass;

namespace SHA256HashGeneratorTest
{
    [TestClass]
    public class FullBlockReaderTest
    {
        [TestMethod]
        public void GetNextBlockTest()
        {
            byte[] bytes = new byte[]
           {
                1, 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
           };

            FullBlockReader blockReader = new FullBlockReader(new MemoryStream(bytes), 8);

            FullBlock p1 = (FullBlock)blockReader.GetNextBlock();
            FullBlock p2 = (FullBlock)blockReader.GetNextBlock();
            FullBlock p3 = (FullBlock)blockReader.GetNextBlock();
            FullBlock p4 = (FullBlock)blockReader.GetNextBlock();
            Assert.AreEqual(p1.Data.Length, 8);
            Assert.AreEqual(p2.Data.Length, 8);
            Assert.AreEqual(p3.Data.Length, 4);

            Assert.AreEqual(p1.Size, 8);
            Assert.AreEqual(p2.Size, 8);
            Assert.AreEqual(p3.Size, 4);

            Assert.AreEqual(p4, null);
        }
    }
}
