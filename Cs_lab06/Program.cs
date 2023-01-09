using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;


namespace Cs_lab06
{
    class Program
    {

        struct Weather
        {
            public string Country;
            public string Name;
            public double Temp;
            public string Description;
            public Weather(string _country, string _name, double _temp, string _desc)
            {
                Country = _country;
                Name = _name;
                Temp = _temp;
                Description = _desc;
            }
        }
        static async Task Main(string[] args)
        {
            var random = new System.Random();
            double lon = random.NextDouble() * 360 - 180;
            double lat = random.NextDouble() * 180 - 90;
            string api = "69d628518916cfeb2075778ed593e6c6";
            var client = new HttpClient();
            var content = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={api}");

            List<Weather> weathers = new List<Weather>(50);

            while (weathers.Count() < 50)
            {
                lon = random.NextDouble() * 360 - 180;
                lat = random.NextDouble() * 180 - 90;
                client = new HttpClient();
                content = await client.GetStringAsync($"https://api.openweathermap.org/data/2.5/weather?lat={lat}&lon={lon}&appid={api}");
                Console.WriteLine(content);

                Regex reCountry = new Regex("(?<=\"country\":\")[^\"]+(?=\")");
                Regex reName = new Regex("(?<=\"name\":\")[^\"]+(?=\")");
                Regex reTemp = new Regex("(?<=\"temp\":)[^:]+(?=,)");
                Regex reDesc = new Regex("(?<=\"description\":\")[^\"]+(?=\")");

                MatchCollection cMatches = reCountry.Matches(content);
                MatchCollection nMatches = reName.Matches(content);
                MatchCollection tMatches = reTemp.Matches(content);
                MatchCollection dMatches = reDesc.Matches(content);

                //WriteLine(tMatches[0]);
                if (cMatches.Count != 0)
                {
                    if(nMatches.Count != 0)
                    {
                        Weather tmpWeathear = new Weather(cMatches[0].Value, nMatches[0].Value, Convert.ToDouble(tMatches[0].Value, System.Globalization.CultureInfo.InvariantCulture) - 273, dMatches[0].Value);
                        weathers.Add(tmpWeathear);
                    }
                    else
                    {
                        continue;
                    }
                    
                }
                else
                {
                    continue;
                }
            }

            foreach(Weather obj in weathers)
            {
                Console.WriteLine($"Сountry: {obj.Country}  Name: {obj.Name} Temp: {obj.Temp}  Desc: {obj.Description}");
            }


            var tempCollection = (from i in weathers
                                 orderby i.Temp
                                 select i).ToList();


            Console.WriteLine("\n");
            Console.WriteLine($"Min temp in {tempCollection[0].Country} Temp: {tempCollection[0].Temp}");
            Console.WriteLine($"Max temp in {tempCollection[9].Country} Temp: {tempCollection[9].Temp}");

            var averTemp = (from i in weathers
                            let p = i.Temp
                            select p).Average();
            Console.WriteLine("\n");
            Console.WriteLine($"Средняя температура {averTemp}");

            var countryCount = (from i in weathers
                                group new
                                {
                                    i.Name,
                                    i.Temp,
                                    i.Description
                                }
                                by i.Country into weatherInCountr
                                orderby weatherInCountr.Key
                                select weatherInCountr).ToList();

            Console.WriteLine("\n");
            Console.WriteLine($"Количество стран {countryCount.Count}");


            





        }
    }
}

/*
 {"coord":{"lon":143.9794,"lat":63.1223},"weather":[{"id":801,"main":"Clouds","description":"few clouds","icon":"02n"}],"base":"stations","main":{"temp":253.03,"feels_like":253.03,"temp_min":253.03,"temp_max":253.03,"pressure":1010,"humidity":88,"sea_level":1010,"grnd_level":911},"visibility":10000,"wind":{"speed":1.14,"deg":170,"gust":1.59},"clouds":{"all":16},"dt":1666260180,"sys":{"country":"RU","sunrise":1666214629,"sunset":1666248828},"timezone":36000,"id":2122311,"name":"Oymyakon","cod":200}
*/