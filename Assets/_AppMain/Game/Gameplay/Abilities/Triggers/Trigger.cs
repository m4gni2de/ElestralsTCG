using System;
using System.Collections;
using System.Collections.Generic;
using Defective.JSON;
using GameEvents;
using SimpleSQL.Demos;
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
        #region Properties
        public ActivationEvent whenActivate;
        public EventResult result;
        public EventTiming timing;
        public bool isLocal;
        public int index;
        public List<CardLocation> WatchFrom;


        private JSONObject _triggerArgs = null;
        public JSONObject triggerArgs { get { return _triggerArgs; } set { _triggerArgs = value; } }

        private bool _isWatching = false;
        public bool IsWatching { get { return _isWatching; } }  
        [SerializeField] private GameEvent _eventWatching = null;
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
        #endregion

        #region Property Parsing
        /// <summary>
        /// Trigger Arguments are stored in the following format: 'ArgumentKey.ArgumentProperty'. So 'GamePhase.phaseIndex' will search for the phaseIndex property of the argument titled 'GamePhase'
        /// </summary>
        /// <param name="argName"></param>
        /// <param name="breakChar"></param>
        /// <returns></returns>
        public (string, string) ParseKeyPropFromArgs(string argName)
        {
            string breakChar = ".";
            int breakIndex = argName.IndexOf(breakChar);

            string key = argName.Substring(0, breakIndex);
            string prop = argName.Substring(breakIndex + 1);

            return (key, prop);
        }
        #endregion

        #region Initialization
        public Trigger(bool local, int activation, int eventResult, int eventTiming, string watchFrom, string args)
        {
            isLocal = local;
            result = (EventResult)eventResult;
            timing = (EventTiming)eventTiming;
            WatchFrom = new List<CardLocation>();

            List<int> watchLocations = watchFrom.AsList(",").StringToInt();
            for (int i = 0; i < watchLocations.Count; i++)
            {
                WatchFrom.Add((CardLocation)watchLocations[i]);
            }

            if (!args.IsEmpty())
            {
                triggerArgs = new JSONObject(args);
            }
            else
            {
                triggerArgs = null;
            }
            
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

       
        public bool ValidateCardLocation(GameCard source)
        {
            if (source.CurrentSlot == null) { return false; }
            if (WatchFrom.Count == 0) { return true; }
            return WatchFrom.Contains(source.CurrentSlot.slotType); 
        
        }

        public object GetTriggerArgsValue(string key)
        {
            var obj = triggerArgs[key];
            return ParseArgsValue(obj);
        }
        protected object ParseArgsValue(JSONObject o)
        {


            if (o.type == JSONObject.Type.Number)
            {
                if (o.isInteger)
                {
                    return o.intValue;
                }
                return o.floatValue;

            }
            if (o.type == JSONObject.Type.String)
            {
                return o.stringValue;
            }
            return o.stringValue;
        }
        #endregion



    }



}

