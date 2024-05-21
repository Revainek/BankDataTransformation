using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Dynamic.Core;
using System.Linq.Dynamic;
using System.Xml.Serialization;
using System.IO;
using Microsoft.Office.Interop.Excel;
using static BankDataTransformationLogic.Models.BuildRule;

namespace BankDataTransformationLogic.Models
{
    [Serializable,XmlInclude(typeof(BuildRule)), XmlInclude(typeof(STransformation))]
    public class BuilderRules : List<BuildRule>
    {
        public List<string> GetAvailableRules()
        {
            return this.Select(x=> x.Name).ToList();
        }
        public IEnumerable<MEntry> ApplyRule(IQueryable<MEntry> Input, string name)
        {
            return Input.Where(this.First(x => x.Name == name).RuleValue);
        }
        public void AddNewRule(string rulename,string ruleValue)
        {
            BuildRule rule = new BuildRule()
            {
                Name = rulename,
                RuleValue = ruleValue
            };
            this.Add(rule);
        }
        public void SaveRules(string path)
        {
            try
            {
                using (Stream streamWrite = File.Create(path))
                {
                    XmlSerializer formatter = new XmlSerializer(typeof(BuilderRules));
                    formatter.Serialize(streamWrite, this);
                }
            }
            catch (Exception ex)
            {
            }
        }
        public static BuilderRules LoadRules(string path)
        {
            BuilderRules _BRules = new BuilderRules();


            if (File.Exists(path)==false)
            {
                try
                {
                    if (Directory.Exists(path)==false)
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(path));
                    }
                    _BRules.SaveRules(path);
                }
                catch (IOException ex)
                {
                   
                }
            }
            
            try
            {
                using (FileStream fs = new FileStream(path, FileMode.Open, FileAccess.Read))
                {
                    XmlSerializer x = new XmlSerializer(typeof(BuilderRules));
                    object obj = x.Deserialize(fs);
                    _BRules = (BuilderRules)obj;
                }
            }
            catch (Exception)
            {
            }
           
            return _BRules;
         
        }

        internal TransformedHistory ApplyRule(TransformedHistory currentTH, string name)
        {
            var Rule = this.First(x => x.Name == name);
            if (Rule.RuleType==RuleTypeEnum.Filter)
            {
                var result1 = currentTH.AsQueryable().Where(Rule.RuleValue);
                return new TransformedHistory(result1.ToList());
            }
            else if (Rule.RuleType==RuleTypeEnum.Transform)
            {
                
                if (Rule.RuleValueSetters != null)
                {
                    foreach (var rvs in Rule.RuleValueSetters)
                    {
                        foreach (var item in currentTH.AsQueryable().Where(Rule.RuleValue))
                        {
                            var itemPropValue = item.GetType().GetProperty(rvs.First).GetValue(item, null) as string;

                            item.GetType().GetProperty(rvs.First).SetValue(item, rvs.Second.Perform(itemPropValue), null);
                        }
                    }
                }
                return new TransformedHistory(currentTH);
            }
            return currentTH;
        }
        #region TestValues
        /// <summary>
        /// Test values for overview of initial possibilities
        /// </summary>
        private void TestValues()
        {
            this.Add(new BuildRule()
            {
                Name = "Filter Test",
                RuleValue = "x=>x.PreviewDescription.Contains(\"AUTOOSZCZĘDZANIE\")==false",
                RuleType = RuleTypeEnum.Filter
            });
            this.Add(new BuildRule()
            {
                Name = "Replace Values Test",
                RuleValue = "x=>x.entry.Amount<0",
                RuleValueSetters = new List<ObjectPair<string, STransformation>>()
                                 {
                                 new ObjectPair<string,STransformation>("PreviewAmount",new STransformation("Replace","-","")),
                                 new ObjectPair<string,STransformation>("PreviewAmount",new STransformation("Replace",",","."))
                                 },

                RuleType = RuleTypeEnum.Transform
            });
            this.Add(new BuildRule()
            {
                Name = "New Value Test",
                RuleValue = "x=>x.entry.Description.Contains(\"ORLEN\")==true",
                RuleValueSetters = new List<ObjectPair<string, STransformation>>()
                                 {
                 new ObjectPair<string,STransformation>("PreviewDescription",new STransformation("NewValue","Tankowanie")),
                 new ObjectPair<string,STransformation>("PreviewCategory",new STransformation("NewValue","Benzyna")),
                                 },

                RuleType = RuleTypeEnum.Transform
            });

        }
        #endregion
    }

    [Serializable,XmlInclude(typeof(ObjectPair<string, STransformation>))]
    public class BuildRule
    {
      public string Name { get; set; }
      public RuleTypeEnum RuleType { get; set; }
     
      public string RuleValue { get; set; }
      public List<ObjectPair<string, STransformation>> RuleValueSetters { get; set; }
    }
    public enum RuleTypeEnum
    {
        Filter,
        Transform
    }
    [Serializable]
    public class STransformation
    {
        public string Type { get; set; }

        public string P1 { get;  set; }
        public string P2 { get;  set; }
        public STransformation()
        {
                
        }
        public STransformation(string type, string p1, string p2 = "")
        {
            Type = type;
            P1 = p1;
            P2 = p2;
        }
        public string Perform(string InputValue)
        {
            switch (this.Type)
            {
                case "Replace":
                    return Replace(InputValue, P1, P2);
                case "NewValue":
                    return NewValue(InputValue, P1);
            }
            return InputValue;
        }
        public string Replace(string original,string replacedValue,string replacement)
        {
            return original.Replace(replacedValue, replacement);
        }
        public string NewValue(string original, string replacedValue)
        {
            return replacedValue;
        }
    }
}
