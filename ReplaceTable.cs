using System;
using System.IO;
using System.Collections.Generic;

namespace StrReplacer
{
    class ReplaceTable
    {
        Dictionary<char, char> normalToRandom = new Dictionary<char, char>();
        Dictionary<char, char> randomToNormal = new Dictionary<char, char>();

        const int CJK_START = 0x4E00;
        const int CJK_END = 0x9FBF;
        public void GenerateList()
        {
            int size = CJK_END - CJK_START + 1;
            char[] list = new char[size];

            // generate
            for (int i = 0; i < size; i++)
            {
                list[i] = (char)(i + CJK_START);
            }

            // shuffle
            for (int i = 0; i < size; i++)
            {
                char temp = list[i];
                int rand = new Random().Next(size);
                list[i] = list[rand];
                list[rand] = temp;
            }

            normalToRandom.Clear();
            randomToNormal.Clear();
            for (int i = 0; i < size; i++)
            {
                normalToRandom.Add((char)(i + CJK_START), list[i]);
                randomToNormal.Add(list[i], (char)(i + CJK_START));
            }
        }

        public void LoadList(string file)
        {
            using (var stream = File.OpenText(file))
            {
                randomToNormal.Clear();
                normalToRandom.Clear();
                while (!stream.EndOfStream)
                {
                    var str = stream.ReadLine();
                    var strs = str.Split(' ');

                    if (strs.Length == 2)
                    {
                        int from = int.Parse(strs[0]);
                        int to = int.Parse(strs[1]);

                        randomToNormal.Add((char)from, (char)to);
                        normalToRandom.Add((char)to, (char)from);
                    }
                }
            }
        }

        public void WriteList(string file)
        {
            string[] strs = new string[normalToRandom.Count];

            int index = 0;
            foreach (var item in normalToRandom)
            {
                strs[index++] = $"{(int)item.Key} {(int)item.Value}";
            }

            File.WriteAllLines(file, strs);
        }

        public char Encode(char c)
        {
            if (normalToRandom.ContainsKey(c))
                return normalToRandom[c];
            else
                return c;
        }

        public char Decode(char c)
        {
            if (randomToNormal.ContainsKey(c))
                return randomToNormal[c];
            else
                return c;
        }

        public bool EncodeExist(char c)
        {
            return normalToRandom.ContainsKey(c);
        }

        public bool DecodeExist(char c)
        {
            return randomToNormal.ContainsKey(c);
        }
    }
}
