using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class Queue<T>
    {
        private QueueNode<T> front;
        private QueueNode<T> rear;
        private int count;

        public int Count => count;
        public bool IsEmpty => front == null;

        public void Enqueue(T data)
        {
            QueueNode<T> newNode = new QueueNode<T>(data);

            if (rear == null)
            {
                front = rear = newNode;
            }
            else
            {
                rear.Next = newNode;
                rear = newNode;
            }
            count++;
        }

        public T Dequeue()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty");

            T data = front.Data;
            front = front.Next;

            if (front == null)
                rear = null;

            count--;
            return data;
        }

        public T Peek()
        {
            if (IsEmpty)
                throw new InvalidOperationException("Queue is empty");

            return front.Data;
        }

        public void Clear()
        {
            front = rear = null;
            count = 0;
        }


        public List<T> ToList()
        {
            List<T> list = new List<T>();
            QueueNode<T> current = front;
            while (current != null)
            {
                list.Add(current.Data);
                current = current.Next;
            }
            return list;
        }
    } 
    }
