using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Gameplay.Commands
{
   


    public abstract class GameCommand
    {
        #region Enum
        public enum Result
        {
            Pending = 0,
            Succeed = 1,
            Fail = 2,
            Fizzle = 3
        }
        #endregion

        #region Properties
        protected abstract string DefaultKey { get; }
        private string _id = "";
        public string id { get { return _id; } set { _id = value; } }
        private string _key = "";
        public string key { get { return _key; } set { _key = value; } }
        private Result _result = Result.Pending;
        public Result result { get { return _result; } set { _result = value; } }
        #endregion


      
       protected void SetCommand(string key)
        {
            this.key = key;
            id = UniqueString.CreateId(4, key);
        }

        public virtual void Declare()
        {

        }
        public virtual void Do()
        {

        }
        public virtual void Resolve()
        {
           
        }
    }
}

