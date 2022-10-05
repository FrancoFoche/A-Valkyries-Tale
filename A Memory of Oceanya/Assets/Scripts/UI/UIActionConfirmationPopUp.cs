using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using TMPro;
using UnityEngine.SceneManagement;

public class UIActionConfirmationPopUp : MonoBehaviour
{
    private static UIActionConfirmationPopUp _instance;
    public static UIActionConfirmationPopUp i 
    { 
        get 
        { 
            if (_instance == null) 
            { 
                _instance = FindObjectOfType<UIActionConfirmationPopUp>(); 
            } 
            return _instance; 
        } 
        private set 
        { 
            _instance = value; 
        } 
    }

    public GameObject objToHide;
    public TextMeshProUGUI confirmationText;
    public TextMeshProUGUI yesText;
    public TextMeshProUGUI noText;
    private Action confirmAction;
    public bool waitingForConfirmation;

    private void Awake()
    {
        Hide();
    }

    public void Show(Action confirmAction, bool affectedByConfirmActionSetting, string detailText = "Are you sure you want to commit this action?", string yesText = "Yes", string noText = "No")
    {
        this.confirmAction = confirmAction;
        waitingForConfirmation = true;
        TeamOrderManager.i.turnState = TurnState.WaitingForConfirmation;
        confirmationText.text = detailText;
        this.yesText.text = yesText;
        this.noText.text = noText;

        if (affectedByConfirmActionSetting)
        {
            if (SettingsManager.actionConfirmation)
            {
                objToHide.SetActive(true);
            }
            else
            {
                Confirm();
            }
        }
        else
        {
            objToHide.SetActive(true);
        }
    }
    public void Hide()
    {
        objToHide.SetActive(false);
    }
    public void Confirm()
    {
        waitingForConfirmation = false;
        Hide();
        confirmAction();
    }
    public void Deny()
    {
        waitingForConfirmation = false;
        Hide();

        if(BattleManager.i != null)
        {
            if (!BattleManager.i.pauseMenu.paused)
            {
                BattleManager.i.battleLog.LogBattleEffect("Cancelled Action.");
                TeamOrderManager.i.SetTurnState(TurnState.WaitingForAction);

                Character character;

                if (SettingsManager.manualMode)
                {
                    character = BattleManager.caster;
                }
                else
                {
                    character = TeamOrderManager.i.currentTurn;
                }

                BattleManager.i.uiList.SelectCharacter(character);
            }
        }
    }
}

[System.Serializable]
public struct ActionData
{
    public CharacterPos caster;
    public List<CharacterPos> targets;
    public CharActions action;
    public ActivatablePos skill;
    public ActivatablePos item;
    
    public Character GetCaster()
    {
        return caster.GetReferencedCharacter();
    }
    public List<Character> GetTargets()
    {
        List<Character> targetsResult = new List<Character>();
        
        foreach (var target in targets)
        {
            targetsResult.Add(target.GetReferencedCharacter());
        }

        return targetsResult;
    }

    public ActionData(Character caster, List<Character> targets, CharActions action, Skill skill, Item item)
    {
        this.caster = new CharacterPos(caster);
        this.targets = new List<CharacterPos>();
        this.action = action;

        foreach (var target in targets)
        {
            this.targets.Add(new CharacterPos(target));
        }

        this.item = new ActivatablePos();
        this.skill = new ActivatablePos();
        
        if(action == CharActions.Item)
        {
            this.item = new ActivatablePos(caster, ActivatablePos.ActivatableType.Item, item);
        }
        else if (action == CharActions.Skill)
        {
            this.skill = new ActivatablePos(caster, ActivatablePos.ActivatableType.Skill, skill);
        }
    }
    
    [System.Serializable]
    public struct CharacterPos
    {
        public Team team;
        public int position;

        public CharacterPos(Character character)
        {
            team = character.team;
            position = TeamOrderManager.i.GetPositionFromCharacter(character.team, character);
        }
        
        public Character GetReferencedCharacter()
        {
            return TeamOrderManager.i.GetCharacterFromPosition(team, position);
        }
    }

    [System.Serializable]
    public struct ActivatablePos
    {
        public CharacterPos character;
        public int position;
        public ActivatableType type;
        
        public enum ActivatableType
        {
            Skill, Item
        }

        public ActivatablePos(Character character, ActivatableType type, Activatables activatable)
        {
            this.character = new CharacterPos(character);
            this.type = type;

            switch (type)
            {
                case ActivatableType.Item:
                    position = character.GetItemPos((Item) activatable);
                    break;
                case ActivatableType.Skill:
                    position = character.GetSkillPos((Skill) activatable);
                    break;
                
                default:
                    throw new Exception("Couldn't find activatable type");
            }
        }

        public Activatables GetReferenced()
        {
            Character referencedCharacter = character.GetReferencedCharacter();

            switch (type)
            {
                case ActivatableType.Item:
                    return referencedCharacter.inventory[position].item;
                    
                case ActivatableType.Skill:
                    return referencedCharacter.skillList[position].skill;
            }

            throw new Exception("Couldn't get referenced activatable");
        }
    }
}
