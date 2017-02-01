using CommandLine;

namespace SHA256HashGenerator
{
    internal class Options
    {
        [Option('i', "input", Required = true, HelpText = "Полный путь до входного файла")]
        public string InputFileName
        { get; set; }

        [Option('s', "size", HelpText = "Размер блока в байтах", DefaultValue = 100097152)]
        public int BlockSize
        { get; set; }
    }
}
