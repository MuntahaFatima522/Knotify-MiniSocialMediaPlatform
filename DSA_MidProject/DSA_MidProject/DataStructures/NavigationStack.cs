using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class NavigationStack
    {
        private NavigationNode top;
        private NavigationNode bottom;
        private int count;

        public int Count => count;
        public bool IsEmpty => top == null;

        public NavigationStack()
        {
            top = null;
            bottom = null;
            count = 0;
        }

        public void Push(Form form)
        {
            NavigationNode newNode = new NavigationNode(form);

            if (top == null)
            {
                top = newNode;
                bottom = newNode;
            }
            else
            {
                newNode.Next = top;
                top.Previous = newNode;
                top = newNode;
            }
            count++;
        }

        public Form Pop()
        {
            if (IsEmpty)
                return null;

            Form form = top.FormInstance;
            top = top.Next;

            if (top != null)
                top.Previous = null;
            else
                bottom = null;

            count--;
            return form;
        }

        public Form Peek()
        {
            if (IsEmpty)
                return null;

            return top.FormInstance;
        }

        public void Clear()
        {
            top = null;
            bottom = null;
            count = 0;
        }

        public string GetStackInfo()
        {
            if (IsEmpty)
                return "Stack is empty";

            StringBuilder sb = new StringBuilder();
            sb.AppendLine($"Stack Count: {count}");
            sb.AppendLine("Navigation History:");

            NavigationNode current = top;
            int position = 1;
            while (current != null)
            {
                sb.AppendLine($"{position}. {current.FormName}");
                current = current.Next;
                position++;
            }

            return sb.ToString();
        }
    }
}
