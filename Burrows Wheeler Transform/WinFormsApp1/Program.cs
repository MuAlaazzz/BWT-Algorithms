using B_wheeler;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace B_wheeler
{
    internal class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Form1());
        }

        Dictionary<byte, bool[]> traveseTree(HuffmanTree t)
        {
            var table = new Dictionary<byte, bool[]>();

            void Traverse(Node node, List<bool> code)
            {
                if (node == null)
                    return;

                if (node.left == null && node.right == null)
                {
                    if (node.number.HasValue)
                    {
                        table.Add(node.number.Value, code.ToArray());
                        table[node.number.Value] = code.ToArray();
                    }
                    return;
                }

                code.Add(false);
                Traverse(node.left, code);
                code.RemoveAt(code.Count - 1);

                code.Add(true);
                Traverse(node.right, code);
                code.RemoveAt(code.Count - 1);
            }

            Traverse(t.tree.Min(), new List<bool>());

            return table;
        }

        List<bool> Compress(List<byte> text, Dictionary<byte, bool[]> huffmanTable)
        {
            List<bool> compressed = new List<bool>();

            for (int i = 0; i < text.Count; i++)
            {
                if (huffmanTable.ContainsKey(text[i]))
                {
                    foreach (var item in huffmanTable[text[i]])
                    {
                        compressed.Add(item);
                    }
                }
            }

            return compressed;
        }

        List<byte?> Decompress(HuffmanTree tree, List<bool> compressed)
        {
            List<byte?> numbers = new List<byte?>();
            var size = 0;
            Node node = new Node();
            while (true)
            {

                if (compressed[size] == false) { node = tree.tree.Min().left; }
                if (compressed[size] == true) { node = tree.tree.Min().right; }

                while (node.left != null && node.right != null)
                {
                    size++;
                    if (compressed[size] == false) { node = node.left; }
                    if (compressed[size] == true) { node = node.right; }
                }
                if (node.number.HasValue)
                { numbers.Add(node.number); }

                if (size == compressed.Count - 1)
                    break;

                size++;
            }
            return numbers;
        }

        List<byte> ConvertBoolToByte(List<bool> CompressedText)
        {
            List<byte> result = new List<byte>();

            int length = CompressedText.Count;
            int index = 0;

            while (index < length)
            {
                byte res = 0;
                int bitsToProcess = Math.Min(8, length - index);
                for (int i = 0; i < bitsToProcess; i++)
                {
                    if (CompressedText[index + i])
                    {
                        res |= (byte)(1 << (7 - i));
                    }
                }
                result.Add(res);
                index += 8;
            }

            return result;
        }

        List<bool> GetBoolArrayFromByte(byte b)
        {
            List<bool> boolList = new List<bool>(8);

            for (int i = 0; i < 8; i++)
            {
                boolList.Add((b & (1 << (7 - i))) != 0);
            }

            return boolList;
        }

        List<bool> GetBoolListFromBytes(List<byte> bytes, int sizeOfOriginal)
        {
            List<bool> boolList = new List<bool>();

            foreach (byte b in bytes)
            {
                List<bool> bits = GetBoolArrayFromByte(b);
                boolList.AddRange(bits);
            }
            boolList.RemoveRange((boolList.Count - (boolList.Count - sizeOfOriginal)), (boolList.Count - sizeOfOriginal));
            return boolList;
        }

        string Serialize(Node root)
        {
            if (root == null)
            {
                return null;
            }
            Stack<Node> s = new Stack<Node>();
            s.Push(root);

            List<string> l = new List<string>();
            while (s.Count > 0)
            {
                Node t = s.Pop();

                if (t == null)
                {
                    l.Add("#");
                }
                else
                {
                    if (t.number == null) { l.Add(""); }
                    else { l.Add(t.number.ToString()); }
                    l.Add(t.frec.ToString());
                    s.Push(t.right);
                    s.Push(t.left);
                }
            }
            return string.Join(",", l);
        }

        Node GetHuffmanTree(string data)
        {
            int t;
            Node node = Deserialize(data);
            Node Deserialize(string data1)
            {
                if (data1 == null)
                    return null;
                t = 0;
                string[] arr = data1.Split(',');
                return Helper(arr);
            }

            Node Helper(string[] arr)
            {
                if (arr[t].Equals("#"))
                    return null;


                Node root = new Node();
                if (arr[t] == "")
                { root.number = null; }
                else { root.number = byte.Parse(arr[t]); }
                t++;
                root.frec = int.Parse(arr[t]);
                t++;
                root.left = Helper(arr);
                t++;
                root.right = Helper(arr);
                return root;
            }
            return node;
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Compression(string inputEnc, RichTextBox outputBox)
        {          
            int time1 = Environment.TickCount;
            int numSplits = inputEnc.Length - (int)(0.9993 * inputEnc.Length);
            string[] splitInput = SplitString(inputEnc, numSplits);
            string[] lastCols = new string[numSplits];
            int[] indices = new int[numSplits];
            for (int i = 0; i < splitInput.Length; i++)
            {
                string lastCol = BWT_Encryption(splitInput[i], out int originalIndex);
                lastCols[i] = lastCol;
                indices[i] = originalIndex;
            }
            string MoveToFront_Input = string.Join("", lastCols);
            int time2 = Environment.TickCount;
            outputBox.AppendText("finish time for BWT encoding is " + (time2 - time1) + "ms\n");
            int time3 = Environment.TickCount;
            List<byte> encodedOutput = Encode(MoveToFront_Input);
            int time4 = Environment.TickCount;
            outputBox.AppendText("finish time for  Move-To-Front encoding is " + (time4 - time3) + "ms\n");
            int time5 = Environment.TickCount;
            HuffmanTree Alaa = new HuffmanTree();
            Alaa.data = encodedOutput;
            Alaa.ConstructTree();
            Dictionary<byte, bool[]> AlaaTable = new Dictionary<byte, bool[]>();
            AlaaTable = traveseTree(Alaa);
            List<bool> compreesedText = Compress(encodedOutput, AlaaTable);
            string huffmanTree = Serialize(Alaa.tree.data[0]);
            List<byte> byteArray = ConvertBoolToByte(compreesedText);
            StoreInBinaryFile(huffmanTree, byteArray, compreesedText.Count, indices);
            int time6 = Environment.TickCount;
            outputBox.AppendText("finish time for Hoffman encoding is " + (time6 - time5) + "ms\n");
            outputBox.AppendText("finish time for all compression process is " + (time6 - time1) + "ms\n");
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        public void Decompression(string path, RichTextBox outputBox)
        {
            int time7 = Environment.TickCount;         
            string huffmanTree;
            int byteArrayLength;
            byte[] byteArray;
            int compressedArrayLength;
            int indicesLength;
            int[] indices;
            ReadBinaryFile(out huffmanTree, out byteArrayLength, out byteArray, out compressedArrayLength, out indicesLength, out indices , path);
            Node HuffmanTree = GetHuffmanTree(huffmanTree);
            List<bool> compreesedText = GetBoolListFromBytes(byteArray.ToList(), compressedArrayLength);
            HuffmanTree tree = new HuffmanTree();
            tree.tree.data.Add(HuffmanTree);
            List<byte?> decompressed = Decompress(tree, compreesedText);
            int time8 = Environment.TickCount;
            outputBox.AppendText("finish time for Hoffman decoding is " + (time8 - time7) + "ms\n");
            // Append message with newline
            int time9 = Environment.TickCount;
            string decodedoutput = Decode(decompressed);
            string[] splitDecoded = SplitString(decodedoutput, indices.Length);
            int time10 = Environment.TickCount;
            outputBox.AppendText("finish time for Move-To-Front decoding is " + (time10 - time9) + "ms\n");
            int time11 = Environment.TickCount;
            StringBuilder[] Dec_Text = new StringBuilder[indices.Length];
            for (int i = 0; i < splitDecoded.Length; i++)
            {
                Dec_Text[i] = BW_Decryption(splitDecoded[i], indices[i]);
            }
            StringBuilder finalDecrypted = new StringBuilder();
            for (int i = 0; i < Dec_Text.Length; i++)
            {
                finalDecrypted.Append(Dec_Text[i]);
            }
            int time12 = Environment.TickCount;
            outputBox.AppendText("finish time for BWT decoding is " + (time12 - time11) + "ms\n");
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = Path.Combine(documentsPath, $"DecodedData_{timestamp}.txt");

            using (StreamWriter streamWriter = new StreamWriter(fileName))
            {
                streamWriter.Write(finalDecrypted.ToString());
            }
            outputBox.AppendText("finish time for all decompression process is " + (time12 - time7) + "ms\n");
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static string[] SplitString(string input, int numSplits)
        {
            if (numSplits <= 0 || input.Length < numSplits)
            {
                throw new ArgumentException("Invalid number of splits or input length");
            }
            int chunkSize = input.Length / numSplits;

            int remainder = input.Length % numSplits;

            string[] splitStrings = new string[numSplits];

            int currentStartIndex = 0;

            for (int i = 0; i < numSplits; i++)
            {
                int currentChunkSize = chunkSize;

                if (i < remainder)
                {
                    currentChunkSize++;
                }
                splitStrings[i] = input.Substring(currentStartIndex, currentChunkSize);

                currentStartIndex += currentChunkSize;
            }
            return splitStrings;
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        static string BWT_Encryption(string inputEnc, out int originalIndex)
        {
            originalIndex = 0;
            string[] Suffixes = new string[inputEnc.Length];
            char[] lastCol = new char[inputEnc.Length];
            Suffixes[0] = inputEnc;
            for (int i = 1; i < Suffixes.Length; i++)
            {
                char firstChar = Suffixes[i - 1][0];
                string rest = Suffixes[i - 1].Substring(1);
                Suffixes[i] = rest + firstChar;
            }
            Array.Sort(Suffixes, StringComparer.Ordinal);
            for (int i = 0; i < Suffixes.Length; i++)
            {
                if (Suffixes[i].Equals(inputEnc))
                    originalIndex = i;
                lastCol[i] = (Suffixes[i][Suffixes.Length - 1]);
            }
            return new string(lastCol);
        }
        static StringBuilder BW_Decryption(string lastColumn, int originalIndex)
        {
            int lastColumn_Size = lastColumn.Length;
            Dictionary<char, Queue<int>> adjacencyList = new Dictionary<char, Queue<int>>();
            for (int i = 0; i < lastColumn_Size; i++)
            {
                char currentChar = lastColumn[i];
                if (!(adjacencyList.ContainsKey(currentChar)))
                    adjacencyList[currentChar] = new Queue<int>();
                adjacencyList[currentChar].Enqueue(i);
            }
            char[] sortedLastCol = lastColumn.ToCharArray();
            RadixSort(sortedLastCol);
            int[] nextArr = new int[lastColumn_Size];
            for (int i = 0; i < lastColumn_Size; i++)
            {
                char currentChar = sortedLastCol[i];
                nextArr[i] = adjacencyList[currentChar].Dequeue();
            }
            StringBuilder originalText = new StringBuilder(lastColumn_Size);
            for (int i = 0; i < lastColumn_Size; i++)
            {
                originalIndex = nextArr[originalIndex];
                originalText.Append(lastColumn[originalIndex]);
            }
            return originalText;
        }
        static void RadixSort(char[] input)
        {
            int[] apperences = new int[256];
            char[] output = new char[input.Length];
            foreach (char c in input)
                apperences[c]++;
            for (int i = 1; i < 256; i++)
                apperences[i] += apperences[i - 1];

            for (int i = input.Length - 1; i >= 0; i--)
            {
                output[apperences[input[i]] - 1] = input[i];
                apperences[input[i]]--;
            }
            for (int i = 0; i < input.Length; i++)
                input[i] = output[i];
        }
//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        const int A_size = 256;
        static List<byte> Encode(string input)
        {

            List<char> ordered_Ascii_list = new List<char>(A_size);

            for (int i = 0; i < A_size; i++)
            {
                ordered_Ascii_list.Add((char)i);
            }

            List<byte> output = new List<byte>();

            foreach (char c in input)
            {
                byte index = (byte)ordered_Ascii_list.IndexOf(c);
                output.Add(index);
                for (int i = index; i >= 1; i--)
                {
                    ordered_Ascii_list[i] = ordered_Ascii_list[i - 1];
                }
                ordered_Ascii_list[0] = c;
            }
            return output;
        }

        static string Decode(List<byte?> encodedInput)
        {
            List<char> ordered_Ascii_list = new List<char>(A_size);

            for (int i = 0; i < A_size; i++)
            {
                ordered_Ascii_list.Add((char)i);
            }

            char[] output = new char[encodedInput.Count];
            int Index = 0;

            foreach (int i in encodedInput)
            {

                char z = ordered_Ascii_list[i];
                output[Index] = z;
                Index++;

                for (int j = i; j >= 1; j--)
                {
                    ordered_Ascii_list[j] = ordered_Ascii_list[j - 1];
                }
                ordered_Ascii_list[0] = z;
            }
            return new string(output);
        }

//////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        void StoreInBinaryFile(string huffmanTree, List<byte> ByteArray, int CompressedArrayLength, int[] index)
        {       
            string documentsPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            string timestamp = DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss");
            string fileName = $"data_{timestamp}.bin";
            string fullPath = Path.Combine(documentsPath, fileName);
            using (FileStream fs = new FileStream(fullPath, FileMode.Create))
            {
                using (BinaryWriter writer = new BinaryWriter(fs))
                {
                    writer.Write(huffmanTree.Length);
                    writer.Write(Encoding.ASCII.GetBytes(huffmanTree));
                    writer.Write(ByteArray.Count);
                    writer.Write(ByteArray.ToArray());
                    writer.Write(CompressedArrayLength);
                    writer.Write(index.Length);
                    foreach (int i in index)
                    {
                        writer.Write(i);
                    }
                }
            }
        }
        void ReadBinaryFile(out string huffmanTree, out int byteArrayLength, out byte[] ByteArray, out int CompressedArrayLength, out int indexLength, out int[] index , string path)
        {
            using (FileStream fs = new FileStream(path, FileMode.Open))
            {
                using (BinaryReader reader = new BinaryReader(fs))
                {
                    int huffmanLength = reader.ReadInt32();
                    huffmanTree = Encoding.ASCII.GetString(reader.ReadBytes(huffmanLength));

                    byteArrayLength = reader.ReadInt32();
                    ByteArray = reader.ReadBytes(byteArrayLength);

                    CompressedArrayLength = reader.ReadInt32();

                    indexLength = reader.ReadInt32();

                    index = new int[indexLength];
                    for (int i = 0; i < indexLength; i++)
                    {
                        index[i] = reader.ReadInt32();
                    }
                }
            }
        }
    }
}
