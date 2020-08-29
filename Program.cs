using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;

namespace OneListClient
{
    class Program
    {
        class Item
        {
            public int id { get; set; }
            public string text { get; set; }
            public bool complete { get; set; }
            public DateTime created_at { get; set; }
            public DateTime updated_at { get; set; }
            public string CompletedStatus
            {
                get
                {
                    // Uses a ternary to return "completed" if the 'complete variable. Returns "not completed" if false
                    return complete ? "completed" : "not completed";
                }
            }
        }
        static async Task Main(string[] args)
        {
            var client = new HttpClient();

            // Make a 'Get' request to the API and get back a stream of data.
            var responseAsStream = await client.GetStreamAsync("https://one-list-api.herokuapp.com/items?access_token=josh");

            // Supply that stream of data to a Deserialize that will interpret it as a List of Item objects.
            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);

            // For each item in our deserialized List of items
            foreach (var item in items)
            {
                // Output some details on that item.
                Console.WriteLine($"The task {item.text} was created on {item.created_at} and is {item.CompletedStatus}");
            }
        }
    }
}
