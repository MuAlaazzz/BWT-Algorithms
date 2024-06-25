using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace B_wheeler
{
    internal class Node
    {
        public int frec { get; set; }
        public byte? number { get; set; }
        public Node left { get; set; }
        public Node right { get; set; }
    }
}
