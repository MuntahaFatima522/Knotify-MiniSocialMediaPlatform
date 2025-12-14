using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class Stack<T>
    {
        private StackNode<T> top;
        private int count;

        public int Count => count;
        public bool IsEmpty => top == null;

        public void Push(T data)
        {
            StackNode<T> newNode = new StackNode<T>(data);
            newNode.Next = top;
            top = newNode;
            count++;
        }

        public T Pop()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty");

            T data = top.Data;
            top = top.Next;
            count--;
            return data;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Stack is empty");

            return top.Data;
        }

        public void Clear()
        {
            top = null;
            count = 0;
        }

    }
}
