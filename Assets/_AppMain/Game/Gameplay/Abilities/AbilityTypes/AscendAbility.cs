using System.Collections;
using System.Collections.Generic;
using Databases;
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

        public AscendAbility(string name, string key) : base(name, key)
        {
            abiType = AbilityType.Ascend;
        }

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

        public override void TryAbility()
        {
            
        }
        public override void Do(GameCard source)
        {
            //if (Targets.Count == 1)
            //{
            //    GameCard toAscend = Targets[0];
            //    List<GameCard> spirits = new List<GameCard>();
            //    GameManager.Instance.Ascend(source.Owner, toAscend, source, spirits, source.Owner.deck.SpiritDeck.AtPosition(0), CardMode.Attack);
            //    return;

            //}
            //else if (Targets.Count > 1)
            //{

            //}
            GameCard toAscend = Targets[0];
            List<GameCard> spirits = new List<GameCard>();
            GameManager.Instance.Ascend(source.Owner, toAscend, source, spirits, source.Owner.deck.SpiritDeck.AtPosition(0), CardMode.Attack);
            GameManager.Instance.popupMenu.CloseMenu();

            return;

        }
        #endregion

    }
}

