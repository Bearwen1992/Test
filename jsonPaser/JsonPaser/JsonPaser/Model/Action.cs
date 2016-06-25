using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public struct Transtion
    {
        public string From{get;set;}
        public string To{get;set;}
    };
    public class Action
    {
        private string name;

public string Name
{
  get { return name; }
  set { name = value; }
}
        private string condition;

public string Condition
{
  get { return condition; }
  set { condition = value; }
}
private IList<string> operation = new List<string>();

public IList<string> Operation
{
  get { return operation; }
  set { operation = value; }
}
private string from_state;

public string From_state
{
    get { return from_state; }
    set { from_state = value; }
}
private string to_state;

public string To_state
{
    get { return to_state; }
    set { to_state = value; }
}
public Action(JObject jo)
{
    name = jo["Name"].ToString();
    condition = jo["WHEN"].ToString();
    JArray ja = (JArray)jo["ASSIGN"];
    for (int i = 0; i < ja.Count; i++)
    {
        operation.Add(ja[i].ToString());
    }
    //operation = jo["ASSIGN"].ToString();
   // JObject jt = (JObject)jo["Transition"];
    from_state = jo["From"].ToString();
    to_state = jo["To"].ToString();
}
    }
}
