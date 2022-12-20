using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace GameEvents
{

    public class GameEvent
    {
       
       
       
        #region Static Initialization
        public static GameEvent Create(string eventKey)
        {
            return new GameEvent(eventKey);
        }
        public static GameEvent<T> Create<T>(string eventKey,string para1)
        {
            GameEvent<T> g = new GameEvent<T>(eventKey);
            g.AddParameters(new Parameter<T>(para1, 0));
            return g;
        }
        public static GameEvent<T1, T2> Create<T1, T2>(string eventKey, string para1, string para2)
        {
            GameEvent<T1, T2> g = new GameEvent<T1,T2>(eventKey);
            g.AddParameters(new Parameter<T1>(para1, 0), new Parameter<T2>(para2, 1));
            return g;
        }
        public static GameEvent<T1, T2, T3> Create<T1, T2, T3>(string eventKey, string para1, string para2, string para3)
        {
            GameEvent<T1, T2,T3> g = new GameEvent<T1, T2,T3>(eventKey);
            g.AddParameters(new Parameter<T1>(para1, 0), new Parameter<T2>(para2, 1), new Parameter<T3>(para3, 2));
            return g;
        }
        public static GameEvent<T1, T2, T3, T4> Create<T1, T2, T3, T4>(string eventKey, string para1, string para2, string para3, string para4)
        {
            GameEvent<T1, T2, T3,T4> g = new GameEvent<T1, T2, T3,T4>(eventKey);
            g.AddParameters(new Parameter<T1>(para1, 0), new Parameter<T2>(para2, 1), new Parameter<T3>(para3, 2), new Parameter<T3>(para4, 3));
            return g;
        }

        #endregion

        #region Properties
        public string key { get; set; }
        public string id { get; set; }
        protected List<iParameter> _parameters = null;
        public List<iParameter> Parameters { get { _parameters ??= new List<iParameter>(); return _parameters; } }

        private List<Watcher> _watchers = null;
        public List<Watcher> Watchers
        {
            get
            {
                _watchers ??= new List<Watcher>();
                return _watchers;
            }
        }

        #endregion

        #region Static Functions
        private static string GenerateId(string prefix = "")
        {
            if (string.IsNullOrWhiteSpace(prefix)) { prefix = "gv"; }
            string id = prefix;
            for (int i = 0; i < 7; i++)
            {
                int rand = UnityEngine.Random.Range(0, 10);
                id += rand.ToString();
            }
            return id;

           
            
        }
        #endregion

       

        #region Initialization
        public GameEvent(string eventKey)
        {
            key = eventKey;
            GameEventSystem.Register(key, this);
        }
        public virtual void AddParameters(params iParameter[] parameters)
        {
            for (int i = 0; i < parameters.Length; i++)
            {
                Parameters.Add(parameters[i]);
            }
        }
        #endregion

        #region Watchers
        public void SetWatcher(UnityAction ac, bool silent = false)
        {
            Watcher wa = new Watcher(ac, silent);
            Watchers.Add(wa);
        }

        #endregion


        #region Calling
        protected void Invoke(params object[] args)
        {
           if (args.Length != Parameters.Count) { throw new SystemException("Parameter count does not match."); }

            for (int i = 0; i < args.Length; i++)
            {
                Parameters[i].SetValue(args[i]);
            }

            for (int i = 0; i < Watchers.Count; i++)
            {
                Watcher w = Watchers[i];
                w.Invoke(args);
            }
        }
        #endregion



       
    }

    public class GameEvent<T> : GameEvent
    {
        public T param1 { get => (T)Parameters[0].GetValue(); }

        public GameEvent(string eventKey) : base(eventKey) { }
        public void Call(T obj)
        {
            Invoke(obj);
        }

    }

    public class GameEvent<T1, T2> : GameEvent
    {

        public T1 param1 { get => (T1)Parameters[0].GetValue(); }
        public T2 param2 { get => (T2)Parameters[1].GetValue(); }

        public GameEvent(string eventKey) : base(eventKey) { }
        public void Call(T1 a, T2 b)
        {
            Invoke(a, b);
        }

    }
    public class GameEvent<T1, T2, T3> : GameEvent
    {
        public T1 param1 { get => (T1)Parameters[0].GetValue(); }
        public T2 param2 { get => (T2)Parameters[1].GetValue(); }
        public T3 param3 { get => (T3)Parameters[2].GetValue(); }

        public GameEvent(string eventKey) : base(eventKey) { }
        public void Call(T1 a, T2 b, T3 c)
        {
            Invoke(a, b, c);
        }
    }
    public class GameEvent<T1, T2, T3, T4> : GameEvent
    {
        public T1 param1 { get => (T1)Parameters[0].GetValue(); }
        public T2 param2 { get => (T2)Parameters[1].GetValue(); }
        public T3 param3 { get => (T3)Parameters[2].GetValue(); }
        public T4 param4 { get => (T4)Parameters[4].GetValue(); }

        public GameEvent(string eventKey) : base(eventKey) { }
        public void Call(T1 a, T2 b, T3 c, T4 d)
        {
            Invoke(a, b, c, d);
        }

    }
}


