using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;

namespace Apriori {
    class Program {
        private static readonly string savePath =
            Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                         "EqDFanFic");
        static void Main(string[] args) {
            Dictionary<string, List<string>> data = (Dictionary<string, List<string>>) deserialise("storyTagData.dat");
            Apriori apriori = new Apriori(data);
            List<List<string>> frequentSets = apriori.getFrequentSets();
            string[] search = (from arg in args where arg.Substring(0, 1) != "-" select arg.ToLower()).ToArray();
            string[] unsearch = (from arg in args where arg.Substring(0, 1) == "-" select arg.Substring(1).ToLower()).ToArray();
            Dictionary<List<string>[], double> strongRules = apriori.generateStrongRules(frequentSets);
            foreach (KeyValuePair<List<string>[], double> keyValuePair in strongRules) {
                List<string> key0 = keyValuePair.Key[0];
                List<string> key1 = keyValuePair.Key[1];
                key0 = (from key in key0 select key.ToLower()).ToList();
                key1 = (from key in key1 select key.ToLower()).ToList();
                bool matchesSearch = (key0.Intersect(search).Count() > 0 || key1.Intersect(search).Count() > 0) || search.Length==0;
                bool matchesUnsearch = (key0.Intersect(unsearch).Count() > 0 || key1.Intersect(unsearch).Count() > 0);
                if ((matchesSearch && !matchesUnsearch) || args.Length==0) {
                    if (key0.Count==1 && key1.Count==1)
                    Console.WriteLine(String.Format(
                        @"{0},{1},{2}",
                        String.Join(" ", keyValuePair.Key[0]), String.Join(" ", keyValuePair.Key[1]),
                        Math.Round(keyValuePair.Value * 100, 6)));
                }
            }
        }

        private static object deserialise(string filepath) {
            string path = Path.Combine(savePath, filepath);
            object thing = null;
            if (File.Exists(path)) {
                using (Stream readStream = File.Open(path, FileMode.Open)) {
                    thing = new BinaryFormatter().Deserialize(readStream);
                }
            }
            return thing;
        }
    }
}
