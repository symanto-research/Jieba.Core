using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Symanto.Jieba.Net.Core.Common;

namespace Symanto.Jieba.Net.Core
{
    public class WordDictionary
    {
        private static readonly Lazy<WordDictionary> LazyInstance = new Lazy<WordDictionary>(() => new WordDictionary());
        private static readonly string MainDict = ConfigManager.MainDictFile;

        internal readonly IDictionary<string, int> Trie = new Dictionary<string, int>();

        /// <summary>
        /// total occurrence of all words.
        /// </summary>
        public double Total { get; set; }

        private WordDictionary()
        {
            LoadDict();

            Debug.WriteLine("{0} words (and their prefixes)", Trie.Count);
            Debug.WriteLine("total freq: {0}", Total);
        }

        public static WordDictionary Instance => LazyInstance.Value;

        private void LoadDict()
        {
            try
            {
                var stopWatch = new Stopwatch();
                stopWatch.Start();
                var filePath = ConfigManager.MainDictFile;
                
                
                using (var sr = new StreamReader(filePath))
                {
                    string line = null;
                    while ((line = sr.ReadLine()) != null)
                    {
                        var tokens = line.Split(' ');
                        if (tokens.Length < 2)
                        {
                            Debug.Fail($"Invalid line: {line}");
                            continue;
                        }

                        var word = tokens[0];
                        var freq = int.Parse(tokens[1]);

                        Trie[word] = freq;
                        Total += freq;

                        foreach (var ch in Enumerable.Range(0, word.Length))
                        {
                            var wfrag = word.Sub(0, ch + 1);
                            if (!Trie.ContainsKey(wfrag))
                            {
                                Trie[wfrag] = 0;
                            }
                        }
                    }
                }

                stopWatch.Stop();
                Debug.WriteLine("main dict load finished, time elapsed {0} ms", stopWatch.ElapsedMilliseconds);
            }
            catch (IOException e)
            {
                Debug.Fail($"{MainDict} load failure, reason: {e.Message}");
            }
            catch (FormatException fe)
            {
                Debug.Fail(fe.Message);
            }
        }

        public bool ContainsWord(string word)
        {
            return Trie.ContainsKey(word) && Trie[word] > 0;
        }

        public int GetFreqOrDefault(string key)
        {
            return ContainsWord(key) ? Trie[key] : 1;
        }

        public void AddWord(string word, int freq, string tag = null)
        {
            if (ContainsWord(word))
            {
                Total -= Trie[word];
            }

            Trie[word] = freq;
            Total += freq;
            for (var i = 0; i < word.Length; i++)
            {
                var wfrag = word.Substring(0, i + 1);
                if (!Trie.ContainsKey(wfrag))
                {
                    Trie[wfrag] = 0;
                }
            }
        }

        public void DeleteWord(string word)
        {
            AddWord(word, 0);
        }

        internal int SuggestFreq(string word, IEnumerable<string> segments)
        {
            double freq = 1;
            foreach (var seg in segments)
            {
                freq *= GetFreqOrDefault(seg) / Total;
            }

            return Math.Max((int)(freq * Total) + 1, GetFreqOrDefault(word));
        }
    }
}