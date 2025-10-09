using OfficeOpenXml;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace Lab2App
{
    class FileHandler
    {
        public bool IsFilePresent(string path, string filename)
        {
            return File.Exists(Path.Combine(path, filename));
        }
    }

    class ExcelHandler
    {
        private FileInfo file = null;
        private ExcelPackage ep = null;

        public ExcelHandler()
        {
            ExcelPackage.License.SetNonCommercialPersonal("Politechnika Gdańska - Lab JP.NET");
            file = new FileInfo(@"labEpp.xlsx");
            file.Delete();
        }
            
        public void CreateExcelSheet(string name)
        {
            ep = new ExcelPackage(file);
            ExcelWorksheet ws = ep.Workbook.Worksheets.Add(name);
            ws.Cells.AutoFitColumns(0);
            ep.Save();
        }

        public ExcelPackage GetExcelPackage()
        {
            return ep;
        }

        public int SaveFolderStructure(string worksheetName, string path, int searchDepth, int rowNumber = 1, int outline = 0)
        {
            if (searchDepth < 1)
            {
                return rowNumber;
            }
            
            if (!Directory.Exists(path))
            {
                throw new DirectoryNotFoundException($"Podana ścieżka {path} jest nieprawidłowa.");
            }

            ExcelPackage ep = GetExcelPackage();
            ExcelWorksheet ws = ep.Workbook.Worksheets[worksheetName];
            var dir = new DirectoryInfo(path);

            // start
            ws.Cells[rowNumber, 1].Value = dir;
            ws.Row(rowNumber).OutlineLevel = outline;
            rowNumber++;

            // check for files at start folder
            foreach (FileInfo file in dir.GetFiles())
            {
                try
                {

                    ws.Cells[rowNumber, 1].Value = file;
                    ws.Cells[rowNumber, 2].Value = file.Extension;
                    ws.Cells[rowNumber, 3].Value = file.Length;
                    ws.Cells[rowNumber, 4].Value = file.Attributes;
                    ws.Row(rowNumber).OutlineLevel = outline + 1;
                    rowNumber++;
                }
                catch (UnauthorizedAccessException)
                {
                    Console.WriteLine($"Brak dostępu do pliku: {file.FullName}");
                    continue;
                }
            }
            // run for subfolders
            foreach (DirectoryInfo subDir in dir.GetDirectories())
            {
                rowNumber = SaveFolderStructure(worksheetName, subDir.FullName, searchDepth - 1, rowNumber, outline + 1);
            }
            ws.Column(1).AutoFit();
            ep.Save();

            return rowNumber;
        }

        public void Generate10BiggestFilesStatistic(string statisticSheetName, string dataFeederSheetName, int foundObjectsNumber)
        {
            ExcelPackage ep = GetExcelPackage();
            ExcelWorksheet ws = ep.Workbook.Worksheets[dataFeederSheetName];
            ExcelWorksheet savews = ep.Workbook.Worksheets[statisticSheetName];

            // obatin data
            List<(string Name, long Size, string Type)> allFiles = new List<(string Name, long Size, string Type)>();

            for (int i = 1; i < foundObjectsNumber - 1; i++)
            {
                if (ws.Cells[i, 3].Value != null)
                {
                    allFiles.Add((
                                 ws.Cells[i, 1].Text,
                                 int.Parse(ws.Cells[i, 3].Text),
                                 ws.Cells[i, 2].Text
                             ));
                }
            }
            List<(string Name, long Size, string Type)> top10 = allFiles.OrderByDescending(f => f.Size).Take(10).ToList();
            
            // make report
            int row = 1;
            foreach (var file in top10)
            {
                //Console.WriteLine($"Name: {file.Name}, Size: {file.Size}, Type: {file.Type}");
                savews.Cells[row, 1].Value = file.Name;
                savews.Cells[row, 2].Value = file.Size;
                savews.Cells[row, 3].Value = file.Type;
                row++;
            }
            savews.Column(1).AutoFit();
            ep.Save();

            // make stats
            var stats = top10.GroupBy(f => f.Type).Select(g => (Extension: g.Key, Count: g.Count(), TotalSize: g.Sum(f => f.Size)) ).ToList();

            row = 1;
            foreach (var item in stats)
            {
                savews.Cells[row, 4].Value = item.Extension;
                savews.Cells[row, 5].Value = item.Count;
                savews.Cells[row, 6].Value = item.TotalSize;
                row++;
            }
            ep.Save();

            // make piecharts
            var chartCount = savews.Drawings.AddChart("PieChartCount", OfficeOpenXml.Drawing.Chart.eChartType.Pie3D) as OfficeOpenXml.Drawing.Chart.ExcelPieChart;
            chartCount.Series.Add(
                savews.Cells[$"E1:E{stats.Count}"],
                savews.Cells[$"D1:D{stats.Count}"]
            );
            chartCount.Title.Text = "% rozszerzeń ilościowo";
            chartCount.SetPosition(0, 0, 7, 0);
            chartCount.SetSize(400, 300);

            // --- Pie chart: Size per Extension ---
            var chartSize = savews.Drawings.AddChart("PieChartSize", OfficeOpenXml.Drawing.Chart.eChartType.Pie3D) as OfficeOpenXml.Drawing.Chart.ExcelPieChart;
            chartSize.Series.Add(
                savews.Cells[$"F1:F{stats.Count}"],
                savews.Cells[$"D1:D{stats.Count}"]
            );
            chartSize.Title.Text = "% rozszerzeń wg rozmiaru";
            chartSize.SetPosition(15, 0, 7, 0);
            chartSize.SetSize(400, 300);

            ep.Save();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            ExcelHandler excelHandler = new ExcelHandler();
            FileHandler fileHandler = new FileHandler();

            string worksheetName = "Struktura katalogu";
            string statisticsSheetName = "Statystyki";
            string pathToSearch = @"C:\\Users\Kuba_loc\Desktop\";
            int searchDepth = 3;

            excelHandler.CreateExcelSheet(worksheetName);
            Console.WriteLine("Status utworzenia pliku: " + fileHandler.IsFilePresent(".", "labEpp.xlsx"));

            int foundObjectsNumber = excelHandler.SaveFolderStructure(worksheetName, pathToSearch, searchDepth);
            Console.WriteLine("Ścieżkę " + pathToSearch + " poddano eksploracji do głębokości " + searchDepth);

            excelHandler.CreateExcelSheet(statisticsSheetName);
            excelHandler.Generate10BiggestFilesStatistic(statisticsSheetName, worksheetName, foundObjectsNumber);
            Console.WriteLine("Wygenerowano raport 10 największych pików");
        }
    }
}