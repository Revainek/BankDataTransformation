using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;

namespace BankDataTransformationLogic.Models
{
    public class AccountMHistory : List<MEntry>
    {

    }

    public class MEntry
    {
        public MEntry() 
        {
        }
        public DateTime OperationDate { get;  set; }

        public DateTime BookingDate { get;  set; }

        public string TransactionType { get;   set; }
        public string Description { get;  set; }

        public double Amount { get;  set; }

        public string Currency { get;  set; }

        private double balanceAfterTransaction;

        public double BalanceAfterTransaction
        {
            get { return balanceAfterTransaction; }
            set { balanceAfterTransaction = value; }
        }
        public SenderInfo SenderInformation { get;  set; }
        public ReceiverInfo ReceiverInformation { get;  set; }
     

        
    }

    public class SenderInfo
    {
        public SenderInfo(string _ID, string _name, string _address)
        {
            AccountID = _ID;
            Name = _name;
            Address = _address;
        }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string AccountID { get; private set; }
    }
    public class ReceiverInfo
    {
        public ReceiverInfo(string _ID, string _name, string _address)
        {
            AccountID = _ID;
            Name = _name;
            Address = _address;
        }
        public string Name { get; private set; }
        public string Address { get; private set; }
        public string AccountID { get; private set; }
    }
    
    
}
