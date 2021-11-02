using ClosedXML.Excel;
using DrewMar.Domain;
using System.Linq;

namespace DrewMar
{
    internal class XlsxFileExporter : IFileExporter
    {
        int currentExcelCellVerticalPosition = 1;
        int currentExcelCellHorizontalPosition = 1;

        public void Export(string targetPath, Transport transport)
        {
            var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("paczki");

            var packageNumber = 0;
            foreach (var package in transport.packages)
            {
                packageNumber++;
                worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Value = $"Paczka nr. {packageNumber}";
                worksheet.Cell(currentExcelCellVerticalPosition + 1, currentExcelCellHorizontalPosition).Value = $"Grubość = {package.desks.First().Thick}";
                worksheet.Cell(currentExcelCellVerticalPosition + 2, currentExcelCellHorizontalPosition).Value = "Sztuki = " + package.desks.Count;
                worksheet.Cell(currentExcelCellVerticalPosition + 3, currentExcelCellHorizontalPosition).Value = "Masa paczki = " + package.Capacity;
                worksheet.Cell(currentExcelCellVerticalPosition + 4, currentExcelCellHorizontalPosition).Value = package.GetAssignedWorkers();
                UpdateCurrentExcelCellPosition(0, 1);

                worksheet.PageSetup.PrintAreas.Clear();
                worksheet.PageSetup.PrintAreas.Add("A1:P200");

                int startCellVert = currentExcelCellVerticalPosition;
                int startCellHor = currentExcelCellHorizontalPosition;

                foreach (var desk in package.desks)
                {
                    worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Value = desk.Length;
                    UpdateCurrentExcelCellPosition(1, 0);
                    worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Value = desk.Width;

                    if (desk == package.desks.Last())
                    {
                        UpdateCurrentExcelCellPositionAfterPackage(package.desks.Count, startCellVert);
                        worksheet.Row(currentExcelCellVerticalPosition - 1).Height = 8;
                    }
                    else
                    {
                        UpdateCurrentExcelCellPosition((-1), 1);
                    }
                }
            }

            worksheet.Columns("A:P").AdjustToContents();
            worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Value = "Transport skompletowany";
            UpdateCurrentExcelCellPosition(1, 0);
            worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            worksheet.Cell(currentExcelCellVerticalPosition, currentExcelCellHorizontalPosition).Value = transport.CompletionTime;

            UpdateCurrentExcelCellPosition(2, 0);

            worksheet.SheetView.ZoomScale = 75;

            worksheet.PageSetup.AdjustTo(80);

            workbook.SaveAs(targetPath);
        }

        public void UpdateCurrentExcelCellPosition(int vertical, int horizontal)
        {
            if (currentExcelCellHorizontalPosition + horizontal < 17)
            {
                currentExcelCellVerticalPosition += vertical;
                currentExcelCellHorizontalPosition += horizontal;
            }
            else
            {
                currentExcelCellHorizontalPosition = 2;
                currentExcelCellVerticalPosition += 1;
            }
        }

        public void UpdateCurrentExcelCellPositionAfterPackage(int numberOfDesks, int startCellVert)
        {
            if (numberOfDesks < 31)
            {
                currentExcelCellVerticalPosition = startCellVert + 6;
            }
            else
            {
                currentExcelCellVerticalPosition += 2;
            }

            currentExcelCellHorizontalPosition -= (currentExcelCellHorizontalPosition - 1);
        }
    }
}
