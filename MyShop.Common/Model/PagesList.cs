using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.Common
{
    public class PagesList<T> : List<T> where T : class
    {
        public int totalCount { get; set; }
        public List<T> pageList { get; set; }

    }
}
