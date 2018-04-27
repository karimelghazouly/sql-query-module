using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace QueryModule.QueryParser
{
    enum Type
    {
        STRING, NUM
    }
    class Entity
    {   
        internal Type type;
        internal string valueS, name;
        internal double valueN;
        internal Entity(string Name, Type t, string s)
        {
            name = Name;
            type = t;
            valueS = s;
            valueN = 0;
        }
        internal Entity(string Name, Type t, double x)
        {
            name = Name;
            type = t;
            valueS = "";
            valueN = x;
        }
    }
}
