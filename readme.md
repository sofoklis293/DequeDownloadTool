# 🚀 Deque Assistant Tool

## 📌 Overview
Deque Assistant Tool is a command-line utility designed to automate the process of downloading accessibility reports from given URLs, converting them into CSV format, and consolidating the data for further analysis. This tool uses **Selenium WebDriver** to navigate through webpages, **ExcelDataReader** to extract URLs from an Excel file, and **Newtonsoft.Json** for JSON parsing.

---

## 📂 Features
✅ **Bulk Download Deque Axe Dev Tools Extension** – Uses Selenium WebDriver to fetch accessibility reports from specified URLs.    
✅ **JSON to CSV Conversion** – Converts downloaded JSON reports into structured CSV format.  
✅ **Provides a consolidated report** – Merges all individual CSV files into a single consolidated report.  

---

## 🛠 Technologies Used
- **C# (.NET Core)**
- **Selenium WebDriver** (ChromeDriver)
- **Newtonsoft.Json** (for JSON parsing)
- **ExcelDataReader** (for reading Excel files)

---

## 📥 Installation & Setup

### 🔹 Prerequisites
Ensure you have the following installed on your system:

1. **.NET SDK**  
   [Download from .NET official site](https://dotnet.microsoft.com/en-us/download)
2. **Google Chrome Browser**  
   [Download Google Chrome](https://www.google.com/chrome/)
3. **ChromeDriver** matching your Chrome version  
   [Download ChromeDriver](https://sites.google.com/chromium.org/driver/)
4. **Required NuGet Packages**
   Install the necessary dependencies via the command line:

   ```sh
   dotnet add package ExcelDataReader
   dotnet add package Selenium.WebDriver
   dotnet add package Newtonsoft.Json
   
---

## 📥 How to run


1. Navigate through terminal to the projects root directory
   
2. Compile and execute the tool via the terminal or command prompt:
   ```sh
   dotnet run
