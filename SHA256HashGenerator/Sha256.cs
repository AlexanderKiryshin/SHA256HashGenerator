using SHA256HashGenerator.DataClass;
using System;
using System.Collections;
using System.Security.Cryptography;

namespace SHA256HashGenerator
{
    /// <summary>
    /// Вычисляет SHA256 хэш для входных данных
    /// </summary>
    public class SHA256
    {
        private static object lockObj = new object();
        private static SortedList sha256DataList;

        public SHA256()
        {
            sha256DataList = new SortedList();
            sha256DataList = SortedList.Synchronized(sha256DataList);
        }
        /// <summary>
        ///Рассчитывает SHA256 для блока предварительно разделенного на части
        /// </summary>
        /// <param name="partBlock">Часть блока для которого расчитывается хеш</param>
        /// <param return>Возвращает рассчитанный хеш </param>
        public byte[] CalculateSha256(PartBlock partBlock)
        {
            int index = sha256DataList.IndexOfKey(partBlock.IdFullBlock);
            SHA256Data sha256Data = null;
            if (index >= 0)
            {
                sha256Data = (SHA256Data)sha256DataList.GetByIndex(index);
            }
            if (sha256Data == null)
            {
                sha256Data = new SHA256Data();
              
                sha256Data.Sha256 = new SHA256Managed();
                lock (lockObj)
                {
                   sha256DataList.Add(partBlock.IdFullBlock, sha256Data);
                }
            }
            if (partBlock.SizeFullBlock != 0)
            {
                sha256Data.BlockSize = partBlock.SizeFullBlock;
            }
            sha256Data.Sha256.TransformBlock(partBlock.Data, 0, partBlock.Size, partBlock.Data, 0);
            sha256Data.CurrentCalculated += partBlock.Size;
            bool bBlockCalculateIsOver = sha256Data.CurrentCalculated == sha256Data.BlockSize;
            if ((sha256Data.BlockSize != 0)
                && (bBlockCalculateIsOver))
            {
                byte[] input = new byte[0];
                sha256Data.Sha256.TransformFinalBlock(input, 0, 0);
                sha256DataList.Remove(partBlock.IdFullBlock);
                return sha256Data.Sha256.Hash;
            }
            return new byte[0];
        }
        /// <summary>
        /// Рассчитывает SHA256 для блока 
        /// </summary>
        /// <param name="block">Часть блока для которого расчитывается хеш</param>
        /// <param return>Возвращает рассчитанный хеш </param>
        public byte[] CalculateSha256(FullBlock block)
        {
            var sha256hasher = new SHA256Managed();
            sha256hasher.ComputeHash(block.Data, 0, block.Size);
            return sha256hasher.Hash;
        }
    }
}
