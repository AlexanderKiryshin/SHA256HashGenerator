using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SHA256HashGenerator.BlockReaders;
using System.IO;
using SHA256HashGenerator.DataClass;
using System.Security.Cryptography;
using SHA256 = SHA256HashGenerator.SHA256;

namespace SHA256HashGeneratorTest
{
    [TestClass]
    public class SHA256Test
    {
        [TestMethod]
        public void CalculateSha256FullBlockTest()
        {
            byte[] bytes = new byte[]
          {
                1, 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
          };

            FullBlockReader blockReader = new FullBlockReader(new MemoryStream(bytes), 8);
            FullBlock[] fb = new FullBlock[3];
            for (int i = 0; i < 3; i++)
            {
                fb[i] = (FullBlock)blockReader.GetNextBlock();
            }
            SHA256Managed sha256Verified = new SHA256Managed();
            SHA256 sha256NotVerified = new SHA256();
            byte[][] verifiedHash = new byte[3][];
            byte[][] notVerifiedHash = new byte[3][];

            for (int i = 0; i < 3; i++)
            {
                verifiedHash[i] = sha256Verified.ComputeHash(fb[i].Data);
                notVerifiedHash[i] = sha256NotVerified.CalculateSha256(fb[i]);
            }
            bool bEqual = true;
            for (int i = 0; i < 3; i++)
            {
                if (verifiedHash[i].Length != notVerifiedHash[i].Length)
                {
                    bEqual = false;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int length = verifiedHash[i].Length;
                for (int j = 0; j < length; j++)
                {
                    if (verifiedHash[i][j] != notVerifiedHash[i][j])
                        bEqual = false;
                }
            }
            Assert.AreEqual(true, bEqual);
        }
        [TestMethod]
        public void CalculateSha256PartBlockTest()
        {
            byte[] bytes = new byte[]
           {
                1, 2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
           };

            PartBlockReader blockReader = new PartBlockReader(new MemoryStream(bytes), 8, 3);
            PartBlock[] pb = new PartBlock[8];
            pb[0] = (PartBlock)blockReader.GetPartBlock(0, 0);
            pb[1] = (PartBlock)blockReader.GetPartBlock(0, 1);
            pb[2] = (PartBlock)blockReader.GetPartBlock(0, 2);
            pb[3] = (PartBlock)blockReader.GetPartBlock(1, 0);
            pb[4] = (PartBlock)blockReader.GetPartBlock(1, 1);
            pb[5] = (PartBlock)blockReader.GetPartBlock(1, 2);
            pb[6] = (PartBlock)blockReader.GetPartBlock(2, 0);
            pb[7] = (PartBlock)blockReader.GetPartBlock(2, 1);
            SHA256Managed sha256Verified = new SHA256Managed();
            SHA256 sha256NotVerified = new SHA256();
            byte[][] verifiedHash = new byte[3][];
            byte[][] notVerifiedHash = new byte[3][];
            sha256NotVerified.CalculateSha256(pb[0]);
            sha256NotVerified.CalculateSha256(pb[1]);
            notVerifiedHash[0] = sha256NotVerified.CalculateSha256(pb[2]);
            sha256NotVerified.CalculateSha256(pb[3]);
            sha256NotVerified.CalculateSha256(pb[4]);
            notVerifiedHash[1] = sha256NotVerified.CalculateSha256(pb[5]);
            sha256NotVerified.CalculateSha256(pb[6]);
            notVerifiedHash[2] = sha256NotVerified.CalculateSha256(pb[7]);

            byte[] bytes2 = new byte[8];
            Array.Copy(bytes, bytes2, 8);
            verifiedHash[0] = sha256Verified.ComputeHash(bytes2);
            Array.Copy(bytes, 8, bytes2, 0, 8);
            verifiedHash[1] = sha256Verified.ComputeHash(bytes2);
            bytes2 = new byte[4];
            Array.Copy(bytes, 16, bytes2, 0, 4);
            verifiedHash[2] = sha256Verified.ComputeHash(bytes2);

            bool bEqual = true;
            for (int i = 0; i < 3; i++)
            {
                if (verifiedHash[i].Length != notVerifiedHash[i].Length)
                {
                    bEqual = false;
                }
            }

            for (int i = 0; i < 3; i++)
            {
                int length = verifiedHash[i].Length;
                for (int j = 0; j < length; j++)
                {
                    if (verifiedHash[i][j] != notVerifiedHash[i][j])
                        bEqual = false;
                }
            }

            Assert.AreEqual(true, bEqual);
        }
        [TestMethod]
        public void CalculateSha256PartBlock2Test()
        {
            Stream stream = BlockReader.GetInputStream("C:/test/test.avi");
            PartBlockReader blockReader = new PartBlockReader(stream, 100097152, 2097152);

            long length = stream.Length - 9 * 100097152;
            int count = (int)Math.Ceiling((double)length / 2097152);
            PartBlock[] pb = new PartBlock[count];
            SHA256 sha256NotVerified = new SHA256();
         
            for (int i=0;i<count;i++)
            {
                pb[i] = (PartBlock)blockReader.GetPartBlock(9, i);
            }

            for (int i = 0; i < count-1; i++)
            {
                sha256NotVerified.CalculateSha256(pb[i]);
            }
            SHA256Managed sha256Verified = new SHA256Managed();
            byte[] notVerifiedHash;
            notVerifiedHash = sha256NotVerified.CalculateSha256(pb[count-1]);            
            FullBlockReader fullBlockReader = new FullBlockReader(BlockReader.GetInputStream("C:/test/test.avi"), 100097152);
            FullBlock block=new FullBlock();
            for (int i = 0; i < 10; i++)
            {
                block=(FullBlock)fullBlockReader.GetNextBlock();
            }
            byte[] verifiedHash = sha256Verified.ComputeHash(block.Data);

            bool bEqual = true;

                if (verifiedHash.Length != notVerifiedHash.Length)
                {
                    bEqual = false;
                }            
                int length1 = verifiedHash.Length;
                for (int j = 0; j < length1; j++)
                {
                    if (verifiedHash[j] != notVerifiedHash[j])
                        bEqual = false;
                }           
            Assert.AreEqual(true, bEqual);
        }
    }
}
