# 🚀Axe DevTools Extension - Bulk Report Downloader

## 📌 Overview
This Tool is command-line based, designed to automate the process of downloading accessibility reports generated from Axe DevTools extension as a JSON format, given the URLs. Converting them into CSV format, and consolidating the data into single CSV for further analysis.

---

## 📂 Features

✅ **Bulk JSON Report Download** – Reads URLs from an Excel file and downloads the JSON reports.  
✅ **CSV Conversion** – Converts all the JSON reports into CSV.   
✅ **Data Consolidation** – Merges all individual CSV files into a single consolidated csv report.  

---

## 🛠 Technologies Used
- **C# (.NET 6)**
- **Selenium WebDriver** (ChromeDriver)
- **Newtonsoft.Json** (for JSON parsing)
- **ExcelDataReader** (for reading Excel files)
- **System.IO** (for file handling)

---

## 📥 Installation & Setup

### 🔹 Prerequisites
Ensure you have the following installed on your system:

1. **.NET SDK**  
   [Download from .NET official site](https://dotnet.microsoft.com/en-us/download/dotnet/6.0)
2. **Google Chrome Browser** - Skip if you already have Google Chrome  
   [Download Google Chrome](https://www.google.com/chrome/)
3. **ChromeDriver** matching your Chrome version  
   [Download ChromeDriver](https://sites.google.com/chromium.org/driver/)

---
## 📌 Guide


1. Create a .xlsx file with one column named URL that contains all the report URLs
2. [Run the application](#-how-to-run).
3. Once the application runs, it will prompt you to enter the full path of the Excel file. You have to put the path to the above Excel file and press "Enter"
         - Mac: Drag the file into the terminal   
         - Windows: Right-click on the file "Copy as Path", and then paste it into the terminal. (Make sure you remove " character at the beginning and end)

## 📥 How to run


1. Navigate through terminal to the projects root directory
   
2. Compile and execute the tool via the terminal or command prompt:
   ```sh
   dotnet run
   
## 📂 Output


- **My Project**
    - **ReportUrls.xlsx** - provided by user:
    - **JSON_Reports_{timestamp}** - Generated
    - **CSV_Reports_{timestamp}** - Generated
        - **All CSV Reports** (Individual CSV Reports)
        - **Report.csv** (Consolidating Report)
        
        

