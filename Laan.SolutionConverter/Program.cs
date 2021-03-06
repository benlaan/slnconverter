﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

using CommandLine;
using CommandLine.Text;

namespace Laan.SolutionConverter
{
    public enum Mode { Xml, Sln }

    public class Options
    {
        [Option('i', "input", Required = true, HelpText = "Input file to read")]
        public string InputFile { get; set; }

        [Option('o', "output", Required = false, HelpText = "Name of output")]
        public string OutputFile { get; set; }

        [Option('m', "mode", Required = true, HelpText = "Indicates the convert mode. Sln -> Xml or Xml -> Sln")]
        public Mode Mode { get; set; }
    }

    public class Program
    {
        private static void VerifyPath(string path, string extension)
        {
            if (String.IsNullOrEmpty(path))
                throw new ArgumentException("file not supplied as an argument");

            if (!File.Exists(path))
                throw new ArgumentException("file not found");

            if (Path.GetExtension(path) != "." + extension)
                throw new ArgumentException(String.Format("file must be a VS solution file (.{0})", extension));
        }

        private static string ConvertInput(string path)
        {
            return Path.Combine(Path.GetDirectoryName(path), Path.GetFileNameWithoutExtension(path) + ".xml");
        }

        private static void ConvertToXml(Options options)
        {
            string path = options.InputFile;
            VerifyPath(path, "sln");

            var tokenizer = new SlnTokenizer(File.ReadAllText(path));
            tokenizer.Initialise();
            tokenizer.SetSkip(true, TokenType.WhiteSpace);

            SolutionParser solutionParser = new SolutionParser(tokenizer);
            var document = solutionParser.Execute();

            var converter = new SlnToXmlConverter();
            string outputName = !String.IsNullOrEmpty(options.OutputFile) ? options.OutputFile : ConvertInput(path);
            converter.WriteDocument(document, outputName);
        }

        private static void ConvertToSln(Options options)
        {
            string path = options.InputFile;
            VerifyPath(path, "xml");

            var converter = new XmlToSlnConverter();
            string outputName = !String.IsNullOrEmpty(options.OutputFile) ? options.OutputFile : ConvertInput(path);

            converter.WriteDocument(path, outputName);
        }

        private static void Main(string[] args)
        {
            try
            {
                var options = new Options();
                if (!Parser.Default.ParseArguments(args, options))
                {
                    var helpText = new HelpText("Laan SolutionConverter", Assembly.GetEntryAssembly().GetName().Version.ToString());

                    helpText.AddOptions(options);
                    Console.WriteLine(helpText.ToString());
                    return;
                }

                switch (options.Mode)
                {
                    case Mode.Xml:
                        ConvertToXml(options);
                        break;

                    case Mode.Sln:
                        ConvertToSln(options);
                        break;
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }
    }
}
