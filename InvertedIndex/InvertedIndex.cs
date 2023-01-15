using Newtonsoft.Json;
using System.Diagnostics;
using System.Text;

namespace InvertedIndex;

public class InvertedIndex
{
    private Dictionary<string, Dictionary<string, int>> _invertedIndex = new Dictionary<string, Dictionary<string, int>>();
    Mutex mutex = new Mutex();
    public readonly int ThreadCount = 4;
    public readonly string SavedIndexPath = @"C:\inverted_index\saved_index.json";

    public void BuildIndex(string directory, string[] separators)
    {
        var subfolders = new string[] { @"\test\neg", @"\test\pos", @"\train\neg", @"\train\pos", @"\train\unsup" };
        string[] allFiles = subfolders
            .Select(file => Directory.EnumerateFiles(directory + file))
            .SelectMany(x => x)
            .ToArray();


        Thread[] threads = new Thread[ThreadCount];
        for (var i = 0; i < ThreadCount; i++)
        {
            int startIndex = allFiles.Length / ThreadCount * i;
            int endIndex = i == ThreadCount - 1 ? allFiles.Length : allFiles.Length / ThreadCount * (i + 1);

            threads[i] =
                new Thread(() =>
                {
                    for (var i = startIndex; i < endIndex; i++)
                    {
                        mutex.WaitOne();
                        string file = allFiles[i];
                        string[] content = File
                            .ReadAllText(file)
                            .ToLower()
                            .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                            .ToArray();
                        AddToIndex(content, file.Replace(directory, ""));
                        mutex.ReleaseMutex();
                    }
                });

            threads[i].Start();
        }

        for (var i = 0; i < ThreadCount; i++)
        {
            threads[i].Join();
        }
    }

    void AddToIndex(string[] words, string fileName)
    {
        foreach (var word in words)
        {
            if (!_invertedIndex.ContainsKey(word))
            {
                _invertedIndex.Add(word, new Dictionary<string, int> { { fileName, 1 } });
            }
            else if (!_invertedIndex[word].ContainsKey(fileName))
            {
                _invertedIndex[word].Add(fileName, 1);
            }
            else
            {
                _invertedIndex[word][fileName]++;
            }
        }
    }

    public string SearchIndex(string word)
    {
        var timer = new Stopwatch();
        StringBuilder result = new StringBuilder();
        int totalCount = 0;

        timer.Start();
        result.Append($"Word \"{word}\" was found in such files:\n");
        if (_invertedIndex.ContainsKey(word))
        {
            foreach (var countByFile in _invertedIndex[word])
            {
                result.Append($"{countByFile.Key} - {countByFile.Value} times \n");
                totalCount += countByFile.Value;
            }
        }

        timer.Stop();
        result.Append($"Total count: {totalCount} results in {timer.ElapsedMilliseconds} milliseconds\n");

        return result.ToString();
    }

    public void SaveIndex()
    {
        string jsonString = JsonConvert.SerializeObject(_invertedIndex, Formatting.Indented);
        File.WriteAllText(SavedIndexPath, jsonString);
    }

    public void ReadIndex()
    {
        string jsonString = File.ReadAllText(SavedIndexPath);
        _invertedIndex = JsonConvert.DeserializeObject<Dictionary<string, Dictionary<string, int>>>(jsonString);
    }
}