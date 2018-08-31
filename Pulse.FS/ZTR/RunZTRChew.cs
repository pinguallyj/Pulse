using Pulse.Core;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Pulse.FS.ZTR
{
    static class RunZTRChew
    {

        public static void readJapaneseZTR()
        {
            String[] allfiles = System.IO.Directory.GetFiles(@"C:\LR\txtres\event\ev_edaa_200", "*_jp.ztr", System.IO.SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                Stream fs = File.OpenRead(file);
                FFXIIITextEncoding encoding = FFXIIITextEncodingFactory.CreateJpn();
                ZtrFileUnpacker unpacker = new ZtrFileUnpacker(fs, encoding);
                ZtrFileEntry[] entries = unpacker.Unpack();
                String outputfile = file.Substring(0, file.Length - 4) + ".txt";
                using (FileStream fs2 = File.OpenWrite(outputfile))
                {
                    StreamWriter sw = new StreamWriter(fs2);
                    foreach (ZtrFileEntry entry in entries)
                    {
                        sw.WriteLine("");
                        if (entry.IsAnimatedText)
                            sw.WriteLine(entry.Key + " | " + entry.Value + " | Is Animated? " + entry.IsAnimatedText);
                        else
                            sw.WriteLine(entry.Key + " | " + entry.Value);
                    }
                    sw.Flush();
                    sw.Close();

                }
            }
        }

        public static void readEnglishZTR()
        {
            String[] allfiles = System.IO.Directory.GetFiles(@"C:\LR\HRW", "*_us.ztr", System.IO.SearchOption.AllDirectories);
            foreach (var file in allfiles)
            {
                Stream fs = File.OpenRead(file);
                FFXIIITextEncoding encoding = FFXIIITextEncodingFactory.CreateEuro();
                ZtrFileUnpacker unpacker = new ZtrFileUnpacker(fs, encoding);
                ZtrFileEntry[] entries = unpacker.Unpack();
                String outputfile = file.Substring(0, file.Length - 4) + ".txt";
                using (FileStream fs2 = File.OpenWrite(outputfile))
                {
                    StreamWriter sw = new StreamWriter(fs2);
                    foreach (ZtrFileEntry entry in entries)
                    {
                        sw.WriteLine("");
                        if (entry.IsAnimatedText)
                            sw.WriteLine(entry.Key + " | " + entry.Value + " | Is Animated? " + entry.IsAnimatedText);
                        else
                            sw.WriteLine(entry.Key + " | " + entry.Value);
                    }
                    sw.Flush();
                    sw.Close();

                }
            }
        }

        public static string Convert(byte[] bytes)
        {
            string response = string.Empty;

            foreach (byte b in bytes)
                response = response + (Char)b;

            return response;
        }

        public static void writetoZTR()
        {
            String[] allfiles = System.IO.Directory.GetFiles(@"C:\LR\ysn\", "*.txt", System.IO.SearchOption.AllDirectories);
            foreach (var file in allfiles)
                
            {
                int counter = 0;
                string line;
                List<ZtrFileEntry> entries = new List<ZtrFileEntry>();
                System.IO.StreamReader filestream = new System.IO.StreamReader(file);
                while ((line = filestream.ReadLine()) != null)
                {
                    string[] splitstring = line.Split(new[] { " | " }, StringSplitOptions.None);
                    ZtrFileEntry lineentry = new ZtrFileEntry();
                    lineentry.IsAnimatedText = false;
                    lineentry.Key = splitstring[0];
                    if (splitstring[1].Contains("#"))
                        lineentry.Value = "";

                    else
                    {
                        if (splitstring[1].Contains("%")) {
                            string[] deconstructedstring = splitstring[1].Split(new[] { "%" }, StringSplitOptions.None);
                            //byte[] charsepbytes = new byte[] { 0, 0, 1, 0 };
                            //string characterseparator = Convert(charsepbytes);
                            char charzero = '*';
                            char charone = '&';
                            string characterseparator = "";
                            StringBuilder builder = new StringBuilder(characterseparator);
                            builder.Append(charzero);
                            builder.Append(charone);
                            

                            System.Diagnostics.Debug.WriteLine(builder.ToString());
                            lineentry.Value = deconstructedstring[0] + builder.ToString() + deconstructedstring[1];


                        }
                        else { 

                        lineentry.Value = splitstring[1];
                        }

                    }

                   
                    entries.Add(lineentry);
                    System.Diagnostics.Debug.WriteLine(splitstring[1]);

                }

                ZtrFileEntry[] ztrarray = entries.ToArray();

                FFXIIITextEncoding encoding = FFXIIITextEncodingFactory.CreateEuro();
                
                String outputfile = @"C:\LR\ysn\" + Path.GetFileName(file) + ".ztr";
                using (FileStream fs2 = File.OpenWrite(outputfile))
                {
                    ZtrFilePacker packer = new ZtrFilePacker(fs2, encoding, ZtrFileType.BigEndianCompressedDictionary);
                    packer.Pack(ztrarray);
                    StreamWriter sw = new StreamWriter(fs2);
                    
                    sw.Flush();
                    sw.Close();

                }

            }
        }
    

        public static void Main(string[] args)
        {
            //readJapaneseZTR();
            writetoZTR();
            //readEnglishZTR();
        }

    }
}
