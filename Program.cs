using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using ExcelDataReader;
using System.Diagnostics;

class Program
{
    static void Main()
    {
        Console.WriteLine("==============================================");
        Console.WriteLine("     🚀 Welcome to Deque Assistant Tool 🚀     ");
        Console.WriteLine("==============================================");
        Console.WriteLine();
        Console.WriteLine("This tool automates the process of downloading accessibility reports, ");
        Console.WriteLine("converting them to CSV, and consolidating data for analysis.");
        Console.WriteLine();

        Console.Write("   Would you like to proceed with downloading JSON reports? (y/n): ");
        string userInput = Console.ReadLine()?.Trim().ToLower();
        if (userInput != "y")
        {
            Console.WriteLine("   Operation canceled by user. Exiting...");
            return;
        }

        Console.Write("\n   Please enter the full path of the Excel file containing the URLs: ");
        string excelFilePath = Console.ReadLine()?.Trim();
        if (string.IsNullOrEmpty(excelFilePath) || !File.Exists(excelFilePath))
        {
            Console.WriteLine("   Invalid file path. Please restart the program and provide a valid path.");
            return;
        }

        string selectedDirectory = Path.GetDirectoryName(excelFilePath);
        string downloadDir = Path.Combine(selectedDirectory, "JSON_Reports");
        string csvDir = Path.Combine(selectedDirectory, "CSV_Reports");

        if (Directory.Exists(downloadDir))
        {
            Console.WriteLine("\n   The 'JSON_Reports' folder already exists. Choose an option:");
            Console.WriteLine("   1️ - Overwrite the existing folder");
            Console.WriteLine("   2️ - Create a new folder with a timestamp");
            Console.WriteLine("   3️ - Cancel the operation");
            Console.Write("👉 Enter your choice (1/2/3): ");
            string option = Console.ReadLine()?.Trim();

            if (option == "1")
            {
                Directory.Delete(downloadDir, true);
                Directory.CreateDirectory(downloadDir);
            }
            else if (option == "2")
            {
                string timestamp = DateTime.Now.ToString("yyyyMMdd_HHmmss");
                downloadDir = Path.Combine(selectedDirectory, $"JSON_Reports_{timestamp}");
                Directory.CreateDirectory(downloadDir);
            }
            else
            {
                Console.WriteLine("   Operation canceled by user. Exiting...");
                return;
            }
        }
        else
        {
            Directory.CreateDirectory(downloadDir);
        }

        var urls = ReadUrlsFromExcel(excelFilePath);
        if (urls.Length == 0)
        {
            Console.WriteLine("   No URLs found in the provided Excel file. Please check the file and try again.");
            return;
        }

        Console.WriteLine("\n🚀 Initiating browser automation for downloading reports...");
        ChromeOptions options = new ChromeOptions();
        options.AddUserProfilePreference("download.default_directory", downloadDir);
        options.AddUserProfilePreference("download.prompt_for_download", false);
        options.AddUserProfilePreference("download.directory_upgrade", true);
        options.AddUserProfilePreference("safebrowsing.enabled", true);
        options.AddArgument("--headless");
        options.AddArgument("--no-sandbox");
        options.AddArgument("--disable-dev-shm-usage");
        options.AddArgument("--log-level=3");
        options.AddArgument("--silent");
        options.AddExcludedArgument("enable-logging");

        IWebDriver driver = new ChromeDriver(options);

        foreach (string url in urls)
        {
            try
            {
                Console.WriteLine($"\n🌐 Processing: {url}");
                driver.Navigate().GoToUrl(url);
                Thread.Sleep(3000);

                string buttonSelector = "#main-content > div > div > div.b76820dbcdc0 > div > button:nth-child(3)";
                IWebElement button = driver.FindElement(By.CssSelector(buttonSelector));
                ((IJavaScriptExecutor)driver).ExecuteScript("arguments[0].scrollIntoView();", button);
                button.Click();

                WaitForDownload(downloadDir);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Error processing {url}: {ex.Message}");
            }
        }

        driver.Quit();
        Console.WriteLine($"\n  All files have been downloaded successfully! They are saved in: {downloadDir}");

        Console.Write("\n📊 Would you like to convert the downloaded JSON reports to CSV? (y/n): ");
        string convertInput = Console.ReadLine()?.Trim().ToLower();
        if (convertInput == "y")
        {
            SuppressConsoleOutput(() => ConvertJsonToCsv(downloadDir, csvDir));
        }
    }

    static void ConvertJsonToCsv(string jsonDir, string csvDir)
    {
        if (!Directory.Exists(csvDir))
        {
            Directory.CreateDirectory(csvDir);
        }

        string consolidatedFile = Path.Combine(csvDir, "Report.csv");
        bool firstFile = true;

        string[] headers = { "ruleId", "description", "help", "helpUrl", "impact", "isManual", "needsReview", "igt",
                         "selector", "summary", "source", "tags", "foundBy", "testName", "testUrl", "shareURL",
                         "createdAt", "screenshotURL" };

        string[] extendedHeaders = new[] { "Source" }.Concat(headers).ToArray();

        using (StreamWriter consolidatedWriter = new StreamWriter(consolidatedFile))
        {
            foreach (string file in Directory.GetFiles(jsonDir, "*.json"))
            {
                try
                {
                    string content = File.ReadAllText(file);
                    JObject jsonData = JObject.Parse(content);
                    JArray issues = (JArray)jsonData["allIssues"];
                    if (issues == null || issues.Count == 0)
                    {
                        Console.WriteLine($"⚠ No issues found in {file}. Skipping...");
                        continue;
                    }

                    string fileNameWithoutExt = Path.GetFileNameWithoutExtension(file);
                    string csvFileName = Path.Combine(csvDir, fileNameWithoutExt + ".csv");

                    using (StreamWriter sw = new StreamWriter(csvFileName))
                    {
                        sw.WriteLine(string.Join(",", extendedHeaders));
                        foreach (JObject issue in issues)
                        {
                            var row = new[] { fileNameWithoutExt.Split('-')[0] }
                                         .Concat(headers.Select(header => issue[header]?.ToString()?.Replace("\"", "\"\"") ?? ""))
                                         .ToArray();
                            sw.WriteLine(string.Join(",", row.Select(value => $"\"{value}\"")));
                        }
                    }

                    using (StreamReader reader = new StreamReader(csvFileName))
                    {
                        string line;
                        if (firstFile)
                        {
                            line = reader.ReadLine();
                            consolidatedWriter.WriteLine(line);
                            firstFile = false;
                        }
                        else
                        {
                            reader.ReadLine();
                        }

                        while ((line = reader.ReadLine()) != null)
                        {
                            consolidatedWriter.WriteLine(line);
                        }
                    }

                    Console.WriteLine($"✅ Converted {file} to CSV and added it to the consolidated report.");
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"❌ Error processing {file}: {ex.Message}");
                }
            }
        }

        Console.WriteLine($"\n📊 All CSV files have been successfully consolidated into: {consolidatedFile}");
    }

    static string[] ReadUrlsFromExcel(string filePath)
    {
        System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        using (var stream = File.Open(filePath, FileMode.Open, FileAccess.Read))
        using (var reader = ExcelReaderFactory.CreateReader(stream))
        {
            var result = reader.AsDataSet();
            if (result.Tables.Count > 0)
            {
                var table = result.Tables[0];
                return table.AsEnumerable().Skip(1)
                            .Select(row => row[0]?.ToString())
                            .Where(url => !string.IsNullOrEmpty(url))
                            .ToArray();
            }
        }
        return new string[0];
    }

    static void WaitForDownload(string downloadPath)
    {
        Console.WriteLine("⏳ Waiting for downloads to complete...");
        bool downloading;
        do
        {
            var files = Directory.GetFiles(downloadPath);
            downloading = files.Any(f => f.EndsWith(".crdownload"));
            Thread.Sleep(1000);
        } while (downloading);
        Console.WriteLine("✅ Download completed successfully!");
    }

    static void SuppressConsoleOutput(Action action)
    {
        TextWriter originalConsoleOut = Console.Out;
        try
        {
            Console.SetOut(TextWriter.Null);
            action();
        }
        finally
        {
            Console.SetOut(originalConsoleOut);
        }
    }
}
