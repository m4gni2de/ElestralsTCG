using System.Collections;
using System.Collections.Generic;
using Databases;
using Gameplay.CardActions;
using Gameplay.Commands;
using UnityEngine;

namespace Gameplay.Abilities
{
    public class AscendAbility : Ability, iCardFind
    {
        #region Args
        public class AscendAbilityArgs
        {
            public int forceMode;
            public int cardMode;
            public string queryKey;

        }
        #endregion

        #region Properties
        private AscendAbilityArgs _abilityArgs = null;
        public AscendAbilityArgs AbilityArgs
        {
            get { return _abilityArgs; }
        }
        private CardFindQuery _ascendQuery = null;
        protected CardFindQuery AscendQuery { get { return _ascendQuery; } set { _ascendQuery = value; } }

        private List<GameCard> _targets = null;
        public List<GameCard> Targets { get { _targets ??= new List<GameCard>(); return _targets; } }
        #endregion

        #region Initialization
        public AscendAbility(string name, string key) : base(name, key)
        {
            abiType = AbilityType.Ascend;
        }
        #endregion

        #region Overrides
        public override void LoadArgs(string args)
        {
            _abilityArgs = JsonUtility.FromJson<AscendAbilityArgs>(args);
            if (AbilityArgs.queryKey.IsEmpty()) { AscendQuery = CardFindQuery.All; } else { AscendQuery = CardFindQuery.Lookup(AbilityArgs.queryKey); }
        }
        public override bool CanActivate()
        {
            Targets.Clear();
            _targets = CardFind.FindCards(AscendQuery);
            return Targets.Count > 0;
        }

        public override void TryAbility(GameCard source)
        {
            abilityActions.Clear();
            Ascend ascend = Ascend.FromTributedChosen(source.Owner, source, Targets);
            if (AbilityArgs.forceMode.IntToBool() == true)
            {
                ascend.SetForceMode(true, (CardMode)AbilityArgs.cardMode);
            }
            GameManager.ActiveGame.DoAscend(ascend);
            GameManager.Instance.popupMenu.CloseMenu();

            ascend.OnActionReady += AwaitAction;
            return;
        }
        private void AwaitAction(Ascend ascend, bool doAbility)
        {
            ascend.OnActionReady -= AwaitAction;
            if (doAbility)
            {
                AscendAction ac = ascend.commandAction as AscendAction;
                abilityActions.Add(ac);
                Do(true);
            }
        }
        
        #endregion

    }
}

