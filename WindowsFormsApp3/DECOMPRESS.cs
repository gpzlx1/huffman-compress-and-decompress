using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace dehuff
{
    class HUFFDECOMPRESS
    {
        public int validCount;
        public int mark;

        public struct HUFFCODE
        {
            public byte content;
            public string codeString;
        }

        public HUFFCODE[] huffcode;

        public void decodeFile(string address)
        {
            FileStream fs = new FileStream(address, FileMode.Open);
            long size = fs.Length;
            byte[] arrays = new byte[size];
            fs.Read(arrays, 0, arrays.Length);
            fs.Close();

            validCount = arrays[0] + 1;
            mark = arrays[1];
            int ptr = 2;
            huffcode = new HUFFCODE[validCount];
            for (int i = 0; i < validCount; i++)
            {
                int j = 0;
                huffcode[i].content = arrays[ptr++];
                j = arrays[ptr++];
                byte[] temp = new byte[j];
                for (int k = 0; k < j; k++)
                {
                    temp[k] = arrays[ptr++];
                }
                huffcode[i].codeString = System.Text.Encoding.Default.GetString(temp);
            }

            //writeFile(address, arrays);
        }

        public void writeFile(string address, byte[] contents)
        {
            int index1 = address.LastIndexOf('.');
            address = address.Substring(0, index1);
            int index2 = address.LastIndexOf('.');
            address = address.Insert(index2, "_copy");
            FileStream F = new FileStream(address, FileMode.Create);
            F.Write(contents, 0, contents.Length);
            F.Close();
        }
    }
}
