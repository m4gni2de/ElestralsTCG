using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;

namespace Gameplay.Commands
{
    #region Enum
    public enum Result
    {
        Cancelled = -1,
        Pending = 0,
        Succeed = 1,
        Fail = 2,
        Fizzle = 3
    }
    #endregion


    public abstract class GameCommand
    {
       

        #region Properties
        protected abstract string DefaultKey { get; }
        private string _id = "";
        public string id { get { return _id; } set { _id = value; } }
        private string _key = "";
        public string key { get { return _key; } set { _key = value; } }
        private Result _result = Result.Pending;
        public Result result { get { return _result; } set { _result = value; } }

        private Player _player = null;
        public Player player { get { return _player; } protected set { _player = value; } }

        public CardAction commandAction { get; set; }
        #endregion

        #region Functions
        public abstract bool CanActivate { get; }
        #endregion

       
        protected void SetCommand(string key)
        {
            this.key = key;
            id = UniqueString.CreateId(4, key);
        }

        public abstract void CompleteCommand();
        public abstract void Refresh();
        public abstract void Do();
        public virtual void Resolve(Result res)
        {
            result = res;
        }
    }
}

