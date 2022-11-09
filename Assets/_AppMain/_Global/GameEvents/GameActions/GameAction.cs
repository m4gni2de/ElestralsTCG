using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using Gameplay;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Rendering;
using UnityEngine.UIElements;

namespace GameActions    
{
    public class GameAction : iGameAction
    {
        #region Operators
        public static MultiAction operator +(GameAction a, GameAction b)
        {
            return MultiAction.Create(a, b);
        }
        #endregion

        #region Interface
        public string uniqueId { get; set; }

        public void SetAction(Delegate ac, params object[] args)
        {
            Action = ac;
            parameters = new object[args.Length];
            for (int i = 0; i < args.Length; i++)
            {
                parameters[i] = args[i];
            }
        }

        public void Invoke()
        {
            this.Action.DynamicInvoke(parameters);
        }

        public bool IsEqual(Delegate a)
        {
            if (a == Action) { return true; }
            if (a.Target == Action.Target && a.Method.Name.ToLower() == Action.Method.Name.ToLower()) { return true; }
            return false;
        }
        #endregion

        #region Properties
        public Delegate Action { get; set; }

        public object[] parameters { get; set; }
        #endregion


        #region Initialization
        private static GameAction CreatedAction(Delegate de, params object[] args)
        {
            return new GameAction(de, args);
        }
        public static GameAction Create(Action ac)
        {
            return CreatedAction(ac);
        }
        public static GameAction Create<T>(Action<T> ac, T para0)
        {
            return CreatedAction(ac, para0);
        }
        public static GameAction Create<T0, T1>(Action<T0, T1> ac, T0 para0, T1 para1)
        {
            return CreatedAction(ac, para0, para1);
        }
        public static GameAction Create<T0, T1, T2>(Action<T0, T1> ac, T0 para0, T1 para1, T2 para2)
        {
            return CreatedAction(ac, para0, para1, para2);
        }

        #endregion


        public GameAction(Delegate ac, params object[] args)
        {
            uniqueId = UniqueString.CreateId(4, "ga");
            SetAction(ac, args);
        }

       
        
    }

    public class MultiAction : iGameAction
    {
        #region Operators
        public static MultiAction operator +(MultiAction a, GameAction b)
        {
            a.AddAction(b);
            return a;
        }
        #endregion

        #region Interface
        public string uniqueId { get; set; }
        public void Invoke()
        {
            foreach (KeyValuePair<Delegate, object[]> item in actions)
            {
                Delegate de = item.Key;
                object[] args = item.Value;

                de.DynamicInvoke(args);
            }
        }
        public void SetAction(Delegate ac, params object[] args)
        {
            if (!actions.ContainsKey(ac))
            {
                actions.Add(ac, args);
            }

        }
        public bool IsEqual(Delegate a)
        {
            foreach (KeyValuePair<Delegate, object[]> item in actions)
            {
                Delegate de = item.Key;
                object[] args = item.Value;

                if (a == de) { return true; }
                if (a.Target == de.Target && a.Method.Name.ToLower() == de.Method.Name.ToLower()) { return true; }
            }
           
            return false;
        }
        #endregion

       

        private Dictionary<Delegate, object[]> actions = new Dictionary<Delegate, object[]>();

        public Delegate this[int index]
        {
            get
            {
                List<Delegate> list = new List<Delegate>();
                list.AddRange(actions.Keys);
                if (index < actions.Count) { return list[index]; }
                return null;
            }
        }

        public static MultiAction Create(params GameAction[] args)
        {
            if (args.Length < 2) { App.LogFatal("MultiAction needs at least 2 GameActions!"); }
            MultiAction m = new MultiAction(args);
            return m;
        }

        public MultiAction(GameAction[] args)
        {
            uniqueId = UniqueString.CreateId(4, "ga");
            for (int i = 0; i < args.Length; i++)
            {
                SetAction(args[i].Action, args[i].parameters);
            }
        }

        public void AddAction(GameAction ac)
        {
            SetAction(ac.Action, ac.parameters);
        }
        

        
    }
}

