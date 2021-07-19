using System;
using System.IO;
using System.Threading.Tasks;

namespace TranslateJsonFlatten
{
    class Program
    {
        static async Task Main(string[] args)
        {
            var fi = new FileInfo(args.Length > 0 ? args[0] : throw new ArgumentException("Please provide file"));
            if (!fi.Exists)
            {
                throw new ArgumentException("Unable to locate file");
            }

            switch (fi.Extension.ToLower())
            {
                case ".json":
                    var jsonText = File.ReadAllText(fi.FullName);
                    var csvLines = new TranslationFileConversionService().ConvertToCsv(jsonText, "Key,English,Welsh");
                    var csvOutputFile = GetOutputFile(args[0], ".csv");
                    File.WriteAllLines(csvOutputFile.FullName, csvLines);
                    break;
                case ".csv":
                    var lines = File.ReadAllLines(fi.FullName);
                    var jsonString = new TranslationFileConversionService().ConvertToJson(lines);
                    var jsonOutputFile = GetOutputFile(args[0], ".json");
                    File.WriteAllText(jsonOutputFile.FullName, jsonString);
                    break;
                default:
                    throw new ArgumentException($"Invalid file type {fi.Extension}");
            }
            

        }

        static FileInfo GetOutputFile(string inputPath, string outputExtension)
        {
            var inputFi = new FileInfo(inputPath);
            var outputFile = new FileInfo(inputFi.Directory.FullName + "\\" + inputFi.Name.Replace(inputFi.Extension, outputExtension));
            outputFile.Delete();
            return outputFile;
        }
    }
}
