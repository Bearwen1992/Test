using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JsonPaser.Model
{
    public class State
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private bool init;

        public bool Init
        {
            get { return init; }
            set { init = value; }
        }
        public State(string n,bool i)
        {
            name = n;
            init = i;
        }
    }
}
