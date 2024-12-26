using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

class Program
{
    static void Main(string[] args)
    {
        Console.Write("Enter path to JSON file: ");
        string filePath = Console.ReadLine();
        filePath = filePath.Replace("\"", "");

        try
        {
            // Read file content
            string content = File.ReadAllText(filePath);

            // Find all IPs using regex
            var ipRegex = new Regex(@"(?:""(?:[0-9]{1,3}\.){3}[0-9]{1,3}"")", RegexOptions.Compiled);
            var matches = ipRegex.Matches(content);

            // Extract IPs and remove quotes
            var ips = matches
                .Select(m => m.Value.Trim('"'))
                .ToList();

            // Group by first two octets
            var groupedIps = ips
                .GroupBy(ip =>
                {
                    var parts = ip.Split('.');
                    return $"{parts[0]}.{parts[1]}";
                })
                .ToDictionary(g => g.Key, g => g.Take(3).ToList());

            // Prepare output
            var outputLines = groupedIps
                .Select(group => string.Join(",", group.Value))
                .ToList();

            // Replace newlines with commas in the entire output
            string outputContent = string.Join(",", outputLines);

            // Save to file
            string outputPath = Path.Combine(
                Path.GetDirectoryName(filePath),
                $"grouped_ips_{DateTime.Now:yyyyMMdd_HHmmss}.txt"
            );
            File.WriteAllText(outputPath, outputContent);

            Console.WriteLine($"Done! Results saved to: {outputPath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }
}