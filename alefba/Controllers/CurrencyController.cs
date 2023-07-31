using alefba.api.Model;
using alefba.api.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Bson;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace alefba.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class CurrencyController : ControllerBase
    {
        // GET api/currency
        [HttpGet, AllowAnonymous]
        public string GetAsync()
        {
            var startTimeSpan = TimeSpan.Zero;
            var periodTimeSpan = TimeSpan.FromSeconds(30);
            var timer = new System.Threading.Timer((e) =>
            {
                var result = GetData();
            }, null, startTimeSpan, periodTimeSpan);
            return "Repeat the data extraction every 30 seconds.";
        }

        // GET api/currency/getdata
        [HttpGet, AllowAnonymous, Route(nameof(GetData))]
        public async Task<ArrayList> GetData()
        {
            ArrayList currencyList = new ArrayList();
            var path = "https://mex.co.ir/v1/service/product/fetch/mainboard";
            using (HttpClient client = new HttpClient())
            {
                HttpRequestMessage requestMessage = new HttpRequestMessage(HttpMethod.Post, path);
                requestMessage.Content = JsonContent.Create(new { type = "C" });
                HttpResponseMessage response = client.SendAsync(requestMessage).Result;
                string apiResponse = response.Content.ReadAsStringAsync().Result;
                var resObject = JsonConvert.DeserializeObject<Currency>(apiResponse);

                if (response.IsSuccessStatusCode)
                {
                    Calendar calendar = new Calendar();
                    var date = resObject.info.products[1].LASTEDIT.Split()[0].Split('/');
                    var miladiDate = Convert.ToDateTime(calendar.Miladi(date[0], date[1], date[2]));
                    var document = new BsonDocument {
                 {"currency",resObject.info.products[1].SYMBOL}
                , {"date",resObject.info.products[1].LASTEDIT.Split()[0]}
                , {"rate",resObject.info.products[1].SELLPRICE}
                , {"time",resObject.info.products[1].LASTEDIT.Split()[1]}
                , {"miladiDate",miladiDate}
        };
                    addData(document);
                    Currency currency = new Currency
                    {
                        currency = resObject.info.products[1].SYMBOL,
                        rate = resObject.info.products[1].SELLPRICE,
                        time = resObject.info.products[1].LASTEDIT.Split()[1],
                        date = resObject.info.products[1].LASTEDIT.Split()[0],
                        miladiDate = miladiDate
                    };
                    currencyList.Add(currency);
                }
            }


            return currencyList;
        }

        public void addData(BsonDocument document)
        {
            MongoClient dbClient = new MongoClient("mongodb://localhost:27017/");
            var database = dbClient.GetDatabase("alefba");
            var collection = database.GetCollection<BsonDocument>("currency3");
            collection.InsertOne(document);
        }

        // POST api/currency/average
        [HttpPost, Route(nameof(Average))]
        public string Average(DateTime? fromDateTime = null, DateTime? toDateTime = null)

        {
            if (!fromDateTime.HasValue && !toDateTime.HasValue)
            {
                fromDateTime = DateTime.Now.AddDays(-15);
                toDateTime = DateTime.Now;
            }
            int average;
            var collection = new MongoClient().GetDatabase("alefba").GetCollection<CurrencyResult>("currency3");
            var secondDocument = collection.Find(new BsonDocument()).ToList().Where(item => item.miladiDate > fromDateTime && item.miladiDate < toDateTime);
            var rateDocument = secondDocument.Select(item => item.rate).ToList();
            int sum = rateDocument.Sum();
            if (sum > 0) { average = sum / rateDocument.Count(); }
            else
            {
                average = 0;
            }
            return "average is:" + average;
        }

    }
}