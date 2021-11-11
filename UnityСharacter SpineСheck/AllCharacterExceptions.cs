using System;
using System.Collections.Generic;
using Game.Models.Enums;
using static Game.Models.Enums.ECharacterType;

namespace Editor.Characters
{
    public enum EExceptionType
    {
        Subfolder,
        Atlas,
        ForeshorteningSkin,
        RummagingSkin
    }

    public class CharacterExceptions
    {
        public ECharacterType CharacterType { get; private set; }
        
        public CharacterExceptions(ECharacterType characterType)
        {
            CharacterType = characterType;
        }
        
        public readonly List<string> subfolderExceptions = new List<string>();
        public readonly List<string> atlasesExceptions = new List<string>();
        public readonly List<string> foreshorteningSkinExceptions = new List<string>();
        public readonly List<string> rummagingSkinExceptions = new List<string>();
        
        public bool Exist(string exception, EExceptionType type)
        {
            bool result = false;
            
            switch (type)
            {
                case EExceptionType.Subfolder:
                    result = Exist(exception, subfolderExceptions);
                    break;
                case EExceptionType.Atlas:
                    result = Exist(exception, atlasesExceptions);
                    break;
                case EExceptionType.ForeshorteningSkin:
                    result = Exist(exception, foreshorteningSkinExceptions);
                    break;
                case EExceptionType.RummagingSkin:
                    result = Exist(exception, rummagingSkinExceptions);
                    break;
            }
            return result;
        }
        
        private bool Exist(string checkedEx, List<string> exceptions)
        {
            if (exceptions == null) return false;
            
            foreach (var exception in exceptions)
                if (checkedEx.Contains(exception)) return true;
            
            return false;
        }
    }
    
    public class AllCharacterExceptions
    {
        private readonly List<CharacterExceptions> _exceptions;
        private readonly CharacterExceptions _bishop = new CharacterExceptions(Bishop); 
        private readonly CharacterExceptions _prometheus = new CharacterExceptions(Prometheus);
        private readonly CharacterExceptions _sakharov = new CharacterExceptions(Sakharov);
        private readonly CharacterExceptions _warVictim = new CharacterExceptions(WarVictim);
        private readonly CharacterExceptions _bigDog = new CharacterExceptions(BigDog);
        private readonly CharacterExceptions _twins = new CharacterExceptions(Twins);
        private readonly CharacterExceptions _corpse = new CharacterExceptions(Corpse);
        private readonly CharacterExceptions _lob = new CharacterExceptions(Lob);
        private readonly CharacterExceptions _sergeant = new CharacterExceptions(Sergeant);
        private readonly CharacterExceptions _auxoustic = new CharacterExceptions(Auxoustic);
        private readonly CharacterExceptions _juna = new CharacterExceptions(Juna);
        private readonly CharacterExceptions _departed = new CharacterExceptions(Departed);
        private readonly CharacterExceptions _parasite = new CharacterExceptions(Parasite);
        private readonly CharacterExceptions _opossum = new CharacterExceptions(Opossum);
        private readonly CharacterExceptions _pangolin = new CharacterExceptions(Pangolin);
        private readonly CharacterExceptions _deadWoundead = new CharacterExceptions(DeadWoundead);
        private readonly CharacterExceptions _amundsen = new CharacterExceptions(Amundsen);
        private readonly CharacterExceptions _tokamak = new CharacterExceptions(Tokamak);
        private readonly CharacterExceptions _birdman = new CharacterExceptions(Birdman);
        private readonly CharacterExceptions _missingNo = new CharacterExceptions(MissingNo);
        private readonly CharacterExceptions _master = new CharacterExceptions(Master);
        
        public AllCharacterExceptions()
        {
            _sakharov.foreshorteningSkinExceptions.Add("Backpacks/Cargo/Cargo 4");
            
            _warVictim.foreshorteningSkinExceptions.Add("Backpacks");
            _warVictim.rummagingSkinExceptions.Add("Backpacks");
            
            _bigDog.subfolderExceptions.Add("Backpack_Rummaging");
            _bigDog.atlasesExceptions.Add("Backpacks");
            _bigDog.atlasesExceptions.Add("Objects");
            _bigDog.foreshorteningSkinExceptions.Add("Basic/Hand_L");
            _bigDog.foreshorteningSkinExceptions.Add("Basic/Hand_R");
            _bigDog.foreshorteningSkinExceptions.Add("Hand_Objects");
            _bigDog.foreshorteningSkinExceptions.Add("Backpacks");
            
            _corpse.subfolderExceptions.Add("Backpack_Rummaging");
            _corpse.foreshorteningSkinExceptions.Add("Basic/Hand_L");
            _corpse.foreshorteningSkinExceptions.Add("Basic/Hand_R");
            
            _exceptions = new List<CharacterExceptions>
            {
                _bishop,
                _prometheus,
                _sakharov,
                _warVictim,
                _bigDog,
                _twins,
                _corpse,
                _lob,
                _sergeant,
                _auxoustic,
                _juna,
                _departed,
                _parasite,
                _opossum,
                _pangolin,
                _deadWoundead,
                _amundsen,
                _tokamak,
                _birdman,
                _missingNo,
                _master
            };
        }
        
        public CharacterExceptions FindCharacterExceptions(string characterName) =>
            Enum.TryParse(characterName, out ECharacterType type) ? FindCharacterExceptions(type) : null;

        public CharacterExceptions FindCharacterExceptions(ECharacterType type) =>
            _exceptions.Find(exception => exception.CharacterType == type);
    }
}


