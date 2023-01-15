using System.Diagnostics;

namespace InvertedIndex;

internal class Program
{
    static void Main(string[] args)
    {
        string[] separators = { ".", ",", "<br", " ", ":", ";", "/>", "<br/>", "\"", "?", "!" };
        string folderPath = @"C:\inverted_index\data";

        InvertedIndex invertedIndex = new InvertedIndex();
        bool indexExist = false;
        if (File.Exists(invertedIndex.SavedIndexPath))
        {
            Console.WriteLine("Saved inverted index found. Reading...");
            try
            {
                invertedIndex.ReadIndex();
                indexExist = true;
            }
            catch
            {
                Console.WriteLine("Error trying to read saved index");
            }
        }

        if (!indexExist)
        {
            var timer = new Stopwatch();
            timer.Start();
            invertedIndex.BuildIndex(folderPath, separators);
            timer.Stop();
            Console.WriteLine($"Inverted index built in {timer.ElapsedMilliseconds} milliseconds using {invertedIndex.ThreadCount} threads");

            invertedIndex.SaveIndex();
            Console.WriteLine("Inverted index saved to file");
        }

        while (true)
        {
            Console.Write("Enter word to search -> ");
            Console.WriteLine(invertedIndex.SearchIndex(Console.ReadLine()));
            Console.WriteLine("Do you want to continue? (y/n)");
            if (!(Console.ReadLine().ToLower() == "y"))
            {
                break;
            }
        }
    }
}