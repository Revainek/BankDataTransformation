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
    [Serializable,XmlInclude(typeof(BuildRule))]
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
            // Temporary test

            _BRules.Add(new BuildRule() 
            { 
                Name = "Filter AutoSaver", 
                RuleValue = "x=>x.PreviewDescription.Contains(\"AUTOOSZCZĘDZANIE\")==false",
                RuleType=RuleTypeEnum.Filter 
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Filter Incomes",
                RuleValue = "x=>x.entry.Amount<0",
                RuleType = RuleTypeEnum.Filter
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Reverse Amount Values",
                RuleValue = "x=>x.entry.Amount<0",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            { 
                                            new Tuple<string,STransformation>("PreviewAmount",new STransformation("Replace","-","")),
                                            new Tuple<string,STransformation>("PreviewAmount",new STransformation("Replace",",","."))
                                            },
                
                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Filter Internal account Transfers",
                RuleValue = "x=>(x.entry.ReceiverInformation.Name.Contains(\"PAWEŁ\") && x.entry.ReceiverInformation.Name.Contains(\"ŚWIĘCIAK\"))==false",
                RuleType = RuleTypeEnum.Filter
            });
            _BRules.Add(new BuildRule()
            {
                Name = "FuelRule",
                RuleValue = "x=>x.entry.Description.Contains(\"ORLEN\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Tankowanie Ford")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","benzyna")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "EatingAtWork1",
                RuleValue = "x=>x.entry.Description.Contains(\"Piek.-Cukiernia Scigala\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Cukiernia pod pracą")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "EatingAtWork2",
                RuleValue = "x=>x.entry.Description.Contains(\"BUDDA\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Buddha praca")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie miasto")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Clothes1",
                RuleValue = "x=>x.entry.Description.Contains(\"zalando\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zalando zakupy")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","odzież i obuwie")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "EatingOut1",
                RuleValue = "x=>x.entry.Description.Contains(\"MCDONALDS\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Jedzenie McDonalds")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie miasto")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "EatingOut1",
                RuleValue = "x=>x.entry.Description.Contains(\"CIACHOMANIA\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Ciachomania ciasta")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });


            _BRules.Add(new BuildRule()
                 {
                     Name = "Diaries1",
                     RuleValue = "x=>x.entry.Description.Contains(\"LIDL\")==true",
                     RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy Lidl")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie")),
                                            },

                     RuleType = RuleTypeEnum.Transform
                 });
            _BRules.Add(new BuildRule()
            {
                Name = "Diaries2",
                RuleValue = "x=>x.entry.Description.Contains(\"AUCHAN\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy Auchan")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie")),
                                            },

                RuleType = RuleTypeEnum.Transform
            }); 
                _BRules.Add(new BuildRule()
                {
                    Name = "Diaries3",
                    RuleValue = "x=>x.entry.Description.Contains(\"JMP S.A. BIEDRONKA\")==true",
                    RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy Biedra")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","jedzenie")),
                                            },

                    RuleType = RuleTypeEnum.Transform
                });
            _BRules.Add(new BuildRule()
            {
                Name = "E-Cigarettes1",
                RuleValue = "x=>x.entry.Description.Contains(\"Smoke Shop\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","E Fajki w Gemini")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","papierosy")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "E-Cigarettes2",
                RuleValue = "x=>x.entry.Description.Contains(\"Adres : Sklep Wielobranzowy Miasto : Tychy Kraj : POLSKA\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","E Fajki w Gemini")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","papierosy")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "E-Cigarettes3",
                RuleValue = "x=>x.entry.Description.Contains(\"ZABKA\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy żabka")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","?")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });


            _BRules.Add(new BuildRule()
            {
                Name = "HomeShopping1",
                RuleValue = "x=>x.entry.Description.Contains(\"Castorama\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy Castorama")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","do mieszkania")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "HomeShopping2",
                RuleValue = "x=>x.entry.Description.Contains(\"DOZ APTEKA\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Zakupy Castorama")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","leki")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Gaming1",
                RuleValue = "x=>x.entry.Description.Contains(\"store.steampowered.com\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Gry na steam")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","hobby i sporty")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });

            _BRules.Add(new BuildRule()
            {
                Name = "Transport1",
                RuleValue = "x=>x.entry.Description.Contains(\"PARKING\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Parkowanie")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","taxi i bilety")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });
            _BRules.Add(new BuildRule()
            {
                Name = "Transport2",
                RuleValue = "x=>x.entry.Description.Contains(\"UBER.COM\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","Uber Taxi")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","taxi i bilety")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });

            _BRules.Add(new BuildRule()
            {
                Name = "Systematic1",
                RuleValue = "x=>x.entry.Description.Contains(\"PIT28\")==true",
                RuleValueSetters = new List<Tuple<string, STransformation>>()
                                            {
                    new Tuple<string,STransformation>("PreviewDescription",new STransformation("NewValue","podatek najem")),
                    new Tuple<string,STransformation>("PreviewCategory",new STransformation("NewValue","opłaty mieszkanie (stałe)")),
                                            },

                RuleType = RuleTypeEnum.Transform
            });

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
                            var itemPropValue = item.GetType().GetProperty(rvs.Item1).GetValue(item, null) as string;

                            item.GetType().GetProperty(rvs.Item1).SetValue(item, rvs.Item2.Perform(itemPropValue), null);
                        }
                    }
                }
                return new TransformedHistory(currentTH);
            }
            return currentTH;
        }
    }

    [Serializable]
    public class BuildRule
    {
      public string Name { get; set; }
      public RuleTypeEnum RuleType { get; set; }
     
      public string RuleValue { get; set; }
      public List<Tuple<string, STransformation>> RuleValueSetters { get; set; }
    }
    public enum RuleTypeEnum
    {
        Filter,
        Transform
    }
    public class STransformation
    {
        public string Type { get; set; }

        private string P1 { get; set; }
        private string P2 { get; set; }

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
