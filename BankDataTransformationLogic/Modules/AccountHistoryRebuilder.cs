using BankDataTransformationLogic.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace BankDataTransformationLogic.Modules
{
    public interface IAccountHistoryRebuilder
    {
        TransformedHistory ApplyAll(TransformedHistory currentTH);
        BuilderRules GetRules();
        void AddRule();
        void RemoveRule();

        void UndoRuleApplication();
        void ResetRules();

        
    }
    public class AccountHistoryRebuilder : IAccountHistoryRebuilder
    {
        BuilderRules _brules;

        public AccountHistoryRebuilder()
        {
            string defaultSavePath = "";
            InitializeRules(defaultSavePath);
        }

        public void AddRule()
        {
            throw new NotImplementedException();
        }

        public void ApplyAll(TransformedHistory currentTH)
        {
            if (_brules != null)
            {
                foreach (var rule in _brules)
                {
                    currentTH =  _brules.ApplyRule(currentTH,rule.Name);
                }
            }
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
