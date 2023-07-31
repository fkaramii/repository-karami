using MongoDB.Bson;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace alefba.api.Model
{
    public class Currency
    {
        public Guid id { get; set; }
        public string currency { get; set; }
        public int rate { get; set; }
        public string date { get; set; }
        public DateTime miladiDate { get; set; }
        public string time { get; set; }
        public Info info { get; set; }
    }
    public class CurrencyResult
    {
        public ObjectId _id { get; set; }
        public string currency { get; set; }
        public int rate { get; set; }
        public string date { get; set; }
        public string time { get; set; }
        public DateTime miladiDate { get; set; }
    }
}
