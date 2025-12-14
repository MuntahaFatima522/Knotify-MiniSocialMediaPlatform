using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DSA_MidProject.DataStructures
{
    internal class NavigationNode
    {
        public string FormName { get; set; }
        public Form FormInstance { get; set; }
        public NavigationNode Next { get; set; }
        public NavigationNode Previous { get; set; }

        public NavigationNode(Form form)
        {
            FormInstance = form;
            FormName = form.GetType().Name;
            Next = null;
            Previous = null;
        }
    }
}
