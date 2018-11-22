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
        public int number0;

        public struct HUFFCODE
        {
            public byte content;
            public string codeString;
        }

        public HUFFCODE[] huffcode;

        public byte[] locateByte = new byte[8];

        public HUFFDECOMPRESS()
        {
            for (int i = 0; i < 8; i++)
            {
                int temp = 1;
                locateByte[i] = Convert.ToByte(temp << i);
            }
        }

        public int minCodeStringLength;

        public void decodeFile(string address)
        {
            byte[] temp = beginDecode(address);
            writeFile(address, temp);
        }

        public byte[] beginDecode(string address)
        {
            FileStream fs = new FileStream(address, FileMode.Open);
            long size = fs.Length;
            byte[] arrays = new byte[size];
            fs.Read(arrays, 0, arrays.Length);
            fs.Close();


            validCount = arrays[0] + 1;
            number0 = arrays[1];
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

            Dictionary<string, byte> dic = new Dictionary<string, byte>();
            dic = createDictionary(huffcode);

            ///////////////////////////////////////
            ///开始解压
            string currentString = null;
            int mark = 8;
            byte currentByte;
            List<byte> afterDecode = new List<byte>();
            while (ptr < arrays.Length)
            {
                if (ptr == arrays.Length - 1)
                {
                    if (mark == number0)
                        break;
                }

                currentString = currentString + getCurrentString(arrays[ptr], mark);
                mark--;
                if (currentString.Length >= minCodeStringLength)
                {
                    try
                    {
                        currentByte = dic[currentString];
                        afterDecode.Add(currentByte);
                        currentString = null;
                    }
                    catch
                    {
                    }
                }
                if (mark == 0)
                {
                    ptr++;
                    mark = 8;
                }

            }

            return afterDecode.ToArray();

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

        public Dictionary<string, byte> createDictionary(HUFFCODE[] huffcode)
        {
            Dictionary<string, byte> dic = new Dictionary<string, byte>();
            minCodeStringLength = -1;
            foreach (HUFFCODE temp in huffcode)
            {
                dic.Add(temp.codeString, temp.content);
                if (minCodeStringLength == -1)
                {
                    minCodeStringLength = temp.codeString.Length;
                }
                else if (minCodeStringLength > temp.codeString.Length)
                {
                    minCodeStringLength = temp.codeString.Length;
                }
            }
            return dic;
        }

        public string getCurrentString(byte currentByte, int locate) //判断当前字节第locate位是1还是0；
        {
            string result = null;
            int tempResult = currentByte & locateByte[locate - 1];
            if (tempResult == 0)
                result = "0";
            else
                result = "1";
            return result;
        }


    }
}
