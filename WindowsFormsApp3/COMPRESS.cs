using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

namespace huff
{
    public class HUFFCOMPRESS
    {
        public int validCount = 0;

        public Dictionary<byte, int> idDic = new Dictionary<byte, int>();

        public struct HUFF
        {
            public byte content;
            public int weight;
            public int parent;
            public int lchild;
            public int rchild;
        }


        public struct HUFFMANCODE
        {
            public byte content;
            public byte stringLength;
            public string huffcodeString;
        }

        public HUFF[] huffTree = new HUFF[512];

        public HUFFMANCODE[] huffcode;

        public void CreateCompressedFile(string address)
        {
            byte[] output;
            FileStream fs = new FileStream(address, FileMode.Open);
            long size = fs.Length;
            byte[] arrays = new byte[size];
            fs.Read(arrays, 0, arrays.Length);
            fs.Close();



            scanHuff(arrays);
            createHuffTree();
            HuffmanCoding();


            output = beginCompress(arrays);
            /*test();
            foreach(byte temp in output)
            {
                string ok = Convert.ToString(temp, 2);
                Console.Write(ok+" ");
            }*/
            writeFile(address, output);



        }

        public void scanHuff(byte[] contents)
        {
            int i;
            validCount = 0;
            idDic.AsParallel();
            foreach (byte content in contents)
            {
                try
                {
                    
                    i = idDic[content];
                    huffTree[i].weight++;
                }
                catch
                {
                    huffTree[validCount].weight = 1;
                    huffTree[validCount].content = content;
                    idDic.Add(content, validCount++);
                    
                }
            }
        }

        public bool createHuffTree()
        {
            int end = 2 * validCount - 1;
            int min1 = -1, min2 = -1;
            int i;
            for (i = validCount; i < end; i++)  //非常可能出问题
            {
                if (findMinTwoNumber(ref min1, ref min2))
                {
                    huffTree[min1].parent = i;
                    huffTree[min2].parent = i;
                    huffTree[i].lchild = min1;
                    huffTree[i].rchild = min2;
                    huffTree[i].weight = huffTree[min1].weight + huffTree[min2].weight;
                    validCount++;
                }
                else
                    break;
            }
            if (i == end)
                return true;
            return false;
        }

        public bool findMinTwoNumber(ref int min1, ref int min2)
        {
            min1 = -1;
            min2 = -1;
            for (int i = 0; i < validCount; i++)
            {
                if (huffTree[i].parent == 0 && (min1 == -1 || huffTree[i].weight < huffTree[min1].weight))
                {
                    min1 = i;
                }
            }
            for (int i = 0; i < validCount; i++)
            {
                if (i != min1 && huffTree[i].parent == 0 && (min2 == -1 || huffTree[i].weight < huffTree[min2].weight))
                {
                    min2 = i;
                }
            }
            if (min1 == -1 || min2 == -1 || min1 == min2)
                return false;
            return true;
        }

        public bool HuffmanCoding()
        {
            int codeNumber = (validCount + 1) / 2;
            huffcode = new HUFFMANCODE[codeNumber];
            int i;
            for (i = 0; i < codeNumber; i++)
            {
                huffcode[i].content = huffTree[i].content;
                for (int c = i, f = huffTree[i].parent; f != 0; c = f, f = huffTree[f].parent)
                {
                    if (huffTree[f].lchild == c)
                        huffcode[i].huffcodeString = "0" + huffcode[i].huffcodeString;
                    else
                        huffcode[i].huffcodeString = "1" + huffcode[i].huffcodeString;
                }
                huffcode[i].stringLength = Convert.ToByte(huffcode[i].huffcodeString.Length);

                //Console.WriteLine(Convert.ToInt32(huffcode[i].huffcodeString, 2));

            }
            if (i == codeNumber + 1)
                return true;
            return false;

        }

        public void test()
        {
            int i = 0;
            foreach (HUFF temp in huffTree)
            {
                i++;
                if (temp.weight != 0)
                    Console.WriteLine("{0},{1},{2},{3}", temp.weight, temp.parent, temp.lchild, temp.rchild);
            }
            i = 0;
            Console.WriteLine();
            foreach (HUFFMANCODE temp in huffcode)
            {
                i++;
                if (temp.huffcodeString != null)
                    Console.WriteLine("{0},{1},{2}", i, temp.content, temp.huffcodeString);
            }
        }

        public byte[] beginCompress(byte[] contents)
        {
            List<byte> afterCompressed = new List<byte>();
            byte input = 0;
            byte temp;
            int KK;
            int codeLength;
            int offset;
            int mark = 8;
            string codePart;
            string debug;
            idDic.AsParallel();
            foreach (byte content in contents)
            {
                temp = 0;
                KK = idDic[content];
                codeLength = huffcode[KK].stringLength;
                offset = mark - codeLength;
                if (offset > 0)
                {
                    temp = Convert.ToByte(huffcode[KK].huffcodeString, 2);
                    temp = Convert.ToByte(temp << offset);
                    input = Convert.ToByte(input | temp);
                    mark = offset;
                }
                if (offset == 0)
                {
                    temp = Convert.ToByte(huffcode[KK].huffcodeString, 2);
                    input = Convert.ToByte(input | temp);
                    afterCompressed.Add(input);
                    input = 0;
                    mark = 8;
                }
                if (offset < 0)
                {
                    int T = 0;
                    debug = huffcode[KK].huffcodeString;

                    codePart = debug.Substring(0, mark);

                    T = mark;
                    temp = Convert.ToByte(codePart, 2);

                    input = Convert.ToByte(input | temp);
                    afterCompressed.Add(input);
                    input = 0;
                    codeLength = codeLength - mark;

                    if (codeLength == 0)
                        continue;

                    mark = 8;
                    while (codeLength > 8)
                    {
                        codePart = huffcode[KK].huffcodeString.Substring(T, 8);
                        input = Convert.ToByte(codePart, 2);
                        afterCompressed.Add(input);
                        input = 0;
                        codeLength = codeLength - 8;
                        T = T + 8;
                    }
                    codePart = huffcode[KK].huffcodeString.Substring(T);

                    temp = Convert.ToByte(codePart, 2);
                    temp = Convert.ToByte(temp << (mark - codeLength));
                    input = Convert.ToByte(input | temp);
                    mark = mark - codeLength;

                }
                if (mark == 0)
                {
                    afterCompressed.Add(input);
                    input = 0;
                    mark = 8;
                    continue;
                }
            }

            afterCompressed.Add(input);
            byte number = Convert.ToByte((validCount + 1) / 2 - 1);
            afterCompressed.Insert(0, Convert.ToByte(mark));
            afterCompressed.Insert(0, number);
            foreach (HUFFMANCODE code in huffcode)
            {
                codeLength = code.huffcodeString.Length;
                byte[] stringCode = System.Text.Encoding.Default.GetBytes(code.huffcodeString);
                for (int i = stringCode.Length - 1; i >= 0; i--)
                {
                    temp = stringCode[i];
                    afterCompressed.Insert(2, temp);
                }
                afterCompressed.Insert(2, code.stringLength);
                afterCompressed.Insert(2, code.content);
            }
            return afterCompressed.ToArray();
        }

        public void writeFile(string address, byte[] contents)
        {
            string name;
            name = address + ".huff";
            FileStream F = new FileStream(name, FileMode.Create);
            F.Write(contents, 0, contents.Length);
            F.Close();
        }

    }

}



