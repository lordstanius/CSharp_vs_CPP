using System;
using System.IO;
using System.Collections;

namespace NS
{
    class Program
    {
        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceCounter(out long lpPerformanceCount);

        [System.Runtime.InteropServices.DllImport("Kernel32.dll")]
        private static extern bool QueryPerformanceFrequency(out long lpFrequency);

        static void Main(string[] args)
        {
            QueryPerformanceFrequency(out long freq);
            QueryPerformanceCounter(out long startTime);

            var dict = new TestOptimized.Dictionary(); // ~100 ms (release)
            //var dict = new Test.Dictionary(); // ~150 ms (release)

            QueryPerformanceCounter(out long endTime);

            Console.WriteLine("Length: {0}", dict.Length());
            Console.WriteLine("frequency: {0:n0}", freq);
            Console.WriteLine("time: {0:n5}s", (endTime - startTime) / (double)freq);
        }
    }

    class TestOptimized
    {
        public class DictionaryEntry
        {
            private string trad;
            private string pinyin;
            public string english;

            static public DictionaryEntry ParseBuffer(char[] line, int ibIn, int ibMax, out int ibOut)
            {
                DictionaryEntry de = new DictionaryEntry();
                int state = 0;
                char chTrans = ' ';
                int start = ibIn;
                int end = 0;
                int i;

                // there is still a newline or we would not be here
                for (i = ibIn; line[i] != '\n' && i < ibMax; i++)
                {
                    if (line[i] != chTrans)
                        continue;
                    switch (state)
                    {
                        case 0:
                            de.trad = new string(line, start, i - start);
                            state = 1;
                            chTrans = '[';
                            break;

                        case 1:
                            start = i + 1;
                            chTrans = ']';
                            state = 2;
                            break;

                        case 2:
                            de.pinyin = new string(line, start, i - start);
                            chTrans = '/';
                            state = 3;
                            break;

                        case 3:
                            start = i + 1;
                            state = 4;
                            break;

                        case 4:
                            end = i;
                            state = 5;
                            break;

                        case 5:
                            end = i;
                            break;
                    }
                }

                ibOut = i;
                if (state == 5)
                {
                    de.english = new string(line, start, end - start);
                    return de;
                }
                return null;
            }
        }

        public class Dictionary
        {
            ArrayList dict;

            public Dictionary()
            {
                StreamReader src = new StreamReader("cedict_ts.u8", System.Text.Encoding.UTF8);
                DictionaryEntry de;
                dict = new ArrayList();
                char[] buffer = new char[9 * 1024 * 1024];
                int cb = src.Read(buffer, 0, buffer.Length);
                int ib = 0;
                while (ib < cb)
                {
                    if (buffer[ib] == '#')
                    {
                        // there must still be a newline or we wouldn't be here
                        while (buffer[ib] != '\n')
                            ib++;
                    }
                    else
                    {
                        de = DictionaryEntry.ParseBuffer(buffer, ib, cb, out int ibOut);
                        ib = ibOut;
                        if (de != null)
                            dict.Add(de);
                    }
                    ib++;
                }
            }
            public int Length() { return dict.Count; }
        };
    }

    class Test
    {
        public class DictionaryEntry
        {
            private string trad;
            private string pinyin;
            private string english;

            static public DictionaryEntry Parse(string line)
            {
                DictionaryEntry de = new DictionaryEntry();

                int start = 0;
                int end = line.IndexOf(' ', start);

                if (end == -1) return null;
                de.trad = line.Substring(start, end - start);

                start = line.IndexOf('[', end);
                if (start == -1) return null;

                end = line.IndexOf(']', ++start);

                if (end == -1) return null;

                de.pinyin = line.Substring(start, end - start);

                start = line.IndexOf('/', end);

                if (start == -1) return null;
                start++;

                end = line.LastIndexOf('/');
                if (end == -1) return null;
                if (end <= start) return null;

                de.english = line.Substring(start, end - start);

                return de;
            }
        };

        public class Dictionary
        {
            ArrayList dict;

            public Dictionary()
            {
                StreamReader src = new StreamReader("cedict_ts.u8", System.Text.Encoding.UTF8);
                string s;
                DictionaryEntry de;
                dict = new ArrayList();

                while ((s = src.ReadLine()) != null)
                {
                    if (s.Length > 0 && s[0] != '#')
                    {
                        if (null != (de = DictionaryEntry.Parse(s)))
                        {
                            dict.Add(de);
                        }
                    }
                }
            }

            public int Length() { return dict.Count; }
        };
    }
}