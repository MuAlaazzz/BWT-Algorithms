using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace B_wheeler
{
    internal class PriorityQueue
    {
        public List<Node> data;

        // Implemeting Priority Queue using inbuilt available List in C#
        public PriorityQueue()
        {
            this.data = new List<Node>();
        }

        private int GetParent(int index)
        {
            if (index <= 0)
                return 0;

            return (index - 1) / 2;
        }

        private int GetLeft(int index)
        {
            return 2 * index + 1;
        }
        private int GetRight(int index)
        {
            return 2 * index + 2;
        }

        public void Enqueue(Node item)
        {
            data.Add(item);
            ShiftUp(data.Count - 1);
        }

        public Node Dequeue()
        {
            if (data.Count > 0)
            {
                Node item = data[0];
                data[0] = data[data.Count - 1];
                data.RemoveAt(data.Count - 1);
                ShiftDown(0);
                return item;
            }
            throw new InvalidOperationException("The heap is empty");
        }

        private void ShiftDown(int index)
        {
            var smallest = index;
            var left = GetLeft(index);
            var right = GetRight(index);

            if (left < data.Count && data[left].frec < data[index].frec)
            {
                smallest = left;
            }
            if (right < data.Count && data[right].frec < data[smallest].frec)
            {
                smallest = right;
            }
            if (smallest != index)
            {
                var tmp = data[index];
                data[index] = data[smallest];
                data[smallest] = tmp;
                ShiftDown(smallest);
            }
        }
        private void ShiftUp(int index)
        {
            var parent = GetParent(index);
            if (parent >= 0 && data[index].frec < data[parent].frec)
            {
                var tmp = data[index];
                data[index] = data[parent];
                data[parent] = tmp;
                ShiftUp(parent);
            }
        }

        // function which returns peek element
        public Node Min()
        {
            if (data.Count == 0)
                return null;
            Node frontItem = data[0];
            //Dequeue();
            return frontItem;
        }
        public Node Peek()
        {
            if (data.Count == 0)
                return null;
            Node minElemant = data[(data.Count - 1)];
            return minElemant;
        }

        public void DispalyAllNodes()
        {
            if (data.Count == 0)
                Console.WriteLine("No data");
            foreach (var item in data)
            {
                Console.WriteLine(item.frec);
            }
        }
    }
}
