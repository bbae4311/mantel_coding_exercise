using System.Text.RegularExpressions;

if (args.Length == 0)
{
    Console.WriteLine("Please provide a log file name as an argument.");
    return;
}

string fileName = args[0];

if (File.Exists(fileName))
{
    Console.WriteLine($"File found: {fileName}");
    // do something with the file
}
else
{
    Console.WriteLine($"File not found: {fileName}");
    return;
}

// Regex handles:
// 177.71.128.21 - - [timestamp] "GET /url HTTP/1.1"
// 50.112.00.11 - admin [timestamp] "GET /url HTTP/1.1"
var logPattern = new Regex(
    @"^(?<ip>\S+)\s+\S+\s+\S+\s+\[(?<timestamp>[^\]]+)\]\s+""(?<method>\S+)\s+(?<url>\S+)",
    RegexOptions.Compiled);

var uniqueIps = new HashSet<string>();
var ipCount = new Dictionary<string, int>();
var urlCount = new Dictionary<string, int>();

foreach (var line in File.ReadLines(fileName))
{
    var match = logPattern.Match(line);
    if (!match.Success)
        continue;

    var ip = match.Groups["ip"].Value;
    var url = match.Groups["url"].Value;

    uniqueIps.Add(ip);

    if (!ipCount.ContainsKey(ip))
        ipCount[ip] = 0;
    ipCount[ip]++;

    if (!urlCount.ContainsKey(url))
        urlCount[url] = 0;
    urlCount[url]++;
}

Console.WriteLine("=== Log Analysis Results ===");

// 1. Unique IPs
Console.WriteLine($"Unique IP addresses: {uniqueIps.Count}");

// 2. Top 3 URLs
Console.WriteLine("\nTop 3 most visited URLs:");
foreach (var item in urlCount.OrderByDescending(x => x.Value).Take(3))
    Console.WriteLine($"{item.Key} — {item.Value} visits");

// 3. Top 3 active IPs
Console.WriteLine("\nTop 3 most active IP addresses:");
foreach (var item in ipCount.OrderByDescending(x => x.Value).Take(3))
    Console.WriteLine($"{item.Key} — {item.Value} requests");

