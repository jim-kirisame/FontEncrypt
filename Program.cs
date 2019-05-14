using System;
using System.IO;
using System.Text;
using System.Xml;
using Newtonsoft.Json;

namespace StrReplacer
{
    class Program
    {
        const string rep_config = "test.rep";
        const string font_output = "font.new.ttx";
        const string help = @"用法: 
处理字体： replacer font <input.ttx> [output.ttx]
处理文本： replacer text [decode]";
        static void Main(string[] args)
        {
            if (args.Length == 0)
            {
                Console.WriteLine(help);
                return;
            }

            var table = new ReplaceTable();
            if (File.Exists(rep_config))
            {
                table.LoadList(rep_config);
            }
            else
            {
                table.GenerateList();
                table.WriteList(rep_config);
            }

            string command = args[0];
            switch (command)
            {
                case "font":
                    if (args.Length <= 1)
                    {
                        Console.WriteLine(help);
                        return;
                    }
                    string input = args[1];
                    string output = font_output;
                    if (args.Length == 3)
                    {
                        output = args[2];
                    }

                    FontHandler fontHandler = new FontHandler(table);
                    fontHandler.Open(input, output);

                    break;
                case "text":
                    bool decode = false;
                    if (args.Length == 2 && args[1] == "decode")
                        decode = true;

                    Console.WriteLine((decode ? "文本解密模式" : "文本加密模式") + " 请在此处输入文字: ");

                    var rep = new Replacer(table);
                    while (true)
                    {
                        var str = Console.ReadLine();
                        if (str == "") break;
                        if (decode)
                            Console.WriteLine(rep.Decode(str));
                        else
                            Console.WriteLine(rep.Encode(str));
                    }
                    break;
                default:
                    Console.WriteLine(help);
                    return;
            }
        }
    }

    class Replacer
    {
        ReplaceTable table;

        public Replacer(ReplaceTable table)
        {
            this.table = table;
        }

        public string Encode(string str)
        {
            var utf8 = Encoding.UTF8;
            var chars = utf8.GetChars(utf8.GetBytes(str));

            char[] resultChar = new char[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                resultChar[i] = table.Encode(chars[i]);
            }
            return new string(resultChar);
        }

        public string Decode(string str)
        {
            var utf8 = Encoding.UTF8;
            var chars = utf8.GetChars(utf8.GetBytes(str));

            char[] resultChar = new char[chars.Length];
            for (int i = 0; i < chars.Length; i++)
            {
                resultChar[i] = table.Decode(chars[i]);
            }
            return new string(resultChar);
        }
    }

    class FontHandler
    {
        ReplaceTable table;

        public FontHandler(ReplaceTable table)
        {
            this.table = table;
        }

        public void Open(string input, string output)
        {
            Console.WriteLine("Loading font file...");

            var doc = new XmlDocument();
            doc.Load(input);

            var font = doc["ttFont"];
            var cmap = font["cmap"];
            var cmap_4 = cmap["cmap_format_4"];

            var node = cmap_4.FirstChild;

            Console.WriteLine("Proccessing font file...");
            while (node != null)
            {
                var attrs = node.Attributes;
                if (attrs != null)
                {
                    foreach (XmlAttribute attr in attrs)
                    {
                        if (attr.Name == "code")
                        {
                            var val = StrToChar(attr.Value);
                            if (table.EncodeExist(val))
                            {
                                attr.Value = CharToStr(table.Encode(val));
                            }
                            break;
                        }
                    }
                }
                node = node.NextSibling;
            }

            var name = font["name"];
            node = name.FirstChild;
            while (node != null)
            {
                if (node.Name == "namerecord")
                {
                    node.InnerText = node.InnerText + "_ENC";
                }
                node = node.NextSibling;
            }

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            using (var writer = XmlWriter.Create(output, settings))
            {
                doc.WriteTo(writer);
            }
        }

        public string CharToStr(char c)
        {
            return "0x" + ((int)c).ToString("x");
        }
        public char StrToChar(string str)
        {
            return (char)Convert.ToInt32(str, 16);
        }
    }
}
