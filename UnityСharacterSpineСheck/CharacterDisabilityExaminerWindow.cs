using System;
using System.IO;
using Game.Models.Enums;
using Spine.Unity;
using UnityEditor;
using UnityEditor.AnimatedValues;
using UnityEngine;

namespace Editor.Characters
{
    public class CharacterDisabilityExaminerWindow : EditorWindow
    {
        private enum EExaminationMode {All, SpecificCharacter}
        private EExaminationMode _examinationMode = EExaminationMode.All;
        private ECharacterType _characterType = ECharacterType.Bishop;
        private AnimBool _showCharacterSelection;
        private string _charactersFolder = "Assets/Art/Environment/Characters";
        private const string _darkOrange = "#FF8C00";
        private const string _purple = "#BA86FF";
        private bool _noErrorsFound;
        private SkeletonDataAsset _currentSkeleton;
        private CharacterExaminerHelper _examinerHelper;
        private AllCharacterExceptions _exceptions;
        private CharacterExceptions _currentCharacterEx;
        
        [MenuItem("Tools/Character/Disability Examiner")]
        private static void ShowWindow()
        {
            var window = GetWindow<CharacterDisabilityExaminerWindow>();
            window.titleContent = new GUIContent("Disability Examiner");
            window.Show();
        }

        private void OnEnable()
        {
            _showCharacterSelection = new AnimBool(true);
            _showCharacterSelection.valueChanged.AddListener(Repaint);
        }

        private void OnGUI()
        {
            _examinationMode = (EExaminationMode)EditorGUILayout.EnumPopup("Examination Mode", _examinationMode);
            _showCharacterSelection.target = _examinationMode != EExaminationMode.All;
            
            if (EditorGUILayout.BeginFadeGroup(_showCharacterSelection.faded))
            {
                _characterType = (ECharacterType) EditorGUILayout.EnumPopup("Examined Character", _characterType);
            }
            EditorGUILayout.EndFadeGroup();
            
            GUILayout.Space(30);
            if (GUILayout.Button("Conduct Examination")) ConductExamination();
        }

        private void ConductExamination()
        {
            _examinerHelper = new CharacterExaminerHelper();
            _exceptions = new AllCharacterExceptions();
            _noErrorsFound = true;

            if (Directory.Exists(_charactersFolder))
            {
                if (_examinationMode == EExaminationMode.All) ExamineAllCharacters();
                else ExamineAllForeshortening(true, Enum.GetName(typeof(ECharacterType), _characterType));
            }
            else
            {
                throw new Exception("Characters directory not found");
            }
            
            FinalMessage(_noErrorsFound);
        }

        private void ExamineAllCharacters()
        {
            foreach (var character in Enum.GetNames(typeof(ECharacterType)))
            {
                ExamineAllForeshortening(false, character);
            }
        }

        private void ExamineAllForeshortening(bool mustExist, string characterName)
        {
            var subfolderNames = _examinerHelper.characterSubfolerNames;
            _currentCharacterEx = _exceptions.FindCharacterExceptions(characterName);
            
            if (Directory.Exists($"{_charactersFolder}/{characterName}"))
            {
                foreach (var subfolderName in subfolderNames)
                {
                    if (!CheckSubfolderName(_charactersFolder, characterName, subfolderName)) continue;
                    if (!CheckSkeletonName($"{_charactersFolder}/{characterName}", subfolderName)) continue;
                    
                    CheckSkinNames(characterName);
                }
            }
            else
            {
                if (mustExist)
                {
                    Debug.Log($"<color={_darkOrange}>{characterName}</color>'s  character folder not found");
                    _noErrorsFound = false;
                }
            }
        }

        private bool CheckSubfolderName(string characterFolder, string characterName, string subfolderName)
        {
            bool checkPassed = true;

            if (_currentCharacterEx.Exist(subfolderName, EExceptionType.Subfolder))
            {
                checkPassed = false;
                if (Directory.Exists($"{characterFolder}/{characterName}/{subfolderName}"))
                {
                    Debug.Log($"<color={_darkOrange}>{characterName}</color> has a subdirectory " +
                              $"<color={_darkOrange}>{subfolderName}</color> which she shouldn't have");
                    _noErrorsFound = false;
                }
            }
            else
            {
                if (!Directory.Exists($"{characterFolder}/{characterName}/{subfolderName}"))
                {
                    Debug.Log($"<color={_darkOrange}>{characterName}</color>'s character subfolder " +
                              $"<color={_darkOrange}>{subfolderName}</color> not found.");
                    checkPassed = false;
                    _noErrorsFound = false;
                }
            }
            return checkPassed;
        }

        private bool CheckSkeletonName(string path, string subfolderName)
        {
            bool skeletonFound = true;
            var skeletonName = $"{subfolderName}_SkeletonData.asset";
            var skeleton = (SkeletonDataAsset)AssetDatabase.LoadAssetAtPath($"{path}/{subfolderName}/{skeletonName}", 
                typeof(SkeletonDataAsset));

            if (skeleton != null)
            {
                _currentSkeleton = skeleton;
            }
            else
            {
                Debug.Log($"The skeleton named <color={_darkOrange}> {skeletonName} </color> was not found.");
                skeletonFound = false;
                _noErrorsFound = false;
            }
            
            return skeletonFound;
        }

        private void CheckSkinNames(string characterName)
        {
            var skeletonName = _currentSkeleton.name;
            var missingSkins = _examinerHelper.ConductSkinsExamination(_currentSkeleton, _currentCharacterEx);
            
            if(missingSkins.Count > 0)
            {
                _noErrorsFound = false;
                ShowAllMissingSkins();
            }

            void ShowAllMissingSkins()
            {
                foreach (var missingSkin in missingSkins)
                {
                    Debug.Log($"The character <color={_darkOrange}>{characterName}</color> " +
                              $"the skeleton <color={_darkOrange}>{skeletonName}</color> " +
                              $"lacks the skin <color={_darkOrange}>{missingSkin}</color>");
                }
            }
        }

        private void FinalMessage(bool noErrorsFound)
        {
            if (noErrorsFound) Debug.Log($"The check was <color={_purple}> completed without errors. </color>");
        }
    } 
}

