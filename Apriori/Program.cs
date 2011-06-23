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
            string search = args.Length>0 ? args[0].ToLower() : "";
            Dictionary<List<string>[], double> strongRules = apriori.generateStrongRules(frequentSets);
            foreach (KeyValuePair<List<string>[], double> keyValuePair in strongRules) {
                List<string> key0 = keyValuePair.Key[0];
                List<string> key1 = keyValuePair.Key[1];
                key0.ForEach(item => item.ToLower());
                key1.ForEach(item => item.ToLower());
                if (key0.Contains(search) || key1.Contains(search) || search=="") {
                    Console.WriteLine(String.Format(
                        @"Things tagged ""{0}"" are also tagged ""{1}"", {2}% of the time!",
                        String.Join(" ", keyValuePair.Key[0]), String.Join(" ", keyValuePair.Key[1]),
                        Math.Round(keyValuePair.Value * 100, 2)));
                }
            }



            /*
            StreamWriter writer = new StreamWriter(newPath);
            foreach (List<string> allCombination in apriori.getFrequentSets()) {
                writer.WriteLine(String.Join(", ", allCombination));
            }
            writer.Close();
            Process.Start("notepad", newPath);*/
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
