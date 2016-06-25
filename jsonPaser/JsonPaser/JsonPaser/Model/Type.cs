using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public class Type
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private IList<string> values=new List<string>();

        public IList<string> Values
        {
            get { return values; }
            set { values = value; }
        }

        public Type(JObject jo)
        {
            Name = jo["Name"].ToString();
            JArray ja = (JArray)jo["Values"];
            for (int i = 0; i < ja.Count; i++)
            {
                values.Add(ja[i].ToString());
            }
        }

    }
}
