using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class UICharacterActions : ButtonList
{

    [Header("Colors")]
    public Color confirmationColor;
    public Color selectedColor;

    [Header("Other Settings")]
    public int maxTargets;
    public CharActions action;
    public bool waitingForConfirmation;
    public Skill skillToActivate { get; private set; } //in case of using the skill action, this skill will activate after targetting
    public Item itemToUse { get; private set; }

    public string actionString;
    public List<UIActionButton> actionButtons ;
    public UIActionButton confirmationButton;
    public Action confirmAction;

    public static UICharacterActions instance;

    #region Setters
    public void SetSkillToActivate(Skill skill)
    {
        skill.Activate(BattleManager.caster, BattleManager.caster.GetSkillFromSkillList(skill));
        skillToActivate = skill;
        Debug.Log("Set skill to activate to " + skill.name);
    }
    public void SetItemToActivate(Item item)
    {
        item.Activate(BattleManager.caster, BattleManager.caster.GetItemFromInventory(item));
        itemToUse = item;
        Debug.Log("Set item to activate to " + item.name);
    }
    #endregion

    private void Start()
    {
        instance = this;
    }

    public void AddAction(CharActions action)
    {
        GameObject newEntry = AddObject();
        UIActionButton component = newEntry.GetComponent<UIActionButton>();
        component.LoadAction(action);
        actionButtons.Add(component);

        if(CharActions.EndTurn == action)
        {
            confirmationButton = component;
        }
    }
    public void AddAllActions()
    {
        ClearList();
        actionButtons = new List<UIActionButton>();

        for (int i = 0; i < RuleManager.CharActionsHelper.Length; i++)
        {
            AddAction(RuleManager.CharActionsHelper[i]);
        }
    }

    /// <summary>
    /// Returns action description
    /// </summary>
    /// <param name="action"></param>
    /// <returns></returns>
    public static string GetActionDescription(CharActions action)
    {
        string result = "Unknown Action";

        switch (action)
        {
            case CharActions.Attack:
                result = "Target an enemy, and deal damage based on the character's basic attack formula";
                break;
            case CharActions.Defend:
                result = "For this turn, you take 50% damage from all sources. Deactivates on your next turn, or when you get attacked once.";
                break;
            case CharActions.Skill:
                result = "Use a skill from your skill list";
                break;
            case CharActions.Item:
                result = "Utilize an item from your inventory";
                break;
            case CharActions.Rearrange:
                result = "Use this turn to rearrange yourself in the team order.";
                break;
            case CharActions.Prepare:
                result = "(NOT YET IMPLEMENTED) Prepare yourself to be attacked, gain advantage in dodge roll IF you get attacked this turn. However, if a turn passes without being attacked while Ready, you gain DISTRACTED";
                break;
            case CharActions.EndTurn:
                result = "Confirms your action. In case of no action, Skips your turn without doing anything. (You can cancel your action with right click)";
                break;
        }

        return result;
    }

    public void Act(ActionData data)
    {
        ActLocal(data);
    }

    public void ActLocal(ActionData data)
    {
        ActRPC(JsonUtility.ToJson(data));
    }
    
    /// <summary>
    /// Uses the action that was saved to actually run the code for the action.
    /// </summary>
    public void ActRPC(string serializedData)
    {
        ActionData data = JsonUtility.FromJson<ActionData>(serializedData);
        Character caster = data.GetCaster();
        List<Character> target = data.GetTargets();
        CharActions action = data.action;
        Skill skillToActivate = null;
        Item itemToUse = null;
        
        if (action == CharActions.Item)
        {
            itemToUse = (Item)data.item.GetReferenced();
        }
        
        if (action == CharActions.Skill)
        {
            skillToActivate = (Skill)data.skill.GetReferenced();
        }

        BattleManager.i.uiList.SetTargettingMode(false);

        switch (action)
        {
            case CharActions.Attack:
                {
                    for (int i = 0; i < target.Count; i++)
                    {
                        if (!target[i].dead)
                        {
                            if (target[i].targettable)
                            {
                                int basicAttackRaw = RPGFormula.ReadAndSumList(caster.basicAttack.formula, caster.stats);
                                BattleManager.i.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name}!");
                                target[i].GetsDamagedBy(basicAttackRaw, caster.basicAttack.dmgType, caster.basicAttack.element, caster);
                            }
                            else
                            {
                                BattleManager.i.battleLog.LogBattleEffect($"{caster.name} attacks {target[i].name}... but {target[i].name} was untargettable. Attack misses.");
                            }

                        }
                        else
                        {
                            BattleManager.i.battleLog.LogBattleEffect($"{caster.name} attacks the dead body of {target[i].name}... How rude.");
                        }
                    }
                    caster.Analytics_TriggerActionUsed(action);
                    TeamOrderManager.i.EndTurn();
                }
                break;

            case CharActions.Defend:
                {
                    caster.SetDefending(true);
                    caster.Analytics_TriggerActionUsed(action);
                    TeamOrderManager.i.EndTurn();
                }
                break;

            case CharActions.Skill:
                {
                    caster.Analytics_TriggerActionUsed(action);
                    skillToActivate.Action(caster, target, caster.GetSkillFromSkillList(skillToActivate));
                }
                break;

            case CharActions.Item:
                {
                    caster.Analytics_TriggerActionUsed(action);
                    itemToUse.Action(caster, target, caster.GetItemFromInventory(itemToUse));
                }
                break;

            case CharActions.Rearrange:
                {
                    if (target[0].dead)
                    {
                        BattleManager.i.battleLog.LogBattleEffect($"{caster.name} tries to swap places with the dead body of {target[0].name}... but it doesn't work (choose another target)");
                        ActionRequiresTarget(CharActions.Rearrange);
                    }
                    else
                    {
                        if(caster.team == target[0].team)
                        {
                            bool success = TeamOrderManager.i.spdSystem.Swap(caster, target[0]);

                            if (success)
                            {
                                BattleManager.i.battleLog.LogBattleEffect($"{caster.name} swaps places with {target[0].name} to delay their turn!");
                                BattleManager.i.battleLog.LogBattleEffect($"{target[0].name}'s turn is skipped due to being swapped!");
                            }
                            else
                            {
                                BattleManager.i.battleLog.LogBattleEffect($"Can't swap with {target[0].name} because they don't have a turn left in this round!");
                            }
                        }
                        else
                        {
                            BattleManager.i.battleLog.LogBattleEffect($"Can't swap with {target[0].name} because they aren't from the same team as {caster.name}.");
                        }

                        caster.Analytics_TriggerActionUsed(action);
                        TeamOrderManager.i.EndTurn();
                        BattleManager.i.teamOrderMenu.UpdateTeamOrder();
                    }
                }
                break;

            case CharActions.Prepare:
                {
                    BattleManager.i.battleLog.LogBattleEffect($"{caster.name} prepares for an attack!");
                    BattleManager.i.battleLog.LogBattleEffect($"Yet for time reasons, the programmer couldn't make it work yet...");
                    TeamOrderManager.i.EndTurn();
                }
                break;

            case CharActions.EndTurn:
                {
                    BattleManager.i.battleLog.LogBattleEffect($"{caster.name} skips their turn...");
                    caster.Analytics_TriggerActionUsed(action);
                    TeamOrderManager.i.EndTurn();
                }
                break;
            default:
                print("default switch");
                break;
        }

        BattleManager.i.ResetCheckedPassives();
    }

    /// <summary>
    /// The function a button uses to run its code.
    /// </summary>
    /// <param name="action"></param>
    public void ButtonAction(CharActions action)
    {
        Debug.Log("ButtonAction called, with action: " + action);
        Character caster = BattleManager.caster;

        VisualSelectButton(action);
        switch (action)
        {
            case CharActions.Attack:
                {
                    maxTargets = 1;
                    BattleManager.i.battleLog.LogBattleEffect($"{caster.name} attacks someone!");
                    ActionRequiresTarget(CharActions.Attack);
                }
                break;

            case CharActions.Defend:
                {
                    ActionDoesNotRequireTarget(CharActions.Defend);
                }
                break;

            case CharActions.Skill:
                {
                    if (caster.skillList.Count == 0)
                    {
                        BattleManager.i.battleLog.LogBattleEffect($"{caster.name} has no skills to activate...");
                    }
                    else
                    {
                        if (!TeamOrderManager.i.AIturn || SettingsManager.manualMode)
                        {
                            UISkillContext.instance.Show();
                            UIItemContext.instance.Hide();
                        }
                    }
                }
                break;

            case CharActions.Item:
                {
                    if (caster.inventory.Count == 0)
                    {
                        BattleManager.i.battleLog.LogBattleEffect($"{caster.name} has no items to use...");
                    }
                    else
                    {
                        UISkillContext.instance.Hide();
                        UIItemContext.instance.Show();
                    }
                }
                break;

            case CharActions.Rearrange:
                {
                    BattleManager.i.battleLog.LogBattleEffect($"{caster.name} chooses to swap with someone!");
                    maxTargets = 1;
                    ActionRequiresTarget(CharActions.Rearrange);
                }
                break;

            case CharActions.Prepare:
                {
                    ActionDoesNotRequireTarget(CharActions.Prepare);
                }
                break;

            case CharActions.EndTurn:
                {
                    if(confirmAction == null)
                    {
                        UIActionConfirmationPopUp.i.Show(delegate
                        {
                            SetAction(CharActions.EndTurn); 
                            Act(new ActionData(BattleManager.caster, BattleManager.target, action, skillToActivate, itemToUse));
                        }, true, "This will skip your turn, are you sure?");
                    }
                    else
                    {
                        ConfirmAction();
                    }
                }
                break;
        }
    }

    public void ActionRequiresTarget(CharActions action)
    {
        SetAction(action);

        TeamOrderManager.i.SetTurnState(TurnState.WaitingForTarget);
    }
    public void ActionDoesNotRequireTarget(CharActions action)
    {
        SetAction(action);
        TeamOrderManager.i.SetTurnState(TurnState.WaitingForConfirmation);
    }

    void SetAction(CharActions action)
    {
        this.action = action;
    }

    public void StartButtonActionConfirmation(Action confirmAction)
    {
        this.confirmAction = confirmAction;
        TeamOrderManager.i.turnState = TurnState.WaitingForConfirmation;
        waitingForConfirmation = true;
        if (SettingsManager.actionConfirmation)
        {
            InteractableButtons(false);
            Image image = confirmationButton.colorOverlay;
            Color newColor = confirmationColor;
            confirmationButton.ActivateColorOverlay(new Color(newColor.r, newColor.g, newColor.b, image.color.a));
            confirmationButton.GetComponent<Button>().interactable = true;
            BattleManager.i.uiList.InteractableUIs(false);
            BattleManager.i.uiList.SetTargettingMode(false);
            UISkillContext.instance.InteractableButtons(false);
            UIItemContext.instance.InteractableButtons(false);
            BattleManager.i.battleLog.LogImportant("Waiting for confirmation. (Press End Turn, or cancel with right click)");
        }
        else
        {
            ConfirmAction();
        }
    }

    public void ConfirmAction()
    {
        waitingForConfirmation = false;
        confirmationButton.DeactivateColorOverlay();
        confirmationButton.GetComponent<Button>().interactable = false;
        confirmAction?.Invoke();
        confirmAction = null;
    }

    public void DenyAction()
    {
        confirmAction = null;
        waitingForConfirmation = false;
        confirmationButton.DeactivateColorOverlay();
        DeactivateVisualSelect();
        TeamOrderManager.i.SetTurnState(TurnState.WaitingForAction);
        BattleManager.i.battleLog.LogBattleEffect("Cancelled Action.");
        //UIItemContext.instance.Hide();
        //UISkillContext.instance.Hide();

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

    public void VisualSelectButton(CharActions action)
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            UIActionButton current = actionButtons[i];
            if (current.action == action)
            {
                current.ActivateColorOverlay(selectedColor);
            }
            else
            {
                current.DeactivateColorOverlay();
            }
        }
    }

    public void DeactivateVisualSelect()
    {
        for (int i = 0; i < actionButtons.Count; i++)
        {
            UIActionButton current = actionButtons[i];
            
            current.DeactivateColorOverlay();
        }
    }
}
