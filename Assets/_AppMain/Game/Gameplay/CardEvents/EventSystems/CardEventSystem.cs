using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Gameplay.Turns;
using System.Reflection;
using UnityEditor.Rendering;
using System;

namespace Gameplay
{
    public class CardEventSystem : GameEventSystem
    {
        #region Static Properties
        private static GameEvent _GameStateChanged = null;

        #region Global Events
        private static Dictionary<int, GameEvent> _globalEvents = null;
        public static Dictionary<int, GameEvent> GlobalEvents
        {
            get
            {
                _globalEvents ??= new Dictionary<int, GameEvent>();
                return _globalEvents;
            }
        }
       
        public static void RegisterGlobalEvents(CardEventSystem globalSystem)
        {
            GlobalEvents.Clear();

            Type t = typeof(CardEventSystem);
            foreach (var prop in t.GetProperties())
            {
                foreach (var att in prop.GetCustomAttributes(typeof(EventIndexAttribute), true))
                {
                    if (att is EventIndexAttribute)
                    {
                        int i = prop.GetCustomAttribute<EventIndexAttribute>().index;
                        if (!GlobalEvents.ContainsKey(i))
                        {
                            GameEvent ev = (GameEvent)prop.GetValue(globalSystem);
                            GlobalEvents.Add(i, ev);
                        }
                    }
                }
            }
        }

        public static GameEvent FindGlobal(int index)
        {
            if (GlobalEvents.ContainsKey(index))
            {
                return GlobalEvents[index];
            }
            return null;
        }
        #endregion
        #endregion
        [EventIndex(0, false)] public static GameEvent GameStateChanged { get { _GameStateChanged ??= GameEvent.Create("GameStateChanged"); return _GameStateChanged; } }

        public CardEventSystem(bool doLocal) : base(doLocal)
        {

        }


        #region Game Events

        #region Action Related
        private GameEvent<CardAction, GameCard> _actionDeclared = null;
        public GameEvent<CardAction, GameCard> ActionDeclared { get { _actionDeclared ??= GameEvent.Create<CardAction, GameCard>("CardActionDeclared", "Action", "SourceCard"); return _actionDeclared; } }

        #endregion

       
        #region Cast Related
        protected GameEvent<GameCard, CardSlot> _cardCast = null;
        [EventIndex(1, true)] public GameEvent<GameCard, CardSlot> CardCast { get { _cardCast ??= GameEvent.Create<GameCard, CardSlot>("CardCast", "SourceCard", "SlotTo"); return _cardCast; } }
        protected GameEvent<GameCard, GameCard, GameCard> _ascension = null;
        [EventIndex(2, true)] public GameEvent<GameCard, GameCard, GameCard> Ascension { get { _ascension ??= GameEvent.Create<GameCard, GameCard, GameCard>("Ascension", "AscendedElestral", "SacrificedElestral", "CatalystSpirit"); return _ascension; } }
        #endregion

        #region Effect Related(300-399)
        protected GameEvent<GameCard> _effectDeclared = null;
        [EventIndex(300, true)] public GameEvent<GameCard> EffectDeclared { get { _effectDeclared ??= GameEvent.Create<GameCard>("EffectDeclared", "SourceCard"); return _effectDeclared; } }

        protected GameEvent<GameCard, CardEffect> _effectActivated = null;
        [EventIndex(301, true)] public GameEvent<GameCard, CardEffect> EffectActivated { get { _effectActivated ??= GameEvent.Create<GameCard, CardEffect>("EffectActivated", "SourceCard", "Effect"); return _effectActivated; } }
        #endregion

        #region Enchant Related(400-499)
        protected GameEvent<GameCard, GameCard> _enchantRecieved = null;
        [EventIndex(400, true)] public GameEvent<GameCard, GameCard> EnchantRecieved { get { _enchantRecieved ??= GameEvent.Create<GameCard, GameCard>("EnchantRecieved", "SourceCard", "EnchantingSpirit"); return _enchantRecieved; } }
        protected GameEvent<GameCard, GameCard> _disenchant = null;
        [EventIndex(401, true)] public GameEvent<GameCard, GameCard> Disenchant { get { _disenchant ??= GameEvent.Create<GameCard, GameCard>("Disenchant", "SourceCard", "DisenchantedSpirit"); return _disenchant; } }
        #endregion

        #region Battle Related(500-599)
        protected GameEvent<GameCard, CardSlot> _attackDeclared = null;
        [EventIndex(500, true)] public GameEvent<GameCard, CardSlot> AttackDeclared { get { _attackDeclared ??= GameEvent.Create<GameCard, CardSlot>("AttackDeclared", "Attacker", "TargetSlot"); return _attackDeclared; } }

        protected GameEvent<GameCard, GameCard> _battleStart = null;
        [EventIndex(501, true)] public GameEvent<GameCard, GameCard> BattleStart { get { _battleStart ??= GameEvent.Create<GameCard, GameCard>("BattleStart", "Attacker", "AttackTarget"); return _battleStart; } }
        protected GameEvent<GameCard, GameCard, AttackResult> _battleEnd = null;
        [EventIndex(502, true)] public GameEvent<GameCard, GameCard, AttackResult> BattleEnd { get { _battleEnd ??= GameEvent.Create<GameCard, GameCard, AttackResult>("BattleEnd", "Attacker", "AttackTarget", "Result"); return _battleEnd; } }
        #endregion


        #region Turn Phase Related(100-199)
        protected GameEvent<Game> _gameStart = null;
        [EventIndex(100, false)] public GameEvent<Game> GameStart { get { _gameStart ??= GameEvent.Create<Game>("GameStart", "Game"); return _gameStart; } }

        protected GameEvent<Turn, GamePhase> _phaseStart = null;
        [EventIndex(101, false)] public GameEvent<Turn,GamePhase> PhaseStart { get { _phaseStart ??= GameEvent.Create<Turn, GamePhase>("PhaseStart","Turn","Phase"); return _phaseStart; } }
       
        protected GameEvent<Turn, GamePhase> _phaseEnd = null;
        [EventIndex(102, false)] public GameEvent<Turn, GamePhase> PhaseEnd { get { _phaseEnd ??= GameEvent.Create<Turn, GamePhase>("PhaseEnd", "Turn", "Phase"); return _phaseEnd; } }

        protected GameEvent<Turn, GamePhase> _drawPhaseStart = null;
        [EventIndex(103, false)] public GameEvent<Turn, GamePhase> DrawPhaseStart { get { _drawPhaseStart ??= GameEvent.Create<Turn, GamePhase>("DrawPhaseStart", "Turn", "Phase"); return _drawPhaseStart; } }
        protected GameEvent<Turn, GamePhase> _drawPhaseEnd = null;
        [EventIndex(104, false)] public GameEvent<Turn, GamePhase> DrawPhaseEnd { get { _drawPhaseEnd ??= GameEvent.Create<Turn, GamePhase>("DrawPhaseEnd", "Turn", "Phase"); return _drawPhaseEnd; } }

        protected GameEvent<Turn, GamePhase> _mainPhaseStart = null;
        [EventIndex(105, false)] public GameEvent<Turn, GamePhase> MainPhaseStart { get { _mainPhaseStart ??= GameEvent.Create<Turn, GamePhase>("MainPhaseStart", "Turn", "Phase"); return _mainPhaseStart; } }
        protected GameEvent<Turn, GamePhase> _mainPhaseEnd = null;
        [EventIndex(106, false)] public GameEvent<Turn, GamePhase> MainPhaseEnd { get { _mainPhaseEnd ??= GameEvent.Create<Turn, GamePhase>("MainPhaseEnd", "Turn", "Phase"); return _mainPhaseEnd; } }

        protected GameEvent<Turn, GamePhase> _battlePhaseStart = null;
        [EventIndex(107, false)] public GameEvent<Turn, GamePhase> BattlePhaseStart { get { _battlePhaseStart ??= GameEvent.Create<Turn, GamePhase>("BattlePhaseStart", "Turn", "Phase"); return _battlePhaseStart; } }
        protected GameEvent<Turn, GamePhase> _battlePhaseEnd = null;
        [EventIndex(108, false)] public GameEvent<Turn, GamePhase> BattlePhaseEnd { get { _battlePhaseEnd ??= GameEvent.Create<Turn, GamePhase>("BattlePhaseEnd", "Turn", "Phase"); return _battlePhaseEnd; } }

        protected GameEvent<Turn, GamePhase> _endPhaseStart = null;
        [EventIndex(109, false)] public GameEvent<Turn, GamePhase> EndPhaseStart { get { _endPhaseStart ??= GameEvent.Create<Turn, GamePhase>("EndPhaseStart", "Turn", "Phase"); return _endPhaseStart; } }
        protected GameEvent<Turn, GamePhase> _endPhaseEnd = null;
        [EventIndex(111, false)] public GameEvent<Turn, GamePhase> EndPhaseEnd { get { _endPhaseEnd ??= GameEvent.Create<Turn, GamePhase>("EndPhaseEnd", "Turn", "Phase"); return _endPhaseEnd; } }

        protected GameEvent<Game> _gameEnd = null;
        [EventIndex(199, false)] public GameEvent<Game> GameEnd { get { _gameEnd ??= GameEvent.Create<Game>("GameEnd", "Game"); return _gameEnd; } }
        #endregion

        #endregion



      

    }
}

