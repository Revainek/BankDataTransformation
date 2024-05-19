using System;
using System.Collections.Generic;
using System.Text;

namespace BankDataTransformationLogic.Models
{
    public class TransformedEntry
    {
        public MEntry entry { get; set; }
        public TransformedEntry(MEntry test)
        {
            
            this.PreviewDescription = test.Description;
            this.PreviewDate = test.OperationDate.ToString("dd.MM.yyyy");
            this.PreviewAmount = test.Amount.ToString("0.00");
            this.PreviewCategory = "";
            entry = test;
        }

        public string PreviewDate { get; set; }
        public string PreviewCategory { get; set; }
        public string PreviewDescription { get; set; }
        public string PreviewAmount { get; set; }
    }
    public class TransformedHistory : List<TransformedEntry>
    {
        public TransformedHistory()
        {
                
        }
        public TransformedHistory(List<TransformedEntry> entries)
        {
            this.AddRange(entries);
        }
    }
}
