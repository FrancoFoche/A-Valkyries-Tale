using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.TooltipUI;
using Kam.CustomInput;


public enum BattleState
{
    Start,
    InCombat,
    Won,
    Lost,
    End
}
public class Wave
{
    public List<Character> allySide;
    public List<Character> enemySide;
    public Music waveMusic = Music.None;
    public bool winMusicPlay = true;
    public bool lossMusicPlay = true;
    public int transitionOutTime = 5;
    public int EXPgiven = 0;
    public int GoldGiven = 0;
}

public class BattleManager : MonoBehaviour, IObserver
{
    private static BattleManager _instance;
    public static BattleManager i { get { if (_instance == null) { _instance = FindObjectOfType<BattleManager>(); } return _instance; } private set { _instance = value; } }
    public static Character caster { get; private set; }
    public static List<Character> target { get; private set; }

    public static LevelManager.BattleLevel currentLevel { get; private set; }
    public static int currentBattleIndex { get; private set; }

    public BattleState battleState { get; private set; }

    public bool inCombat { get; private set; }
    public bool enemyTeamDeath { get; private set; }
    public bool allyTeamDeath { get; private set; }

    [Header("Controls")]
    public ContextMenuControl[] contextMenuControls;
    public ActionMenuControl[] actionMenuControls;

    [System.Serializable]
    public struct ContextMenuControl
    {
        public string name;
        public KeyCode[] keys;
    }

    [System.Serializable]
    public struct ActionMenuControl
    {
        public string name;
        public KeyCode[] keys;
        public CharActions action;
    }

    [Header("References")]
    public BattleLog battleLog;
    public BattleUIList uiList;
    public UICharacterActions charActions;
    public PauseMenu pauseMenu;
    public UIMenu_TeamOrder teamOrderMenu;

    public GameObject easteregg;

    Utilities_Input_HoldKey hold1 = new Utilities_Input_HoldKey(false);
    private void Awake()
    {
        DataBaseOrder.i.Initialize();
    }

    private void Start()
    {
        #region One time only Initializations
        currentBattleIndex = 0;

        inCombat = false;

        SaveFile loaded = SavesManager.loadedFile;
        if (loaded != null)
        {
            teamOrderMenu.dropdownToggle.isOn = loaded.showOrderOfPlay;
            teamOrderMenu.showDead.isOn = loaded.orderOfPlay_showDead;
            teamOrderMenu.showPast.isOn = loaded.orderOfPlay_showPast;
        }

        pauseMenu.manualMode.isOn = SettingsManager.manualMode;
        pauseMenu.confirmActions.isOn = SettingsManager.actionConfirmation;
        #endregion
        
        DataBaseOrder.i.Initialize();
        StartCombatScene(LevelManager.GetLevel(LevelManager.currentLevel));
    }

    Action onUpdate = null;
    private void Update()
    {
        onUpdate?.Invoke();
    }

    #region Controls
    Action onPlaying = null;
    //This boolean is here to prevent other hotkeys from activating this frame after the confirmation.
    bool wasWaitingForConfirmation = false;
    public void Controls_General()
    {
        wasWaitingForConfirmation = false;

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (TeamOrderManager.i.turnState == TurnState.WaitingForConfirmation || TeamOrderManager.i.turnState == TurnState.WaitingForTarget)
            {
                if (UIActionConfirmationPopUp.i.waitingForConfirmation)
                {
                    UIActionConfirmationPopUp.i.Deny();
                }

                if (UICharacterActions.instance.waitingForConfirmation)
                {
                    UICharacterActions.instance.DenyAction();
                }
            }
            else
            {
                pauseMenu.TogglePauseMenu();
            }
        }

        if (hold1.HoldKey(KeyCode.Escape, 1.5f))
        {
            SceneLoaderManager.instance.Quit();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            if (UIActionConfirmationPopUp.i.waitingForConfirmation)
            {
                wasWaitingForConfirmation = true;
                UIActionConfirmationPopUp.i.Confirm();
            }
            else
            {
                if (!TeamOrderManager.i.AIturn)
                {
                    switch (TeamOrderManager.i.turnState)
                    {
                        case TurnState.WaitingForTarget:
                            Action temp = delegate
                            {
                                Debug.Log("Targetting done.");
                                TeamOrderManager.i.SetTurnState(TurnState.WaitingForConfirmation);
                            };

                            if (target.Count == 0)
                            {
                                UIActionConfirmationPopUp.i.Show(temp, true, "You have selected no targets, are you sure you want to confirm your action?");
                            }
                            else
                            {
                                temp();
                            }
                            break;
                    }
                }
            }
        }

        onPlaying?.Invoke();
    }

    public void Controls_Playing()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            teamOrderMenu.ToggleVisibility();
        }

        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.R))
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                UIActionConfirmationPopUp.i.Show(delegate { SavesManager.DeleteSave(); SceneLoaderManager.instance.ReloadScene(); }, false, "Are you ABSOLUTELY sure that you want to delete your save file?");
            }
            else
            {
                UIActionConfirmationPopUp.i.Show(() => SceneLoaderManager.instance.ReloadScene(), false, "Are you sure you want to restart the battle?");
            }
        }

        if (TeamOrderManager.i.turnState != TurnState.End && TeamOrderManager.i.turnState != TurnState.Start && TeamOrderManager.i.turnState != TurnState.NonDefined)
        {
            if (TeamOrderManager.i.turnState == TurnState.WaitingForTarget || TeamOrderManager.i.turnState == TurnState.WaitingForConfirmation)
            {
                if (TeamOrderManager.i.turnState == TurnState.WaitingForTarget)
                {
                    if (target.Count == UICharacterActions.instance.maxTargets)
                    {
                        Debug.Log("Targetting done.");
                        TeamOrderManager.i.SetTurnState(TurnState.WaitingForConfirmation);
                    }
                    else
                    {
                        if (caster.team == Team.Ally || SettingsManager.manualMode)
                        {
                            SetTargets(uiList.CheckTargets());
                        }
                    }
                }

                if (!TeamOrderManager.i.AIturn)
                {
                    if (Input.GetMouseButtonDown(1))
                    {
                        if (UIActionConfirmationPopUp.i.waitingForConfirmation)
                        {
                            UIActionConfirmationPopUp.i.Deny();
                        }

                        UICharacterActions.instance.DenyAction();
                    }
                }
            }
            else if (TeamOrderManager.i.turnState == TurnState.WaitingForAction)
            {
                if (!UIActionConfirmationPopUp.i.waitingForConfirmation)
                {
                    Controls_ActionHotkeys();
                }

                if (UISkillContext.instance.gameObject.activeInHierarchy || UIItemContext.instance.gameObject.activeInHierarchy)
                {
                    Controls_ContextMenu();
                }

                if (SettingsManager.manualMode)
                {
                    uiList.CheckCurrentSelection();
                    bool togglesOn = uiList.toggleGroup.AnyTogglesOn();
                    if (togglesOn && uiList.different)
                    {
                        uiList.UpdateSelected();
                        GetCaster();
                    }

                    #region Debug Features
                    if (Input.GetKeyDown(KeyCode.LeftControl) && caster != TeamOrderManager.i.currentTurn)
                    {
                        battleLog.LogBattleEffect("The GM decided to revert back to the turn that was supposed to take place. Smh.");
                        ReselectOriginalTurn();
                    }
                    #endregion
                }
                else
                {
                    if (BattleUIList.curCharacterSelected != TeamOrderManager.i.currentTurn && battleState == BattleState.InCombat)
                    {
                        Debug.LogWarning("The half-assed bugfix patch was triggered.");
                        uiList.SelectCharacter(TeamOrderManager.i.currentTurn);
                    }
                }
            }

            if (Input.GetKeyDown(KeyCode.Space) && !wasWaitingForConfirmation)
            {
                UICharacterActions.instance.ButtonAction(CharActions.EndTurn);
            }
        }
    }
    public void Controls_ActionHotkeys()
    {
        for (int i = 0; i < actionMenuControls.Length; i++)
        {
            ActionMenuControl current = actionMenuControls[i];

            if (current.keys.KeyPressed_Down())
            {
                UICharacterActions.instance.ButtonAction(current.action);
            }
        }
    }
    public void Controls_ContextMenu()
    {
        for (int i = 0; i < contextMenuControls.Length; i++)
        {
            ContextMenuControl current = contextMenuControls[i];

            if (current.keys.KeyPressed_Down())
            {
                EventManager.Instance.Trigger("Controls_ContextMenu", i+1);
            }
        }
    }
    #endregion

    public void SetBattleIndexToMax()
    {
        currentBattleIndex = currentLevel.waves.Length - 1;
    }

    public void StartCombatScene(LevelManager.BattleLevel level)
    {
        currentLevel = level;

        StartCombat(currentLevel.waves[0]);
    }
    public void StartCombat(Wave combat)
    {
        easteregg.gameObject.SetActive(false);

        enemyTeamDeath = false;
        allyTeamDeath = false;

        if (combat.waveMusic == Music.None)
        {
            MusicManager.PlayMusic(currentLevel.levelMusic);
        }
        else
        {
            MusicManager.PlayMusic(combat.waveMusic);
        }

        TeamOrderManager.i.BuildTeamOrder(combat);
        SetBattleState(BattleState.Start);
    }
    public void SetupBattle()
    {
        int index = 0;
        while (TeamOrderManager.i.spdSystem.teamOrder[index].dead)
        {
            index++;
        }
        caster = TeamOrderManager.i.spdSystem.teamOrder[index];
        TeamOrderManager.i.SetCurrentTurn(caster);
        TeamOrderManager.i.SetTurnState(TurnState.Start);
        SetBattleState(BattleState.InCombat);
    }

    public void SetBattleState(BattleState state)
    {
        SetBattleStateLocal(state);
    }
    
    public void SetBattleStateLocal(BattleState state)
    {
        if (battleState == state && battleState != BattleState.Start)
        {
            Debug.Log("Battle state is ALREADY set to " + state.ToString() + ".");
        }
        else
        {
            SpriteRenderer[] array = easteregg.GetComponentsInChildren<SpriteRenderer>();
            Wave currentWave = currentLevel.waves[currentBattleIndex];

            switch (state)
            {
                case BattleState.Start:
                    {
                        battleState = BattleState.Start;

                        caster = new Character();
                        target = new List<Character>();

                        charActions.AddAllActions();
                        uiList.SetSides(TeamOrderManager.i.allySide, TeamOrderManager.i.enemySide);


                        for (int i = 0; i < TeamOrderManager.i.totalCharList.Count; i++)
                        {
                            TeamOrderManager.i.totalCharList[i].CheckPassives();
                            TeamOrderManager.i.totalCharList[i].ActivatePassiveEffects(ActivationTime_General.StartOfBattle);
                        }
                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("COMBAT START!");

                        DelayAction(3, SetupBattle);
                    }
                    break;

                case BattleState.InCombat:
                    {
                        inCombat = true;
                        battleState = BattleState.InCombat;
                    }
                    break;

                case BattleState.Won:
                    {
                        battleState = BattleState.Won;
                        if (currentLevel.waves[currentBattleIndex].winMusicPlay)
                        {
                            MusicManager.PlayMusic(Music.Win);
                        }

                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("Ally team wins!");
                        easteregg.SetActive(true);
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].color = Color.green;
                        }
                        SetBattleState(BattleState.End);
                    }
                    break;

                case BattleState.Lost:
                    {
                        battleState = BattleState.Lost;
                        if (currentLevel.waves[currentBattleIndex].lossMusicPlay)
                        {
                            MusicManager.PlayMusic(Music.Lose);
                        }

                        charActions.InteractableButtons(false);
                        uiList.InteractableUIs(false);
                        battleLog.LogBattleStatus("Enemy team wins!");
                        easteregg.SetActive(true);
                        for (int i = 0; i < array.Length; i++)
                        {
                            array[i].color = Color.red;
                        }
                        SetBattleState(BattleState.End);
                    }
                    break;

                case BattleState.End:
                    {
                        UISkillContext.instance.Hide();
                        UIItemContext.instance.Hide();

                        bool lost = battleState == BattleState.Lost;
                        if (lost)
                        {
                            currentBattleIndex = 0;
                            battleLog.LogCountdown(5, "Restarting battle in _countdown_...", () => StartCombat(currentLevel.waves[0]));
                            return;
                        }

                        battleState = BattleState.End;

                        GiveAllyTeamEXP(currentWave.EXPgiven);

                        SettingsManager.groupGold += currentWave.GoldGiven;
                        battleLog.LogImportant("TEAM GAINS " + currentWave.GoldGiven + $"G! Current: {SettingsManager.groupGold}G");

                        DelayAction(3, delegate
                        {
                            currentBattleIndex++;
                            if (currentBattleIndex < currentLevel.waves.Length)
                            {
                                battleLog.LogCountdown(currentWave.transitionOutTime, "Next wave starts in _countdown_...", () => StartCombat(currentLevel.waves[currentBattleIndex]));
                            }
                            else
                            {
                                AnalyticsManager.SendEvent_LevelEnd(currentLevel.levelNumber, !lost, TeamOrderManager.i.allySide);
                                battleLog.LogCountdown(currentLevel.waves[currentLevel.waves.Length - 1].transitionOutTime, 
                                    $"Going to {currentLevel.winningScene.ToString()} in _countdown_...", 
                                    () => SceneLoaderManager.instance.LoadScene(currentLevel.winningScene));
                            }
                        });

                        if (SettingsManager.lastClearedLevel < LevelManager.currentLevel)
                        {
                            SettingsManager.lastClearedLevel = LevelManager.currentLevel;
                        }

                        SettingsManager.SaveSettings();
                    }
                    break;
            }
        }
    }
    public void TotalTeamKill_Check()
    {
        if (battleState != BattleState.End)
        {
            int AllyCount = TeamOrderManager.i.allySide.Count;
            int AllyDeathCount = 0;

            for (int i = 0; i < AllyCount; i++)
            {
                if (TeamOrderManager.i.allySide[i].dead)
                {
                    AllyDeathCount++;
                }
            }

            if (AllyDeathCount == AllyCount)
            {
                allyTeamDeath = true;
                inCombat = false;
                return;
            }


            int EnemyCount = TeamOrderManager.i.enemySide.Count;
            int EnemyDeathCount = 0;

            for (int i = 0; i < EnemyCount; i++)
            {
                if (TeamOrderManager.i.enemySide[i].dead)
                {
                    EnemyDeathCount++;
                }
            }

            if (EnemyDeathCount == EnemyCount)
            {
                enemyTeamDeath = true;
                inCombat = false;
                return;
            }
        }
    }
    public void TotalTeamKill_Act()
    {
        if (battleState != BattleState.End)
        {
            if (allyTeamDeath)
            {
                SetBattleState(BattleState.Lost);
                return;
            }

            if (enemyTeamDeath)
            {
                SetBattleState(BattleState.Won);
                return;
            }
        }
    }
    public void GetCaster()
    {
        if (TeamOrderManager.i.currentTurn != BattleUIList.curCharacterSelected && !TeamOrderManager.i.AIturn)
        {
            caster = BattleUIList.curCharacterSelected;
            battleLog.LogTurn(caster, 3);
            TeamOrderManager.i.SetTurnState(TurnState.Start);
        }
        else
        {
            caster = TeamOrderManager.i.currentTurn;
        }
    }
    public void SetTargets(List<Character> targets)
    {
        if (target.Count != targets.Count)
        {
            Debug.Log("Set targets, current count: " + targets.Count);
        }

        target = targets;

    }
    public void ClearTargets()
    {
        target = new List<Character>();
        Debug.Log("Cleared targets");
    }
    public void ResetCheckedPassives()
    {
        caster.SetCheckedPassives(false);

        for (int i = 0; i < target.Count; i++)
        {
            target[i].SetCheckedPassives(false);
        }
    }
    public void ReselectOriginalTurn()
    {
        uiList.SelectCharacter(TeamOrderManager.i.currentTurn);
        battleLog.LogTurn(TeamOrderManager.i.currentTurn, 2);
    }
    public void GiveAllyTeamEXP(int exp)
    {
        List<Character> characters = uiList.GetTeam(Team.Ally);

        for (int i = 0; i < characters.Count; i++)
        {
            if (!characters[i].dead)
            {
                characters[i].AddExp(exp);
            }
            else
            {
                battleLog.LogBattleEffect(characters[i].name + " was dead and could not receive EXP.");
            }
        }
    }
    public void UpdateTeamOrder()
    {
        TeamOrderManager.i.UpdateTeamOrder();
    }

    
    
    #region Utilities
    public void DelayAction(float secondsToDelay, Action delayedAction)
    {
        StartCoroutine(Delay(secondsToDelay, delayedAction));
    }

    IEnumerator Delay(float secondsToDelay, Action delayedAction)
    {
        yield return new WaitForSeconds(secondsToDelay);

        delayedAction.Invoke();
    }
    #endregion

    #region Observable
    void ActivateButtonInPosition_Skill(params object[] parameters)
    {
        int position = (int)parameters[0];
        UISkillContext.instance.ActivateButtonInPosition(position);
    }
    void ActivateButtonInPosition_Item(params object[] parameters)
    {
        int position = (int)parameters[0];
        UIItemContext.instance.ActivateButtonInPosition(position);
    }
    public void Notify(ObservableActionTypes action)
    {
        switch (action)
        {
            case ObservableActionTypes.ItemContextActivated:
                Debug.Log("Battle Observer: Added Item Controls");
                EventManager.Instance.Subscribe("Controls_ContextMenu", ActivateButtonInPosition_Item);
                break;
            case ObservableActionTypes.SkillContextActivated:
                Debug.Log("Battle Observer: Added Skill Controls");
                EventManager.Instance.Subscribe("Controls_ContextMenu", ActivateButtonInPosition_Skill);
                break;
            case ObservableActionTypes.ItemContextDeActivated:
                Debug.Log("Battle Observer: Removed Item Controls");
                EventManager.Instance.Unsubscribe("Controls_ContextMenu", ActivateButtonInPosition_Item);
                break;
            case ObservableActionTypes.SkillContextDeActivated:
                Debug.Log("Battle Observer: Removed Skill Controls");
                EventManager.Instance.Unsubscribe("Controls_ContextMenu", ActivateButtonInPosition_Skill);
                break;
            case ObservableActionTypes.ChatActivated:
                onUpdate = null;
                break;
            case ObservableActionTypes.ChatDeactivated:
                onUpdate = Controls_General;
                break;
            case ObservableActionTypes.Paused:
                onPlaying = null;
                break;
            case ObservableActionTypes.UnPaused:
                onPlaying = Controls_Playing;
                break;
        }
    }
    #endregion
}
