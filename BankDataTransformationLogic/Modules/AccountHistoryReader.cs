using BankDataTransformationLogic.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using Excel = Microsoft.Office.Interop.Excel;

namespace BankDataTransformationLogic.Modules
{
    public interface IAccountHistoryReader
    {
        AccountMHistory LoadXLS(string xlsPath);
      
    }
    public class AccountHistoryReader : IAccountHistoryReader
    {
        private readonly IExcelModule _excelModule;

        public AccountHistoryReader(IExcelModule excelModule)
        {
            this._excelModule = excelModule;
        }
        
        /// <summary>
        /// Loads the excel file exported directly from PKO Bank account webpage/application
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public AccountMHistory LoadXLS(string path)
        {
            AccountMHistory history = new AccountMHistory();
            CultureInfo culture = CultureInfo.CreateSpecificCulture("en-US");
            //Excel.Application excelApp;
            Excel._Workbook xlsBook;
            Excel._Worksheet workSheet;
            Excel.Application excelApp = _excelModule.GetExcel();

            try
            {
                excelApp.Visible = false;
                xlsBook = (Excel._Workbook)(excelApp.Workbooks.Open(path, 0, true, 5, "", "", true, Excel.XlPlatform.xlWindows, "\t", false, false, 0, true));
                var xlsSheets = xlsBook.Sheets as Excel.Sheets;

                workSheet = (Excel.Worksheet)xlsSheets.get_Item(1);

                string SitName = ((Excel.Range)workSheet.get_Range("B1", Missing.Value)).Value2 as string;
                int lastrow = 1;
                int lastcolumn = 1;
                GetLastRowAndColumn(workSheet, ref lastrow, ref lastcolumn);
               
                if (lastrow>2 && lastcolumn>2)
                {
                    List<string> columnNames = new List<string>();
                    GetNamesFromFirstRow(workSheet, lastcolumn, columnNames);


                    // Build a dictionary for each column, filling missing data with empty strings
                    Dictionary<string,List<string>> EntriesRaw = new Dictionary<string, List<string>>();
                    List<List<string>> UnnamedEntries = new List<List<string>>();
                    for (int col = 0; col < columnNames.Count; col++)
                    {
                        string column = columnNames[col];
                        List<string> stringRowValues = new List<string>();
                        
                            Excel.Range rows = workSheet.get_Range(
                                _excelModule.GetStringCellLocation(2,col) + ":" +
                                _excelModule.GetStringCellLocation(lastrow,col)
                                , Missing.Value);

                            object[,] rowsValues = rows.get_Value() as object[,];
                           
                            foreach (var rowValue in  rowsValues)
                            {
                                if (rowValue != null)
                                {
                                    stringRowValues.Add(rowValue.ToString());
                                }
                                else
                                {
                                    stringRowValues.Add("");
                                }
                                
                            }
                  
                        EntriesRaw.Add(column, stringRowValues);
                    }

                    //Get Additional Data From 4 untitled columns 
                    for (int row = 2; row <= lastrow; row++)
                    {
                        List<string> stringRowValues = new List<string>();

                        Excel.Range rows = workSheet.get_Range(
                            _excelModule.GetStringCellLocation(row, lastcolumn+1) + ":" +
                            _excelModule.GetStringCellLocation(row, lastcolumn + 4)
                            , Missing.Value);

                        object[,] rowsValues = rows.get_Value() as object[,];

                        foreach (var rowValue in rowsValues)
                        {
                            if (rowValue != null)
                            {
                                stringRowValues.Add(rowValue.ToString());
                            }
                            else
                            {
                                stringRowValues.Add("");
                            }

                        }

                        UnnamedEntries.Add(stringRowValues);
                    }


                    // Build MEntries for each row from data in dictionary
                    for (int i = 0; i < lastrow-1; i++)
                    {
                        MEntry entry = new MEntry()
                        {
                            OperationDate = DateTime.Parse(EntriesRaw["Data operacji"][i]),
                            BookingDate = DateTime.Parse(EntriesRaw["Data waluty"][i]),
                            TransactionType = EntriesRaw["Typ transakcji"][i],
                            Amount = Double.Parse(EntriesRaw["Kwota"][i]),
                            Currency = EntriesRaw["Waluta"][i],
                            BalanceAfterTransaction = Double.Parse(EntriesRaw["Kwota"][i]),
                            SenderInformation = new SenderInfo(EntriesRaw["Rachunek nadawcy"][i], EntriesRaw["Nazwa nadawcy"][i], EntriesRaw["Adres nadawcy"][i]),
                            ReceiverInformation = new ReceiverInfo(EntriesRaw["Rachunek odbiorcy"][i], EntriesRaw["Nazwa odbiorcy"][i], EntriesRaw["Adres odbiorcy"][i]),
                            Description = EntriesRaw["Opis transakcji"][i] + string.Join(" ", UnnamedEntries[i]),
                        };
                        history.Add(entry);
                    }

                }

            }
            catch (SerializationException e)
            {

            }
            return history;
        }

        private void GetNamesFromFirstRow(_Worksheet workSheet, int lastcolumn, List<string> columnNames)
        {
            Excel.Range columns = workSheet.get_Range("A1:" + _excelModule.GetStringCellLocation(1, lastcolumn), Missing.Value);
            object[,] columnValues = columns.get_Value() as object[,];

            foreach (string value in columnValues)
            {
                columnNames.Add(value.ToString());
            }
        }

        private void GetLastRowAndColumn(_Worksheet workSheet, ref int lastrow, ref int lastcolumn)
        {
            for (int i = 1; i <= 1000; i++)
            {

                if (workSheet.get_Range("A" + i).Value2 == null)
                {
                    lastrow = i - 1;
                    break;
                }
                else
                {
                    continue;
                }
            }

            for (int i = 1; i <= 1000; i++)
            {

                if (workSheet.get_Range(_excelModule.GetStringCellLocation(1, i)).Value2 == null)
                {
                    lastcolumn = i - 1;
                    break;
                }
                else
                {
                    continue;
                }
            }
        }
    }
}
