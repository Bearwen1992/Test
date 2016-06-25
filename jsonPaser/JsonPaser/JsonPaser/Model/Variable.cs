using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public class Variable
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string type;

        public string Type
        {
            get { return type; }
            set { type = value; }
        }
        private string de_fault;

        public string De_fault
        {
            get { return de_fault; }
            set { de_fault = value; }
        }

        public Variable(JObject jo)
        {
            name = jo["Name"].ToString();
            type = jo["Type"].ToString();
            de_fault = jo["Default"].ToString();
        }

    }
}
