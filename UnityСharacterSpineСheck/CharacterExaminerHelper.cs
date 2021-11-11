using System.Collections.Generic;
using Spine;
using Spine.Unity;

namespace Editor.Characters
{
    public class CharacterExaminerHelper
    {
        public List<SkinRootCategory> categories;

        public static readonly List<string> foreshorteningNames = new List<string>()
        {
            "01_Down",
            "02_DownRight",
            "03_Right",
            "04_TopRight",
            "05_Top",
            "06_TopLeft",
            "07_Left",
            "08_DownLeft"
        };

        public readonly List<string> characterSubfolerNames = new List<string>();
        
        private static readonly string rummagingName = "Backpack_Rummaging";
        private List<string> _missingSkins;
        private BasicSkins _basic;
        private HandObjectsSkins _handObjects;
        private BackpackSkins _backpacks;
        private BackpackRummagingSkins _backpackRummaging;
        private ExposedList<Skin> _skins;
        private CharacterExceptions _exceptions;
        
        public CharacterExaminerHelper()
        {
            _basic = new BasicSkins();
            _handObjects = new HandObjectsSkins();
            _backpacks = new BackpackSkins();
            _backpackRummaging = new BackpackRummagingSkins();
            
            categories = new List<SkinRootCategory>
            {
                _basic,
                _handObjects,
                _backpacks
            };
            
            characterSubfolerNames.AddRange(foreshorteningNames);
            characterSubfolerNames.Add(rummagingName);
        }
    
        public List<string> ConductSkinsExamination(SkeletonDataAsset skeleton, CharacterExceptions exceptions)
        {
            _missingSkins = new List<string>();
            var skeletonData = skeleton.GetSkeletonData(false);
            _skins = skeletonData.Skins;
            _exceptions = exceptions;
            
            if (skeleton.name.Contains("0"))
            {
                foreach (var skin in categories)
                    _missingSkins.AddRange(skin.ConductExamination(_skins, _exceptions));
            }
            if (skeleton.name.Contains("Rummaging"))
            {
                _missingSkins.AddRange(_backpackRummaging.ConductExamination(_skins, _exceptions));
            }
            
            return _missingSkins;
        }

        private class BasicSkins : CategoryWithSkins
        { 
            public BasicSkins() : base("Basic")
            {
                _skinNames = new []
                {
                    "Body",
                    "Hand_L",
                    "Hand_R"    
                };
            }
        }

        private class HandObjectsSkins : CategoryWithCategories
        {
            private List<string> _twoHands = new List<string>{"Box"};
            private List<string> _handL = new List<string>();
            private List<string> _handR = new List<string>();
            
            private string [] _handLR =
            {
                "GreenOrbLight", 
                "RedOrbLight"
            };

            public HandObjectsSkins() : base("Hand_Objects")
            {
                _handL.AddRange(_handLR);
                _handR.AddRange(_handLR);
                
                var _twoHandsSkins = new ExaminedSkin("Two_Hands", _twoHands);
                var _handLSkins = new ExaminedSkin("Hand_L", _handL);
                var handRSkins = new ExaminedSkin("Hand_R", _handR);
                
                _categories = new []
                {
                    _twoHandsSkins,
                    _handLSkins,
                    handRSkins
                };    
            }
        }

        private class BackpackSkins : CategoryWithCategories
        {
            private ExaminedSkin _cargo = new ExaminedSkin("Cargo", new List<string>
            {
                "Cargo 1",
                "Cargo 2",
                "Cargo 3",
                "Cargo 4"
            });
            
            private ExaminedSkin _tourist = new ExaminedSkin("Tourist", new List<string>
            {
                "Tourist 1",
                "Tourist 2",
                "Tourist 3",
                "Tourist 4"
            });
            
            public BackpackSkins() : base("Backpacks")
            {
                _categories = new []
                {
                    _cargo,
                    _tourist
                };
            }
        }

        private class BackpackRummagingSkins : CategoryWithSkins
        {
            public BackpackRummagingSkins() : base("Backpacks")
            {
                _skinNames = new []
                {
                    "Cargo",
                    "Tourist"
                };
            }

            public override List<string> ConductExamination(ExposedList<Skin> examinedSkins, CharacterExceptions exceptions)
            {
                if (ExistExceptions(_rootPath, exceptions)) return new List<string>();
                return base.ConductExamination(examinedSkins, exceptions);
            }

            protected override bool ExistExceptions(string path, CharacterExceptions exceptions) =>
                exceptions != null && exceptions.Exist(path, EExceptionType.RummagingSkin);
        }

        public class ExaminedSkin
        {
            private readonly string categoryName; 
            private readonly List<string> skinNames;

            public ExaminedSkin(string categoryName, List<string>  skinNames)
            {
                this.categoryName = categoryName;
                this.skinNames = skinNames;
            }

            public List<string> ConductExamination
            (
                string rootPath,
                ExposedList<Skin> examinedSkins,
                CharacterExceptions exceptions
            )
            {
                var skinPath = "";
                List<string> missingSkins = new List<string>();
                
                foreach (var skinName in skinNames)
                {
                    skinPath = $"{rootPath}/{categoryName}/{skinName}";
                    if(ExistExceptions(skinPath, exceptions)) continue;
                    if(!examinedSkins.Exists(exSkin => skinPath == exSkin.Name)) missingSkins.Add(skinPath);
                }
                
                return missingSkins;
            }
            
            private bool ExistExceptions(string path, CharacterExceptions exceptions) =>
                exceptions != null && exceptions.Exist(path, EExceptionType.ForeshorteningSkin);
        }

        private class CategoryWithSkins : SkinRootCategory
        {
            protected string[] _skinNames;
            
            public override List<string> ConductExamination
            (
                ExposedList<Skin> examinedSkins,
                CharacterExceptions exceptions
            )
            {
                List<string> missingSkins = new List<string>();
                
                if (!SkinsContainsRoot(examinedSkins, _rootPath))
                {
                    missingSkins.Add(_rootPath);
                    return missingSkins;
                }
                
                foreach (var skin in _skinNames)
                {
                    var skinPath = $"{_rootPath}/{skin}";
                    if(ExistExceptions(skinPath, exceptions)) continue;
                    if (!examinedSkins.Exists(exSkin => skinPath == exSkin.Name)) missingSkins.Add($"{_rootPath}/{skin}");
                }

                return missingSkins;
            }
            
            protected CategoryWithSkins(string rootPath) : base(rootPath) {}
        }

        private class CategoryWithCategories : SkinRootCategory
        {
            protected ExaminedSkin[] _categories;

            protected CategoryWithCategories(string rootPath) : base(rootPath) {}
            
            public override List<string> ConductExamination
            (
                ExposedList<Skin> examinedSkins,
                CharacterExceptions exceptions
            )
            {
                List<string> missingSkins = new List<string>();
                
                if(ExistExceptions(_rootPath, exceptions)) return missingSkins;
                
                foreach (var category in _categories)
                    missingSkins.AddRange(category.ConductExamination(_rootPath, examinedSkins, exceptions));
                
                return missingSkins;
            }
        }
        
        public abstract class SkinRootCategory
        {
            protected readonly string _rootPath;

            protected SkinRootCategory(string rootPath)
            {
                _rootPath = rootPath;
            }

            public abstract List<string> ConductExamination
            (
                ExposedList<Skin> examinedSkins,
                CharacterExceptions exceptions
            );
            
            protected bool SkinsContainsRoot(ExposedList<Skin> examinedSkins, string rootPath)
            {
                foreach (var examinedSkin in examinedSkins)
                {
                    if (examinedSkin.Name.Contains(rootPath)) return true;
                }
                return false;
            }
            
            protected virtual bool ExistExceptions(string path, CharacterExceptions exceptions) =>
                exceptions != null && exceptions.Exist(path, EExceptionType.ForeshorteningSkin);
        }
    }
}

