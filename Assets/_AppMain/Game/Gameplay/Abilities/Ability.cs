using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gameplay.Abilities;
using Databases;

namespace Gameplay
{

    public abstract class Ability
    {
        #region Enum
        public enum AbilityType
        {
            None = 0,
            Ascend = 1,
            SpecialCast = 2
        }
        #endregion
        #region Properties
        public string Name { get; set; }
        protected string key { get; set; }
        protected AbilityType abiType { get; set; }

        protected MagicJson _args = null;
        protected MagicJson Args { get { _args ??= new MagicJson(UniqueString.CreateId(5, "arg")); return _args; }set  { _args = value; } }
        #endregion

        public Ability(string abiName, string abiKey)
        {
            Name = abiName;
            key = abiKey;

        }
        public abstract void LoadArgs(string args);
        public abstract bool CanActivate();
        public abstract void TryAbility();
        public abstract void Do(GameCard source);

        protected void AddKey(string key)
        {
            Args.AddKey(key);
        }

        #region Static Initialization
        public static Ability Get(AbilityDTO dto)
        {
            string name = dto.abiName;
            string key = dto.abiKey;
            AbilityType ty = (AbilityType)dto.abiType;

            Ability abi = null;
            switch (ty)
            {
                case AbilityType.None:
                    return null;
                case AbilityType.Ascend:
                    abi = new AscendAbility(name, key);
                    break;
                case AbilityType.SpecialCast:
                    break;
                default:
                    break;
            }

            abi.LoadArgs(dto.abiArgs);
            return abi;
        }
        #endregion
    }
}

