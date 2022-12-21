using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GameEvents
{
    public class Watcher
    {
        #region Properties
        public Delegate Response;
        public bool isSilent { get; private set; }
        #endregion

        public Watcher(Delegate de, bool silent)
        {
            Response = de;
            isSilent = silent;
        }

        public void Invoke(params object[] args)
        {
            if (Response != null)
            {
                if (isSilent)
                {

                    Response.DynamicInvoke();
                }
                else
                {
                    Response.DynamicInvoke(args);
                }
            }
           
        }
        public void FLush()
        {
            Response = null;
        }
        public void ParseCall(GameEventArgs args)
        {

        }
    }
}
