using System;
using System.Net.Http;
using System.Threading.Tasks;
using System.Text.Json;
using System.Collections.Generic;
using System.Text.Json.Serialization;
using ConsoleTables;

namespace OneListClient
{
    class Program
    {
        class Item
        {
            [JsonPropertyName("id")]
            public int Id { get; set; }
            [JsonPropertyName("text")]
            public string Text { get; set; }
            [JsonPropertyName("complete")]
            public bool Complete { get; set; }
            [JsonPropertyName("created_at")]
            public DateTime CreatedAt { get; set; }
            [JsonPropertyName("updated_at")]
            public DateTime UpdatedAt { get; set; }
            public string CompletedStatus
            {
                get
                {
                    // Uses a ternary to return "completed" if the 'complete variable. Returns "not completed" if false
                    return Complete ? "completed" : "not completed";
                }
            }
        }
        static async Task ShowAllItems(string token)
        {
            var client = new HttpClient();

            // Make a 'Get' request to the API and get back a stream of data.
            var url = $"https://one-list-api.herokuapp.com/items?access_token={token}";
            var responseAsStream = await client.GetStreamAsync(url);

            // Supply that stream of data to a Deserialize that will interpret it as a List of Item objects.
            var items = await JsonSerializer.DeserializeAsync<List<Item>>(responseAsStream);

            // add a table
            var table = new ConsoleTable("Description", "Created At", "Completed");

            // For each item in our deserialized List of items
            foreach (var item in items)
            {
                // Output some details on that item.
                table.AddRow(item.Text, item.CreatedAt, item.CompletedStatus);
            }

            // Write Table
            table.Write(Format.Minimal);
        }
        static async Task GetOneItem(string token, int id)
        {
            var client = new HttpClient();

            var url = $"https://one-list-api.herokuapp.com/items/{id}?access_token={token}";

            var responseAsStream = await client.GetStreamAsync(url);

            var item = await JsonSerializer.DeserializeAsync<Item>(responseAsStream);

            var table = new ConsoleTable("ID", "Description", "Created At", "Updated At", "Completed");

            table.AddRow(item.Id, item.Text, item.CreatedAt, item.UpdatedAt, item.CompletedStatus);

            table.Write(Format.Minimal);
        }
        static async Task Main(string[] args)
        {
            var token = "";

            if (args.Length == 0)
            {
                Console.Write("What list would you like? ");
                token = Console.ReadLine();
            }
            else
            {
                token = args[0];
            }

            var keepGoing = true;
            while (keepGoing)
            {
                Console.Clear();
                Console.Write("Get (A)ll todo, or Get (O)ne todo, or (Q)uit: ");
                var choice = Console.ReadLine().ToUpper();

                switch (choice)
                {
                    case "Q":
                        keepGoing = false;
                        break;
                    case "A":
                        await ShowAllItems(token);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    case "O":
                        Console.Write("Enter the ID of the item to show: ");
                        var id = int.Parse(Console.ReadLine());

                        await GetOneItem(token, id);

                        Console.WriteLine("Press ENTER to continue");
                        Console.ReadLine();
                        break;
                    default:
                        break;
                }
            }

        }
    }
}
