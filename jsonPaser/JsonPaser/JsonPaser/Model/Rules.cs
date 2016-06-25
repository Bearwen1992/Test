using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json.Linq;

namespace JsonPaser.Model
{
    public class Rules
    {
        private string devide_name;

        public string Devide_name
        {
            get { return devide_name; }
            set { devide_name = value; }
        }
        private string trigger_name;

        public string Trigger_name
        {
            get { return trigger_name; }
            set { trigger_name = value; }
        }
        private string api_name;

        public string Api_name
        {
            get { return api_name; }
            set { api_name = value; }
        }
        private string para;

        public string Para
        {
            get { return para; }
            set { para = value; }
        }
        public Rules(JObject jo)
        {
            devide_name = jo["Device"].ToString();
            trigger_name = jo["Trigger"].ToString();
            api_name = jo["API"].ToString();
            para = jo["Para"].ToString();
        }
    }
}
