using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace GameEvents
{
    public class Watcher
    {
        #region Properties
        public Delegate Response;
        public bool isSilent { get; private set; }
        private bool argsOnly { get; set; }
        #endregion

        public Watcher(Delegate de, bool silent)
        {
            Response = de;
            isSilent = silent;
            argsOnly = false;
        }
        public Watcher(Action<GameEventArgs> de)
        {
            Response = de;
            isSilent = false;
            argsOnly = true;
        }

        public void Invoke(GameEventArgs eventArgs, params object[] args)
        {
            if (Response != null)
            {
                if (isSilent)
                {

                    Response.DynamicInvoke();
                }
                else
                {
                    if (argsOnly)
                    {
                        Response.DynamicInvoke(eventArgs);
                    }
                    else
                    {
                        Response.DynamicInvoke(args);
                    }
                   
                }
            }
           
        }
        public void FLush()
        {
            Response = null;
        }
        
    }
}
