using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace B_wheeler
{
    internal class HuffmanTree
    {
        public PriorityQueue tree { get; set; }
        public List<byte> data { get; set; }
        public List<byte> C { get; set; }
        public List<int> Frequencies { get; set; }

        public HuffmanTree()
        {
            data = new List<byte>();
            C = new List<byte>();
            Frequencies = new List<int>();
            tree = new PriorityQueue();
        }

        private void ExtractDistinctCharacter()
        {
            HashSet<byte> list = new HashSet<byte>();
            foreach (var c in this.data)
            {
                list.Add(c);
            }
            this.C = list.ToList();
        }
        private void CalculateFrequencies()
        {
            Dictionary<byte, int> FrecChars = new Dictionary<byte, int>();
            foreach (var c in this.data)
            {
                if (FrecChars.ContainsKey(c))
                {
                    FrecChars[c]++;
                }
                else
                {
                    FrecChars.Add(c, 1);
                }
            }
            this.Frequencies.AddRange(FrecChars.Values);

        }

        public PriorityQueue ConstructTree()
        {
            ExtractDistinctCharacter();
            CalculateFrequencies();

            for (int i = 0; i < this.C.Count; i++)
            {

                Node z = new Node();
                z.frec = this.Frequencies[i];
                z.number = this.C[i];
                z.left = null;
                z.right = null;
                this.tree.Enqueue(z);
            }

            for (int i = 0; i < this.C.Count - 1; i++)
            {

                Node z = new Node();
                var x = this.tree.Min();
                this.tree.Dequeue();
                var y = this.tree.Min();
                this.tree.Dequeue();
                z.left = x;
                z.right = y;
                z.number = null;
                z.frec = x.frec + y.frec;
                this.tree.Enqueue(z);
            }

            return this.tree;
        }
    }
}
