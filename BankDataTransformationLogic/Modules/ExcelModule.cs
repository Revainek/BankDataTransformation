using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;

namespace BankDataTransformationLogic.Modules
{
    public interface IExcelModule
    {
        Excel.Application GetExcel();
        void KillExcel();
        string GetStringCellLocation(int row, int column);
    }
    public class ExcelModule : IExcelModule
    {
        private Excel.Application excelApp;

        public ExcelModule()
        {
         
        }

        public Excel.Application GetExcel()
        {
            if (excelApp != null)
            {
            }
            else { excelApp = new Excel.Application(); }

            return excelApp;
        }
        public void KillExcel()
        {
            if (excelApp != null)
            {
                Process temp = GetExcelProcess(excelApp);
                temp.Kill();
                excelApp = null;
            }
        }
        Process GetExcelProcess(Excel.Application excelApp)
        {
            GetWindowThreadProcessId(excelApp.Hwnd, out int id);
            return Process.GetProcessById(id);
        }
        [DllImport("user32.dll")]
        static extern int GetWindowThreadProcessId(int hWnd, out int lpdwProcessId);

        public string GetStringCellLocation(int row, int column)
        {
            return GetCellLocation(row, column);
        }
        private string GetCellLocation(int row, int column)
        {
            const string letters = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";

            var value = "";

            if (column >= letters.Length)
                value += letters[column / letters.Length - 1];

            value += letters[column % letters.Length];

            return value + row.ToString();
        }
    }
}
