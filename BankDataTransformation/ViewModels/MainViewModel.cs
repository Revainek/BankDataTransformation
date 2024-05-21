using BankDataTransformationLogic.Models;
using BankDataTransformationLogic.Modules;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BankDataTransformation.ViewModels
{
    public interface IMainViewModel
    {
        int CurrentSelectedRule { set; }
        string CurrentFile { get; }
        public TransformedHistory TransformedAccountHistory { get; }
        public BuilderRules AvailableBuilderRules { get; }
        void ApplyAllRules();
        void ApplyRule();
        void DeleteRule();
        void LoadFileFromPath();
        void ResetRules();
    }
    public class MainViewModel : IMainViewModel, INotifyPropertyChanged
    {
        private BuilderRules availableBuilderRules;

        public BuilderRules AvailableBuilderRules
        {
            get { return availableBuilderRules; }
            set { 
                if (AvailableBuilderRules!=value)
                {
                    availableBuilderRules = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private TransformedHistory transformedAccountHistory;

        public TransformedHistory TransformedAccountHistory
        {
            get { return transformedAccountHistory; }
            set { transformedAccountHistory = value;
                NotifyPropertyChanged(); }
        }
    

        private IAccountHistoryReader _accountHistoryReader;
        private IAccountHistoryRebuilder _accHistoryBuilder;

        public MainViewModel(IAccountHistoryReader accHistoryReader, IAccountHistoryRebuilder accHIstoryBuilder) 
        { 
            _accountHistoryReader = accHistoryReader;
            _accHistoryBuilder = accHIstoryBuilder;
            AvailableBuilderRules = _accHistoryBuilder.GetRules();
        }



        
        private async Task LoadSelectedFile()
        {
            await Task.Run(() =>
            {
                try
                {

                    var testInfo2 = _accountHistoryReader.LoadXLS(CurrentFile);
                    TransformedAccountHistory = BuildRawTransformedHistory(testInfo2);
                }
                catch (IOException ext)
                {
                    System.Windows.MessageBox.Show(ext.Message + "\n" + ext.StackTrace);
                }
            });
        }

        private TransformedHistory BuildRawTransformedHistory(AccountMHistory testInfo2)
        {
            TransformedHistory mEntries = new TransformedHistory();
            foreach (var test in testInfo2)
            {
                mEntries.Add(new TransformedEntry(test));
            }
            return mEntries;
        }

        public void ApplyAllRules()
        {
            TransformedAccountHistory = _accHistoryBuilder.ApplyAll(TransformedAccountHistory);
        }
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "") => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

      

        public async void LoadFileFromPath()
        {
            var dialog = new OpenFileDialog()
            {
                FileName = "",
                Multiselect = false,
                Title = "Select Bank exported data file",
                Filter = "Excel files (*.xls;*.xlsx)|*.xls;*.xlsx"
            };
            var result = dialog.ShowDialog();
            if (result==true)
            {
                CurrentFile = dialog.FileName;
                await LoadSelectedFile();
            }
        }

        public void ApplyRule()
        {
            if (CurrentSelectedRule>-1)
            {
                TransformedAccountHistory = _accHistoryBuilder.ApplyRule(TransformedAccountHistory,AvailableBuilderRules[CurrentSelectedRule]);
            }
        }

        public void DeleteRule()
        {
           if (CurrentSelectedRule>-1)
            {
                try
                {
                    AvailableBuilderRules.RemoveAt(CurrentSelectedRule);
                    NotifyPropertyChanged(nameof(AvailableBuilderRules));
                }
                catch (IndexOutOfRangeException)
                {

                }
            }
        }

        public async void ResetRules()
        {
           await LoadSelectedFile();
        }

        #region Properties
        private string currentFile = "";

        public string CurrentFile
        {
            get { return currentFile; }
            set {
            if (currentFile!=value)
                {
                    currentFile = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int currentSelectedRule = -1;

        public int CurrentSelectedRule
        {
            get { return currentSelectedRule; }
            set
            {
                if (currentSelectedRule != value)
                {
                    currentSelectedRule = value;
                    NotifyPropertyChanged();
                }
            }
        }

        #endregion
    }
}
