using System.Globalization;
using System.Text;
using CsvHelper;
using CsvHelper.Configuration;
using GroundTemperatures.Models;
using jaytwo.SolutionResolution;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

#pragma warning disable CS8604 // Possible null reference argument.
#pragma warning disable CS8602 // Dereference of a possibly null reference.
namespace GroundTemperatures
{
    public class Program
    {
        private static SlnFileResolver SlnFileResolver { get; } = new SlnFileResolver();

        public static async Task Main(string[] args)
        {
            // data from: https://www.greencastonline.com/tools/soil-temperature

            var scraper = new GroundTemperatureScraper("a2f0d7a4", "742a069efe55c7015c2245032fb16bbb");
            var location = "40.7607793,-111.8910474"; // SLC
            var startYear = 2000;
            var totalYears = 20;
            var values = await GetGroundTemperatures(scraper, location, startYear, totalYears);

            var averages = values.Single().Value
                .Select(x => new
                {
                    date = DateOnly.Parse(x.Key),
                    avg_temp_f = x.Value.soil_temp_0to10cm.value,
                    min_temp_f = x.Value.soil_temp_min_0to10cm.value,
                    max_temp_f = x.Value.soil_temp_max_0to10cm.value,
                })
                .GroupBy(x => new { x.date.Month, x.date.Day })
                .OrderBy(x => x.Key.Month).ThenBy(x => x.Key.Day)
                .Select(x => new
                {
                    month = x.Key.Month,
                    day = x.Key.Day,
                    avg_temp_f = x.Average(y => y.avg_temp_f),
                    min_temp_f = x.Min(y => y.min_temp_f),
                    max_temp_f = x.Max(y => y.max_temp_f),
                    stdev = x.StdDev(y => y.avg_temp_f),
                });

            var outputFileName = $"soil-temps-10cm-{startYear}-{startYear + totalYears - 1}.csv";
            var csvPath = SlnFileResolver.ResolvePathRelativeToSln(outputFileName);

            Console.WriteLine($"Writing to CSV: {csvPath}");
            var rowsWritten = await WriteToCsvAsync(averages, csvPath);

            Console.WriteLine("Done!  ({0}: rows)", rowsWritten);
        }

        private static async Task<int> WriteToCsvAsync<T>(IEnumerable<T> data, string fileName)
            => await WriteToCsvAsync(data.ToAsyncEnumerable(), fileName);

        private static async Task<int> WriteToCsvAsync<T>(IAsyncEnumerable<T> data, string fileName)
        {
            var encoding = Encoding.UTF8;

            var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture)
            {
                Encoding = encoding,
                TrimOptions = TrimOptions.Trim,
                NewLine = "\r\n",
                Quote = '\"',
                Escape = '\"',
            };

            int rowsWritten = 0;
            using (var fileStream = File.Create(fileName))
            using (var textWriter = new StreamWriter(fileStream, encoding))
            using (var csvWriter = new CsvWriter(textWriter, csvConfiguration))
            {
                csvWriter.WriteHeader<T>();
                await csvWriter.NextRecordAsync();

                await foreach (var datum in data)
                {
                    //Console.WriteLine(datum);
                    csvWriter.WriteRecord(datum);
                    await csvWriter.NextRecordAsync();
                    rowsWritten++;
                }
            }

            return rowsWritten;
        }

        private static async Task<FooResult> GetGroundTemperatures(GroundTemperatureScraper scraper, string location, int startYear, int totalYears)
        {
            var years = Enumerable.Range(startYear, totalYears).ToArray();
            var tasks = years.Select(x => Task.Run(async () =>
            {
                Console.WriteLine($"Querying year {x}");
                var result = await scraper.GetGroundTemperatures(location, x);
                Console.WriteLine($"Finished querying year {x}");

                return result;
            }));
            var allResults = await Task.WhenAll(tasks);

            var result = new FooResult();

            foreach (var fromApi in allResults)
            {
                foreach (var byLocation in fromApi)
                {
                    if (!result.ContainsKey(byLocation.Key))
                    {
                        result[byLocation.Key] = new Dictionary<string, DailyMeasurement>();
                    }

                    foreach (var byDay in byLocation.Value)
                    {
                        result[byLocation.Key][byDay.Key] = byDay.Value;
                    }
                }
            }

            return result;
        }
    }
}
#pragma warning restore CS8602 // Dereference of a possibly null reference.
#pragma warning restore CS8604 // Possible null reference argument.
