using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public class Device
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
       
        private IList<Variable> variables=new List<Variable>();

        public IList<Variable> Variables
        {
            get { return variables; }
            set { variables = value; }
        }
        private IList<State> states=new List<State>();

        public IList<State> States
        {
            get { return states; }
            set { states = value; }
        }
        private IList<Trigger> triggers = new List<Trigger>();

        public IList<Trigger> Triggers
        {
            get { return triggers; }
            set { triggers = value; }
        }
        private IList<Action> actions=new List<Action>();

        public IList<Action> Actions
        {
            get { return actions; }
            set { actions = value; }
        }
        private IList<API> apis = new List<API>();

        public IList<API> APIs
        {
            get { return apis; }
            set { apis = value; }
        }

        public IList<Action> getAction(string state)
        {
            IList<Action> result = new List<Action>();
            foreach (Action ac in Actions)
            {
                if (ac.From_state == state)
                    result.Add(ac);
            }
            return result;
        }

        public bool hasVariable(Variable v)
        {
            bool result = false;
            foreach (Variable var in variables)
            {
                if (var.Name == v.Name && var.Type == v.Type)
                {
                    return true;
                }
            }
            return result;
        }

        public Device(JObject jo)
        {
            Name = jo["Name"].ToString();
            JArray jv = (JArray)jo["Vari"];
            if (jv != null)
             for (int i = 0; i < jv.Count; i++)
             {
                 JObject temp = (JObject)jv[i];
                 Variable tt = new Variable(temp);
                 variables.Add(tt);
             }
             JArray js = (JArray)jo["Mode"];
             if (js != null)
             {
                 string init_State = jo["INIMode"].ToString();

                 for (int i = 0; i < js.Count; i++)
                 {
                     string temp = js[i].ToString();
                     State tt = new State(temp, temp == init_State);
                     states.Add(tt);
                 }
             }
             JArray ja = (JArray)jo["Trans"];
             if (ja != null)
             for (int i = 0; i < ja.Count; i++)
             {
                 JObject temp = (JObject)ja[i];
                 Action tt = new Action(temp);
                 actions.Add(tt);
             }
             JArray jt = (JArray)jo["Trigger"];
             if (jt != null)
             for (int i = 0; i < jt.Count; i++)
             {
                 JObject temp = (JObject)jt[i];
                 Trigger tt = new Trigger(temp);
                 triggers.Add(tt);
             }
             JArray japi = (JArray)jo["API"];
             if (japi != null)
                 for (int i = 0; i < japi.Count; i++)
                 {
                     JObject temp = (JObject)japi[i];
                     API tt = new API(temp);
                     apis.Add(tt);
                 }
        }
    }
}
