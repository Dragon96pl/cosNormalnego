﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace ConverterToTBL
{
    class ConverterGbv2
    {
        public List<TBLForSpecificColumn> tbl = new List<TBLForSpecificColumn>();
        public string readFile(string fileName, string newFileName)
        {
            var lines = File.ReadLines(fileName);
            Boolean start = false;
            string startString = "FEATURES";
            string endString = "//";
            string key = "";
            List<string> names = new List<string>();
            List<string> values = new List<string>();
            string value = "";
            string from = "";
            string to = "";
            int idOfFeatures = 1;
            List<SpecificColumn> listOfValues = new List<SpecificColumn>();
            foreach (var line in lines){
                string singleLine = line.Trim();
                singleLine = Regex.Replace(singleLine, " {2,}", "\t");
                string[] splitLine = singleLine.Split('\t');
                if (splitLine[0].Contains(endString)){
                    start = false;
                    TBLForSpecificColumn tblTemp = new TBLForSpecificColumn();
                    tblTemp.mainName = "Features" + idOfFeatures.ToString();
                    idOfFeatures++;
                    listOfValues.ForEach(element => tblTemp.columns.Add(element));
                    this.tbl.Add(tblTemp);
                    listOfValues.Clear();
                }
                    
                if (start){
                    if(splitLine.Length > 1){
                        if (value != "")
                            values.Add(value);
                        if (key != ""){
                          //  Console.WriteLine(key + " ==> " + value);
                            listOfValues.Add(new SpecificColumn(
                                key, from,to, names, values
                                ));
                            names.Clear();
                            values.Clear();
                            value="";
                        }
                        if (!splitLine[0].Contains("misc_featur")){
                            //Console.WriteLine(splitLine.Length.ToString());
                            key = splitLine[0];
                            from = splitLine[1].Substring(0, splitLine[1].IndexOf("."));
                            to = splitLine[1].Substring(splitLine[1].LastIndexOf(".") + 1);
                            from = from.Replace("\\D+", "");
                            from = from.Replace("(", "");
                            to = to.Replace("\\D+", "");
                            from = this.replaceAll(from);
                            to = this.replaceAll(to);
                            Regex pattern = new Regex("[a-zA-Z]");
                            to = pattern.Replace(to, "");
                            from  = pattern.Replace(from, "");
                        }
                        
                    }
                    else{
                        if (splitLine[0].Contains("/") && splitLine[0].Contains("=")){
                            if (value != "")
                                values.Add(value);
                            value = "";
                            names.Add(this.replaceAll(splitLine[0].Substring(0, splitLine[0].IndexOf("="))));
                            splitLine[0] = splitLine[0].Substring(splitLine[0].IndexOf("=") + 1);
                            splitLine[0] = this.replaceAll(splitLine[0]);
                        }
                        value += splitLine[0];
                    }

                }
                if (splitLine[0].Contains(startString))
                    start = true;

            }
            //for (int i = 0; i < values.Count; i++)
            //    Console.WriteLine(names[i] + " ==> " + values[i]);
            string results = "";
            foreach (var tbl in this.tbl)
                results += tbl.ToString() + "\n";
            if (!newFileName.Contains(".tbl"))
                newFileName += ".tbl";
            if (File.Exists(newFileName))
                File.Delete(newFileName);
            using (FileStream fs = File.Create(newFileName))
            {
                Byte[] info = new UTF8Encoding(true).GetBytes(results);
                fs.Write(info, 0, info.Length);
                System.Windows.Forms.MessageBox.Show("Created");
            }
            return "";
        }

        protected string replaceAll(string value){
            value = value.Replace("\"", "");
            value = value.Replace("(", "");
            value = value.Replace(")", "");
            value = value.Replace("=", "");
            value = value.Replace("/", "");
            value = value.Replace("\\", "");
            return value;
        }
    }
    
}
