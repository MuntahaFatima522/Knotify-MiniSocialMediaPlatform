using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class StackNode<T>
    {
        public T Data { get; set; }
        public StackNode<T> Next { get; set; }

        public StackNode(T data)
        {
            Data = data;
            Next = null;
        }
    }
}
