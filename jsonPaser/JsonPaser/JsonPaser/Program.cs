using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using JsonPaser.Model;

namespace JsonPaser
{
    class Program
    {
        static void Main(string[] args)
        {
            string path = @"G:\研究工作\jsonPaser\JsonPaser\JsonPaser\result2.pml";
            String json = readFile(@"G:\研究工作\jsonPaser\JsonPaser\JsonPaser\test2.json");
            try
            {
                JObject jo = (JObject)JsonConvert.DeserializeObject(json);// JObject.Parse(json);
                IList<JsonPaser.Model.Type> types = new List<JsonPaser.Model.Type>();
                IList<JsonPaser.Model.Device> devices = new List<JsonPaser.Model.Device>();
                JArray jt = (JArray)jo["Specs"]["Types"];
                if(jt!=null)
                for (int i = 0; i < jt.Count; i++)
                {
                    JObject temp = (JObject)jt[i];
                    JsonPaser.Model.Type tt = new Model.Type(temp);
                    types.Add(tt);
                }
                JArray jd = (JArray)jo["Specs"]["Devices"]["Device"];
                if (jd != null)
                for (int i = 0; i < jd.Count; i++)
                {
                    JObject temp = (JObject)jd[i];
                    JsonPaser.Model.Device tt = new Model.Device(temp);
                    devices.Add(tt);
                }
                IList<Rules> rules = new List<Rules>();
                JArray jr = (JArray)jo["Specs"]["Rules"];
                if (jr != null)
                    for (int i = 0; i < jr.Count; i++)
                    {
                        JObject temp = (JObject)jr[i];
                        JsonPaser.Model.Rules tt = new Model.Rules(temp);
                        rules.Add(tt);
                    }
                FileStream fs = new FileStream(path, FileMode.OpenOrCreate);
                //实例化一个StreamWriter-->与fs相关联  
                StreamWriter sw = new StreamWriter(fs);
                
                ///*************************分析完成************************/
                foreach(Device de in devices)
                foreach (Variable var in de.Variables)
                {
                    if (var.Type == "int" || var.Type == "bool")
                    {
                        sw.WriteLine(var.Type + " " + var.Name + " = " + var.De_fault + ";");
                    }
                    else
                    {
                        sw.WriteLine("mtype " + var.Name + " = "+var.De_fault+";");
                    }
                }
                foreach (Device de in devices)
                {
                    sw.WriteLine("mtype " + de.Name + "_state;");
                }
                foreach (Device de in devices)
                {
                    if(de.APIs.Count>0)
                        foreach (API tt in de.APIs)
                        {
                            sw.WriteLine("mtype " + tt.Name + "_auth;");
                            string api_type = "";
                            if (tt.Para.Count == 0)
                                api_type = "byte";
                            else
                            {
                                api_type = "byte";
                                foreach (Para temp in tt.Para)
                                {
                                    if (temp.type == "int" || temp.type == "bool")
                                    {
                                        api_type +=","+temp.type;
                                       // sw.WriteLine(temp.type + " " + temp.name + ";");
                                    }
                                    else
                                    {
                                        api_type += ",mtype" ;
                                        //sw.WriteLine("mtype " + temp.name + ";");
                                    }
                                }
                              //  sw.WriteLine("};");
                              //  api_type = tt.Name + "_Para";
                            }
                            sw.WriteLine("chan " + tt.Name + " = [0] of {" + api_type + "}");
                            if(tt.Para.Count>0)
                            {
                                foreach(Para pp in tt.Para)
                                {
                                    sw.WriteLine(pp.type + " " + pp.name + ";");
                                    if (pp.type == "int")
                                    {
                                        sw.WriteLine(pp.type + " " + pp.name + "_temp=0;");
                                    }
                                    if (pp.type == "bool")
                                    {
                                        sw.WriteLine(pp.type + " " + pp.name + "_temp=false;");
                                    }
                                }
                            }
                        }
                }
                sw.Write("mtype={");
                bool init = true;
                foreach (Model.Type t in types)
                {
                    foreach (string value in t.Values)
                    {
                        if (init)
                        {
                            sw.Write(value);
                            init = false;
                        }
                        else
                            sw.Write("," + value);
                    }
                }
                foreach (Device de in devices)
                {
                    foreach (State st in de.States)
                    {
                        if (init)
                        {
                            sw.Write(de.Name + "_" + st.Name);
                            init = false;
                        }
                        else
                        sw.Write("," + de.Name + "_" + st.Name);
                    }
                }
                foreach (Device de in devices)
                {
                    if (de.APIs.Count > 0)
                        foreach (API tt in de.APIs)
                        {
                            sw.Write("," + tt.Name + "_env");
                            sw.Write("," + tt.Name + "_trigger");
                        }
                }
                sw.WriteLine("};");
                sw.WriteLine();
                foreach (Device de in devices)
                {
                    sw.WriteLine("active proctype " + de.Name + "()");
                    sw.WriteLine("{");
                    sw.WriteLine("beginning:");
                    foreach (State st in de.States)
                    {
                        sw.WriteLine(st.Name + ":");
                        sw.WriteLine("atomic{");
                        sw.WriteLine(de.Name + "_state=" + de.Name + "_" + st.Name + ";");
                        IList<Model.Action> actions = de.getAction(st.Name);
                        if (actions.Count > 1)
                        {
                            sw.WriteLine("if");
                            foreach (Model.Action ac in actions)
                            {
                                sw.Write("::");
                                if (ac.Condition != "")
                                {

                                    if (!ac.Condition.Contains("?"))
                                        sw.WriteLine(ac.Condition + "->atomic{");
                                    else
                                    {
                                        int dis = ac.Condition.IndexOf(")") - ac.Condition.IndexOf("(");
                                        if(dis>1)
                                        sw.WriteLine(ac.Condition.Replace("?","?1") + "->atomic{");
                                        else
                                            sw.WriteLine(ac.Condition.Replace("?()", "?1") + "->atomic{");
                                        //string api = ac.Condition.Replace("?", "");
                                        //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count > 0)
                                        //{
                                        //    sw.WriteLine(ac.Condition.Replace("?", "_Para") + " temp_" + ac.Condition.Replace("?", ""));
                                        //    sw.WriteLine(ac.Condition + "temp_" + ac.Condition.Replace("?", "") + "->atomic{");
                                        //}
                                        //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count <= 0)
                                        //    sw.WriteLine(ac.Condition + "1" + "->atomic{");
                                    }

                                }
                                if (ac.Operation.Count > 0)
                                {
                                    foreach (string operation in ac.Operation)
                                        if (operation.Contains("[") && operation.Contains("]"))
                                        {
                                            string name = operation.Substring(0, operation.IndexOf("=")).Trim();
                                            string high = operation.Substring(operation.IndexOf("[") + 1, operation.IndexOf("]") - operation.IndexOf("[") - 1);
                                            high = high.Substring(high.IndexOf(",") + 1);
                                            sw.WriteLine("do");
                                            sw.WriteLine("::" + name + "<" + high + "->" + name + "++");
                                            sw.WriteLine("::break");
                                            sw.WriteLine("od");
                                        }
                                        else
                                        {
                                            string name = operation.Substring(0, operation.IndexOf("=")).Trim();
                                            if (operation.Contains("!"))
                                            {
                                                int dis = operation.IndexOf(")") - operation.IndexOf("(");
                                                if (dis > 1)
                                                    sw.WriteLine(operation.Replace("!", "!1") + "->;");
                                                else
                                                    sw.WriteLine(operation.Replace("!()", "!1") + ";");
                                                //string api = ac.Condition.Replace("!", "");
                                                //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count > 0)
                                                //{
                                                //    // sw.WriteLine(ac.Condition.Replace("?", "_Para") + " temp_" + ac.Condition.Replace("?", ""));
                                                //    sw.WriteLine(ac.Condition.Replace("!", "_Para") + " temp_" + ac.Condition.Replace("!", ""));
                                                //    sw.WriteLine(ac.Condition + "temp_" + ac.Condition.Replace("!", "") + ";");
                                                //}
                                                //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count <= 0)
                                                //    sw.WriteLine(ac.Condition + "1" + ";");
                                                //Variable temp = getVariable(name, variables);
                                                //if (temp.Type == "int" || temp.Type == "bool")
                                                //    sw.WriteLine(ac.Operation + ";");
                                                //else
                                                //    sw.WriteLine(name + "!" + ac.Operation.Substring(ac.Operation.IndexOf("=") + 1) + ";");
                                            }
                                            else
                                                sw.WriteLine(operation+";");
                                        }
                                }
                                sw.WriteLine("goto " + actions[0].To_state + ";");
                                if (ac.Condition != "")
                                {
                                    sw.WriteLine("}");
                                }
                            }
                            sw.WriteLine("fi");
                        }
                        else
                        {
                            Model.Action ac = actions[0];
                            if (ac.Condition != "")
                            {

                                if (!ac.Condition.Contains("?"))
                                    sw.WriteLine(ac.Condition + "->atomic{");
                                else
                                {
                                    int dis = ac.Condition.IndexOf(")") - ac.Condition.IndexOf("(");
                                    if (dis > 1)
                                        sw.WriteLine(ac.Condition.Replace("?", "?1") + "->atomic{");
                                    else
                                        sw.WriteLine(ac.Condition.Replace("?()", "?1") + "->atomic{");
                                    //string api=ac.Condition.Replace("?", "");
                                    //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count > 0)
                                    //{
                                    //    sw.WriteLine(ac.Condition.Replace("?", "_Para") + " temp_" + ac.Condition.Replace("?", ""));
                                    //    sw.WriteLine(ac.Condition + "temp_" + ac.Condition.Replace("?", "") + "->atomic{");
                                    //}
                                    //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count <= 0)
                                    //    sw.WriteLine(ac.Condition + "1" + "->atomic{");
                                }
                              
                            }
                            if (ac.Operation.Count>0)
                            {
                                foreach(string operation in ac.Operation)
                                    if (operation.Contains("[") && operation.Contains("]"))
                                {
                                    string name = operation.Substring(0, operation.IndexOf("=")).Trim();
                                    string high = operation.Substring(operation.IndexOf("[") + 1, operation.IndexOf("]") - operation.IndexOf("[") - 1);
                                    high = high.Substring(high.IndexOf(",")+1);
                                    sw.WriteLine("do");
                                    sw.WriteLine("::" + name + "<" + high + "->" + name + "++");
                                    sw.WriteLine("::break");
                                    sw.WriteLine("od");
                                }
                                else
                                {
                                    string name = operation.Substring(0, operation.IndexOf("=")).Trim();
                                    if (operation.Contains("!"))
                                    {
                                        int dis = operation.IndexOf(")") - operation.IndexOf("(");
                                        if (dis > 1)
                                            sw.WriteLine(operation.Replace("!", "!1") + ";");
                                        else
                                            sw.WriteLine(operation.Replace("!()", "!1") + ";");
                                        //string api = ac.Condition.Replace("!", "");
                                        //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count > 0)
                                        //{
                                        //   // sw.WriteLine(ac.Condition.Replace("?", "_Para") + " temp_" + ac.Condition.Replace("?", ""));
                                        //    sw.WriteLine(ac.Condition.Replace("!", "_Para") + " temp_" + ac.Condition.Replace("!", ""));
                                        //    sw.WriteLine(ac.Condition + "temp_" + ac.Condition.Replace("!", "") + ";");
                                        //}
                                        //if (getAPI(api, devices) != null && getAPI(api, devices).Para.Count <= 0)
                                        //    sw.WriteLine(ac.Condition + "1" + ";");
                                        //Variable temp = getVariable(name, variables);
                                        //if (temp.Type == "int" || temp.Type == "bool")
                                        //    sw.WriteLine(ac.Operation + ";");
                                        //else
                                        //    sw.WriteLine(name + "!" + ac.Operation.Substring(ac.Operation.IndexOf("=") + 1) + ";");
                                    }
                                    else
                                        sw.WriteLine(operation +";");
                                }
                            }
                            sw.WriteLine("goto " + actions[0].To_state + ";");
                            if (ac.Condition != "")
                            {
                                sw.WriteLine("}");
                            }
                        }

                        sw.WriteLine("}");
                    }
                    sw.WriteLine("}");
                   
                    sw.WriteLine();
                }
                foreach (Rules rule in rules)
                {
                    string name = rule.Devide_name + "_" + rule.Trigger_name;
                    sw.WriteLine("active proctype " + name + "()");
                    sw.WriteLine("{");
                    sw.WriteLine("beginning:");
                    sw.WriteLine("S1:");
                    sw.WriteLine("atomic{");
                    Trigger temp = getTrigger(rule.Trigger_name, rule.Devide_name, devices);
                    sw.WriteLine(temp.When + "->atomic{");//goto S2;
                   
                    sw.WriteLine("goto S2;");
                    //sw.Writeline("}");
                    sw.WriteLine("}");
                    sw.WriteLine("}");
                    sw.WriteLine("S2:");
                     sw.WriteLine("atomic{");
                     foreach (string restore in temp.Restore)
                         sw.WriteLine(restore);
                     sw.WriteLine(rule.Api_name + "_auth=" + rule.Api_name+"_trigger");
                             if (rule.Para=="()")
                                 sw.WriteLine(rule.Api_name + "!1;");
                             else
                                 sw.WriteLine(rule.Api_name+"!1"+rule.Para + ";");
            
                             sw.WriteLine("goto S1;");

                     
                         sw.WriteLine("}");
                    sw.WriteLine("}");
                    sw.WriteLine();
                }
                int count = 0;
                foreach (Device de in devices)
                {
                    foreach (API api in de.APIs)
                    {
                        if (api.Ur)
                        {
                            count++;
                        }
                    }
                }
                if (count > 0)
                {
                    sw.WriteLine("active proctype env()");
                    sw.WriteLine("{");
                    sw.WriteLine("S1:");
                    sw.WriteLine("atomic{");
                    sw.WriteLine("if");
                    foreach (Device de in devices)
                    {
                        foreach (API api in de.APIs)
                        {
                            if (api.Ur)
                            {
                                sw.WriteLine("::atomic{");
                                sw.WriteLine(api.Name + "_auth=" + api.Name + "_env");
                                if (getAPI(api.Name, devices) != null && getAPI(api.Name, devices).Para.Count > 0)
                                {
                                    API api_temp = getAPI(api.Name, devices);
                                    string temp = "";
                                    foreach (Para pp in api_temp.Para)
                                    {
                                        if (pp.type == "int")
                                        {
                                            sw.WriteLine("do");
                                            sw.WriteLine("::" + pp.name + "_temp<100->" + pp.name + "_temp++");
                                            sw.WriteLine("::break");
                                            sw.WriteLine("od");
                                        }
                                        if (pp.type == "bool")
                                        {
                                            sw.WriteLine("if");
                                            sw.WriteLine("::" + pp.name + "_temp=true");
                                            sw.WriteLine("::" + pp.name + "_temp=false");
                                            sw.WriteLine("ofi");
                                        }
                                        temp += ",";
                                        temp += pp.name + "_temp";
                                    }
                                    // sw.WriteLine(ac.Condition.Replace("?", "_Para") + " temp_" + ac.Condition.Replace("?", ""));
                                    sw.WriteLine(api.Name + "!1("+temp.Substring(1)+")" + ";");
                                    //sw.WriteLine(api.Name + "!1();");
                                }
                                if (getAPI(api.Name, devices) != null && getAPI(api.Name, devices).Para.Count <= 0)
                                    sw.WriteLine(api.Name + "!1" + ";");
                                sw.WriteLine("}");
                            }
                        }
                    }
                    sw.WriteLine("fi");
                    sw.WriteLine("goto S1;");
                    sw.WriteLine("}");
                    sw.WriteLine("}");
                }
                /*************************输出完成************************/
                sw.Flush();
                //关闭流  
                sw.Close();
                fs.Close();  
               // Console.Read();
            }
            catch(JsonReaderException e)
            {
                Console.Out.WriteLine("error input!\n"+e.StackTrace);
            }
        }

        static bool hasVariable(Variable v,IList<Variable> variables)
        {
            foreach (Variable var in variables)
            {
                if (var.Name == v.Name && var.Type == v.Type)
                {
                    return true;
                }
            }
            return false;
        }

        static Variable getVariable(string v, IList<Variable> variables)
        {
            foreach (Variable var in variables)
            {
                if (var.Name == v )
                {
                    return var;
                }
            }
            return null;
        }

        static API getAPI(string name, IList<Device> list)
        {
            foreach(Device de in list)
            foreach (API a in de.APIs)
                if (a.Name == name)
                    return a;
            return null;
        }
        static API getAPI(string api_name,string device_name, IList<Device> list)
        {
            foreach (Device de in list)
                if (de.Name == device_name)
                foreach (API a in de.APIs)
                    if (a.Name == api_name)
                        return a;
            return null;
        }
        static Trigger getTrigger(string tri_name, string device_name, IList<Device> list)
        {
            foreach (Device de in list)
                if (de.Name == device_name)
                    foreach (Trigger a in de.Triggers)
                        if (a.Name == tri_name)
                            return a;
            return null;
        }

        static bool exVariable(Variable v,IList<Device> list,Device d)
        {
            foreach (Device de in list)
            {
                if (de.Name == d.Name) continue;
                if (de.hasVariable(v))
                {
                    return true;
                }
            }
            return false;
        }

        static string readFile(string path)
        {
            StreamReader objReader = new StreamReader(path);
            string sLine = "";
            string result = "";
            while (sLine != null)
            {
                sLine = objReader.ReadLine();
                if (sLine != null && !sLine.Equals(""))
                    result += sLine;
            }
            objReader.Close();
            return result;
        }
    }
}
