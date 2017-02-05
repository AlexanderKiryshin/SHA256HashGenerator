using SHA256HashGenerator.BlockReaders;
using SHA256HashGenerator.Processors;
using System;
using System.Diagnostics;
using System.IO;

namespace SHA256HashGenerator
{
    class Program
    {
        private AbstractProcessor processor;
        private Stream inputStream;
        private bool isCanceled;
        static void Main(string[] args)
        {          
            OutputConsole.StartMessage();
            Console.ReadKey();
            Console.WriteLine();
            Options options = new Options();
           try
            { 
                CommandLine.Parser.Default.ParseArguments(args, options);
            }
            catch(Exception ex)
            {
                OutputConsole.DisplayError(ex);
                OutputConsole.ExitMessage();
                Console.ReadKey();            
                return;
            }

            if (string.IsNullOrEmpty(options.InputFileName))
            {
                Console.WriteLine("Не указано имя файла");
                Console.ReadKey();
                return;
            }
            if (options.BlockSize <= 0)
            {
                Console.WriteLine("Размер блока указан неверно");
                Console.ReadKey();
                return;
            }
            Program program = new Program();
            Console.CancelKeyPress += program.Handler;
            program.Run(options);
            if (!program.isCanceled)
            { 
                OutputConsole.ExitMessage();
            }
            Console.ReadKey();
        }

        private void Run(Options options)
        {
            try
            {
                inputStream = BlockReader.GetInputStream(options.InputFileName);
            }
            catch (FileNotFoundException)
            {
                Console.WriteLine("Файл не найден");
                return;
            }
            catch (FileLoadException ex)
            {
                OutputConsole.DisplayError(ex);
                return;
            }
            processor = null;
            bool bBlockNeedFragment = options.BlockSize > BlockReader.LIMIT_DATA_IN_ONE_BLOCK;
            if (bBlockNeedFragment)
            {
                processor = new PartBlocksProcessor(inputStream, options.BlockSize, BlockReader.LIMIT_DATA_IN_ONE_BLOCK);
                processor.Run();
            }
            else
            {
                processor = new FullBlocksProcessor(inputStream, options.BlockSize);
                processor.Run();
            }
            
            BlockReader.DisposeStream(inputStream);
        }
          
        /// <summary>
        /// Событие отмены работы программы
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="args"></param>
        private void Handler(object sender, ConsoleCancelEventArgs args)
        {
            isCanceled = true;
            if (processor != null)
            {
                processor.Abort();
            }
            args.Cancel = true;
            OutputConsole.ExitMessage();
        }
    }
}
