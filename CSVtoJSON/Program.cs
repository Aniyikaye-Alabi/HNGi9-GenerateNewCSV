// See https://aka.ms/new-console-template for more information
using ChoETL;
using Newtonsoft.Json;
using System.Security.Cryptography;
using System.Text;

Console.WriteLine("Enter csv file name in the format name.csv");
Console.Write("Use 'NFTAll.csv' for this demo: ");
string csvFile = Console.ReadLine();

Console.Write("Name of your JSON File in format name.json: ");
string jsonFile = Console.ReadLine();

using (var r = new ChoCSVReader<NFT>(csvFile)
        .WithFirstLineHeader()
        .MayHaveQuotedFields()
        )
{

        
    using (var w = new ChoJSONWriter<NFT>(jsonFile)
            .UseJsonSerialization()
            )
        w.Write(r);
}

{
    List<HashNFT> list = new List<HashNFT>();
    using (StreamReader r = new StreamReader(jsonFile))
    {
        string json = r.ReadToEnd();
        List<NFT> oldNft = JsonConvert.DeserializeObject<List<NFT>>(json);

        foreach (var item in oldNft)
        {
            list.Add(new HashNFT
            {
                SerialNumber = item.SerialNumber,
                Filename = item.Filename,
                UUID = item.UUID,
                Hash = ComputeSha256Hash(item.Filename)
            });
        }

        Console.Out.Write(JsonConvert.SerializeObject(list, Formatting.Indented));

        using (var csv = new ChoCSVWriter("HashNFT.output.csv").WithFirstLineHeader())
        {
            {
                csv.Write(list);
                Console.WriteLine();
                Console.WriteLine("***HashNFT.output.csv file is generated in path from root CSVtoJSON/bin/Debug/.net6 directory***");
            }
        }
    }
}


static string ComputeSha256Hash(string rawData)
{
    // Create a SHA256   
    using (SHA256 sha256Hash = SHA256.Create())
    {
        // ComputeHash - returns byte array  
        byte[] bytes = sha256Hash.ComputeHash(Encoding.UTF8.GetBytes(rawData));

        // Convert byte array to a string   
        StringBuilder builder = new StringBuilder();
        for (int i = 0; i < bytes.Length; i++)
        {
            builder.Append(bytes[i].ToString("x2"));
        }
        return builder.ToString();
    }
}


// Old CSV Model Class
public class NFT
{
    public string SerialNumber { get; set; }
    public string Filename { get; set; }
    public string UUID { get; set; }
}


// New CSV Model Class
public class HashNFT
{
    public string SerialNumber { get; set; }
    public string Filename { get; set; }
    public string UUID { get; set; }
    public string Hash { get; set; }
}