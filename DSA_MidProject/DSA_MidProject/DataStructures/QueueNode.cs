using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class QueueNode<T>
    {
      
            public T Data { get; set; }
            public QueueNode<T> Next { get; set; }

            public QueueNode(T data)
            {
                Data = data;
                Next = null;
            }
        }

       
        }
    
