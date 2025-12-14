using System;
using System.Collections.Generic;

namespace DSA_MidProject.DataStructures
{
    internal class CustomList<T>
    {
        public Node<T> Head { get; set; }
        public int Count { get; set; }

        public CustomList()
        {
            Head = null;
            Count = 0;
        }

        public void Add(T data)
        {
            Node<T> newNode = new Node<T>(data);

            if (Head == null)
            {
                Head = newNode;
            }
            else
            {
                Node<T> current = Head;
                while (current.Next != null)
                {
                    current = current.Next;
                }
                current.Next = newNode;
            }
            Count++;
        }

        public List<T> FindAll(Func<T, bool> condition)
        {
            List<T> result = new List<T>();
            Node<T> current = Head;

            while (current != null)
            {
                if (condition(current.Data))
                {
                    result.Add(current.Data);
                }
                current = current.Next;
            }
            return result;
        }

        public int CountWhere(Func<T, bool> condition)
        {
            int count = 0;
            Node<T> current = Head;

            while (current != null)
            {
                if (condition(current.Data))
                {
                    count++;
                }
                current = current.Next;
            }
            return count;
        }

        public List<T> ToList()
        {
            List<T> result = new List<T>();
            Node<T> current = Head;

            while (current != null)
            {
                result.Add(current.Data);
                current = current.Next;
            }
            return result;
        }
    }
}