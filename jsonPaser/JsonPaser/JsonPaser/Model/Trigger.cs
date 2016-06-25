using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public class Trigger
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private string when;

        public string When
        {
            get { return when; }
            set { when = value; }
        }
        private IList<string> restore=new List<string>();

        public IList<string> Restore
        {
            get { return restore; }
            set { restore = value; }
        }

        public Trigger(JObject jo)
        {
            name = jo["Name"].ToString();
            when = jo["WHEN"].ToString();
            JArray ja = (JArray)jo["RESTORE"];
            for (int i = 0; i < ja.Count; i++)
            {
                restore.Add(ja[i].ToString());
            }
        }
    }
}
