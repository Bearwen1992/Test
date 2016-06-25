using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public struct Para
    {
        public string name;
        public string type;
    };
    public class API
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private IList<Para> para = new List<Para>();

        public IList<Para> Para
        {
            get { return para; }
            set { para = value; }
        }

        private bool ur;

        public bool Ur
        {
            get { return ur; }
            set { ur = value; }
        }


        public API(JObject jo)
        {
            name = jo["Name"].ToString();
            JArray ja = (JArray)jo["Para"];
            if (ja != null)
            {
                for (int i = 0; i < ja.Count; i++)
                {
                    Para temp = new Para();
                    JObject tt = (JObject)ja[i];
                    temp.name = tt["Name"].ToString();
                    temp.type = tt["Type"].ToString();
                    para.Add(temp);
                }
            }
            ur = jo["UR"].ToString() == "true";
        }
    }
}
