using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alefba.api.Model
{
    public class Products
    {
        public int ID { get; set; }
        public string LASTEDIT { get; set; }
        public int SELLPRICE { get; set; }
        public string BUYPRICE { get; set; }
        public string SYMBOL { get; set; }
        public Info info { get; set; }
    }
}
