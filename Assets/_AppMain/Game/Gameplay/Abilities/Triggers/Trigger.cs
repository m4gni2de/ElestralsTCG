using System;
using System.Collections;
using System.Collections.Generic;
using GameEvents;
using UnityEngine;

namespace Gameplay.Abilities
{
    public enum ActivationEvent
    {
        Never = -3,
        YouCan =-2,
        Perpetual = -1,
        OnEvent = 0,
    }

    public enum EventResult
    {
        None = -1,
        Any = 0,
        Succeed = 1,
        Fail = 2,
        Fizzle = 3
    }

    public enum EventTiming
    {
        InterruptAll = -200,
        Interrupt = -100,
        Response = 0,
        RespondLast = 100
    }

    public class Trigger
    {
        public ActivationEvent whenActivate;
        public EventResult result;
        public EventTiming timing;
        public bool isLocal;
        public int index;

        private bool _isWatching = false;
        public bool IsWatching { get { return _isWatching; } }  
        private GameEvent _eventWatching = null;
        public GameEvent EventWatching
        {
            get
            {
                return _eventWatching;
            }
            set
            {
                if (value == _eventWatching) { return; }
                _eventWatching = value;

                if (value != null)
                {
                    _isWatching = true;
                }
                else
                {
                    _isWatching = false;
                }
                
            }
        }


        #region Initialization
        public Trigger(bool local, int activation, int eventResult, int eventTiming)
        {
            isLocal = local;
            result = (EventResult)eventResult;
            timing = (EventTiming)eventTiming;

            GetActivation(activation);
            
        }

        void GetActivation(int activation)
        {
            if (activation >= 0)
            {
                whenActivate = ActivationEvent.OnEvent;
                index = activation;
            }
            else
            {
                whenActivate = (ActivationEvent)activation;

                if (whenActivate == ActivationEvent.YouCan || whenActivate == ActivationEvent.Perpetual)
                {
                    index = 1;
                }
                else
                {
                    index = 0;
                }
            }

        }
        #endregion

    }



}

