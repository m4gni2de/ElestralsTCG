using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Databases
{
    public class TableInfo
    {
        public int cid
        {
            get;
            set;
        }

        public string name
        {
            get;
            set;
        }

        public string type
        {
            get;
            set;
        }

        public int notnull
        {
            get;
            set;
        }

        public string dflt_value
        {
            get;
            set;
        }

        public int pk
        {
            get;
            set;
        }

        public int unique
        {
            get;
            set;
        }
    }
}
