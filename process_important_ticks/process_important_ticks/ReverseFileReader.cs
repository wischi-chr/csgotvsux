using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace process_important_ticks
{
    public static class RevFile
    {
        public static IEnumerable<string> ReadLines(FileStream fs, Encoding enc = null)
        {
            int blocksize = 128;
            var row = new byte[blocksize];
            bool CrLf = false;
            if (enc == null) enc = Encoding.UTF8;

            using (var ms = new MemoryStream())
            {
                long pos = fs.Length;
                while (pos > 0)
                {
                    int len = (int)Math.Min(blocksize, pos);
                    fs.Seek(pos - len, SeekOrigin.Begin);
                    long read = fs.Read(row, 0, len);
                    if (read == len)
                    {
                        for (int i = len - 1; i >= 0; i--)
                        {
                            byte b = row[i];
                            if (CrLf && b != 13 && b != 10)
                            {
                                byte[] line = ms.ToArray();
                                ms.SetLength(0);
                                Array.Reverse(line);
                                var str = enc.GetString(line).Trim();
                                CrLf = false;
                                yield return str;
                            }
                            if (b == 13 || b == 10) CrLf = true;
                            ms.WriteByte(b);
                        }
                        pos -= len;
                    }
                }
            }


            yield break;
        }
    }
}
