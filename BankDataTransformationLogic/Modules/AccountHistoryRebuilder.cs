using BankDataTransformationLogic.Models;
using Microsoft.Office.Interop.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

namespace BankDataTransformationLogic.Modules
{
    public interface IAccountHistoryRebuilder
    {
        TransformedHistory ApplyAll(TransformedHistory currentTH);
        TransformedHistory ApplyRule(TransformedHistory currentTH,BuildRule buildRule);
        BuilderRules GetRules();
        void AddRule();
        void RemoveRule();
        void UndoRuleApplication();
      
    }
    public class AccountHistoryRebuilder : IAccountHistoryRebuilder
    {
        BuilderRules _brules;

        public AccountHistoryRebuilder()
        {
           
            string defaultSavePath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments) + "\\BankDataTransformation\\BuildRules.xml";
            InitializeRules(defaultSavePath);
        }

        public void AddRule()
        {
            throw new NotImplementedException();
        }

        public TransformedHistory ApplyRule(TransformedHistory currentTH, BuildRule buildRule)
        {
            if (buildRule!=null)
            {
                currentTH = _brules.ApplyRule(currentTH, buildRule.Name);
            }
            return currentTH;
        }

        public BuilderRules GetRules()
        {
          if (_brules!=null)
            {
                return _brules;
            }
          else{
                return new BuilderRules();
            }
        }

        public void RemoveRule()
        {
            throw new NotImplementedException();
        }

        public void ResetRules()
        {
            throw new NotImplementedException();
        }

        public void UndoRuleApplication()
        {
            throw new NotImplementedException();
        }

        TransformedHistory IAccountHistoryRebuilder.ApplyAll(TransformedHistory currentTH)
        {
            if (_brules != null)
            {
                foreach (var rule in _brules)
                {
                    currentTH = _brules.ApplyRule(currentTH, rule.Name);
                }
            }
            return currentTH;
        }

        private void InitializeRules(string Path)
        {
            _brules = BuilderRules.LoadRules(Path);
        }
    }
}
