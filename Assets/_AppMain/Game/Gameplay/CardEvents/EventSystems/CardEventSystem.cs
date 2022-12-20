using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameEvents;
using Gameplay.Turns;
using System.Reflection;
using UnityEditor.Rendering;

namespace Gameplay
{
    public class CardEventSystem : GameEventSystem
    {
        
        public CardEventSystem(bool doLocal) : base(doLocal)
        {

        }
       
        #region Game Events

        #region Cast Related
        protected GameEvent<GameCard, CardSlot> _cardCast = null;
        [EventIndex(1, true)] public GameEvent<GameCard, CardSlot> CardCast { get { _cardCast ??= GameEvent.Create<GameCard, CardSlot>("CardCast", "Source Card", "Slot To"); return _cardCast; } }
        protected GameEvent<GameCard, GameCard, GameCard> _ascension = null;
        [EventIndex(2, true)] public GameEvent<GameCard, GameCard, GameCard> Ascension { get { _ascension ??= GameEvent.Create<GameCard, GameCard, GameCard>("Ascension", "Ascended Elestral", "Sacrificed Elestral", "Catalyst Spirit"); return _ascension; } }
        #endregion

        protected GameEvent<GameCard> _effectDeclared = null;
        [EventIndex(3, true)] public GameEvent<GameCard> EffectDeclared { get { _effectDeclared ??= GameEvent.Create<GameCard>("EffectDeclared", "Source Card"); return _effectDeclared; } }

        #region Enchant Related
        protected GameEvent<GameCard, GameCard> _enchantRecieved = null;
        [EventIndex(40, true)] public GameEvent<GameCard, GameCard> EnchantRecieved { get { _enchantRecieved ??= GameEvent.Create<GameCard, GameCard>("EnchantRecieved", "Source Card", "Enchanting Spirit"); return _enchantRecieved; } }
        protected GameEvent<GameCard, GameCard> _disenchant = null;
        [EventIndex(41, true)] public GameEvent<GameCard, GameCard> Disenchant { get { _disenchant ??= GameEvent.Create<GameCard, GameCard>("Disenchant", "Source Card", "Disenchanted Spirit"); return _disenchant; } }
        #endregion

        #region Battle Related
        protected GameEvent<GameCard, CardSlot> _attackDeclared = null;
        [EventIndex(50, true)] public GameEvent<GameCard, CardSlot> AttackDeclared { get { _attackDeclared ??= GameEvent.Create<GameCard, CardSlot>("AttackDeclared", "Attacker", "Target Slot"); return _attackDeclared; } }

        protected GameEvent<GameCard, GameCard> _battleStart = null;
        [EventIndex(51, true)] public GameEvent<GameCard, GameCard> BattleStart { get { _battleStart ??= GameEvent.Create<GameCard, GameCard>("BattleStart", "Attacker", "Attack Target"); return _battleStart; } }
        protected GameEvent<GameCard, GameCard,AttackResult> _battleEnd = null;
        [EventIndex(52, true)] public GameEvent<GameCard, GameCard, AttackResult> BattleEnd { get { _battleEnd ??= GameEvent.Create<GameCard, GameCard, AttackResult>("BattleEnd", "Attacker", "Attack Target", "Result"); return _battleEnd; } }
        #endregion

        #region Turn Phase Related
        protected GameEvent<Turn,GamePhase> _phaseStart = null;
        [EventIndex(100, false)] public GameEvent<Turn,GamePhase> PhaseStart { get { _phaseStart ??= GameEvent.Create<Turn, GamePhase>("PhaseStart", "Turn","Phase Index"); return _phaseStart; } }
        protected GameEvent<Turn, GamePhase> _phaseEnd = null;
        [EventIndex(101, false)] public GameEvent<Turn, GamePhase> PhaseEnd { get { _phaseEnd ??= GameEvent.Create<Turn, GamePhase>("PhaseEnd", "Turn", "Phase Index"); return _phaseEnd; } }
        #endregion

        #endregion

    }
}

