using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class BattleLog : MonoBehaviour, IObservable
{
    public int maxMessages;

    public GameObject chatPanel, textObject;
    public InputField chatBox;

    public Color playerMessage;
    public Color battleStatus;
    public Color battleEffect;
    public Color allyTurn;
    public Color enemyTurn;
    public Color important; 

    [SerializeField]
    List<Message> messageList = new List<Message>();
    bool inputActive = false;
    private void Start()
    {
        AddToObserver(BattleManager.i);
        NotifyObserver(ObservableActionTypes.ChatDeactivated);
    }

    private void Update()
    {
        if (chatBox.isFocused && !inputActive)
        {
            inputActive = true;
            NotifyObserver(ObservableActionTypes.ChatActivated);
        }
        else if(!chatBox.isFocused && inputActive)
        {
            inputActive = false;
            NotifyObserver(ObservableActionTypes.ChatDeactivated);
        }

        if(chatBox.text != "")
        {
            
            if (Input.GetKeyDown(KeyCode.Return))
            {
                if(CommandCheck(chatBox.text) == false)
                {
                    SendMessageToChat("GameMaster: " + chatBox.text, Message.Type.PlayerMessage);
                }
                
                chatBox.text = "";
            }

        }
        else
        {
            if (!chatBox.isFocused && Input.GetKeyDown(KeyCode.Return))
            {
                chatBox.ActivateInputField();
            }
        }
    }

    /// <summary>
    /// Just a bunch of cheatcodes for easier debug.
    /// Returns true if the string is a command (and runs the code for it), returns false if it wasn't a command
    /// </summary>
    /// <returns></returns>
    public bool CommandCheck(string str)
    {
        switch (str.ToLower())
        {
            case "/giveup":
                LogImportant("Player tries to give up.");
                UIActionConfirmationPopUp.i.Show(delegate { BattleManager.i.SetBattleState(BattleState.Lost); }, false, "Are you sure you want to give up?");
                return true;

            case "/win":
                LogImportant("Player cheats and wins.");
                UIActionConfirmationPopUp.i.Show(delegate { BattleManager.i.SetBattleState(BattleState.Won); }, false, "Are you sure you want to skip this battle?");
                return true;

            case "/skiplevel":
                LogImportant("Player cheats and wins the entire level.");
                UIActionConfirmationPopUp.i.Show(delegate { BattleManager.i.SetBattleIndexToMax(); BattleManager.i.SetBattleState(BattleState.Won); }, false, "Are you sure you want to skip this battle?");
                return true;

            case "/togglemanual":
                SettingsManager.ToggleDebugMode();
                return true;

            case "/deletesave":
                UIActionConfirmationPopUp.i.Show(delegate { SavesManager.DeleteSave(); SceneLoaderManager.instance.ReloadScene(); }, false, "Are you ABSOLUTELY sure that you want to delete your save file?");
                return true;
        }

        return false;
    }
    public void SendMessageToChat(string text, Message.Type messageType)
    {
        if(messageList.Count >= maxMessages)
        {
            Destroy(messageList[0].textObject.gameObject);
            messageList.Remove(messageList[0]);
        }

        Message newMessage = new Message();

        Debug.Log(text);
        newMessage.text = text;

        GameObject newText = Instantiate(textObject, chatPanel.transform);

        newMessage.textObject = newText.GetComponent<TextMeshProUGUI>();

        newMessage.textObject.text = newMessage.text;
        newMessage.textObject.color = MessageTypeColor(messageType);

        switch (messageType)
        {
            case Message.Type.AllyTurn:
            case Message.Type.EnemyTurn:
            case Message.Type.BattleStatus:
                newMessage.textObject.alignment = TextAlignmentOptions.Center;
                break;

            case Message.Type.BattleEffect:
            case Message.Type.PlayerMessage:
            case Message.Type.Important:
                newMessage.textObject.alignment = TextAlignmentOptions.MidlineLeft;
                break;

        }

        messageList.Add(newMessage);
    }

    Color MessageTypeColor(Message.Type messageType)
    {
        Color color = battleEffect;

        switch (messageType)
        {
            case Message.Type.PlayerMessage:
                color = playerMessage;
                break;

            case Message.Type.AllyTurn:
                color = allyTurn;
                break;

            case Message.Type.EnemyTurn:
                color = enemyTurn;
                break;

            case Message.Type.Important:
                color = important;
                break;

            case Message.Type.BattleStatus:
                color = battleStatus;
                break;

            case Message.Type.BattleEffect:
                color = battleEffect;
                break;
        }

        return color;
    }

    public void LogBattleStatus(string text)
    {
        SendMessageToChat("*" + text + "*", Message.Type.BattleStatus);
    }

    public void LogBattleEffect(string text)
    {
        SendMessageToChat("BattleLog: " + text, Message.Type.BattleEffect);
    }

    public void LogImportant(string text)
    {
        SendMessageToChat("BattleLog: " + text, Message.Type.Important);
    }

    /// <summary>
    /// Sends a message to the log that says it is the character's turn
    /// </summary>
    /// <param name="character"></param>
    /// <param name="mode">1 = Normal turn. 2 = Repeated turn. ("Again") 3 = Non-Ordered </param>
    public void LogTurn(Character character, int mode = 1)
    {
        string messageToSend = "";
        Message.Type type = Message.Type.AllyTurn;

        switch (mode)
        {
            case 1:
                messageToSend = $"*{character.name}'s Turn*";
                break;
            case 2:
                messageToSend = $"*{character.name}'s Turn. Again*";
                break;
            case 3:
                messageToSend = $"*{character.name}'s (Non-Ordered) Turn.*";
                break;
            default:
                Debug.LogError("Invalid Turn Mode");
                break;
        }

        switch (character.team)
        {
            case Team.Enemy:
                type = Message.Type.EnemyTurn;
                break;
            case Team.Ally:
                type = Message.Type.AllyTurn;
                break;
        }

        SendMessageToChat(messageToSend, type);
    }

    /// <summary>
    /// Starts a countdown in the battleLog, logging a text every second. When the countdown ends, an event happens.
    /// </summary>
    /// <param name="countdownTime">Time you want the countdown to last</param>
    /// <param name="text">Text you want the battleLog to log every second (Use "_countdown_" in the text in the case you want it to log the current countdown.)</param>
    /// <param name="eventAfterCountdown">Action it will do after the countdown is over</param>
    /// <returns></returns>
    public void LogCountdown(int countdownTime, string text, Action eventAfterCountdown)
    {
        StartCoroutine(LogCountdownCoroutine(countdownTime, text, eventAfterCountdown));
    }


    /// <summary>
    /// Starts a countdown in the battleLog, logging a text every second. When the countdown ends, an event happens.
    /// </summary>
    /// <param name="countdownTime">Time you want the countdown to last</param>
    /// <param name="text">Text you want the battleLog to log every second (Use "_countdown_" in the text in the case you want it to log the current countdown.)</param>
    /// <param name="eventAfterCountdown">Action it will do after the countdown is over</param>
    /// <returns></returns>
    IEnumerator LogCountdownCoroutine(int countdownTime, string text, Action eventAfterCountdown)
    {
        for (int i = 0; i < countdownTime; i++)
        {
            int secondsLeft = countdownTime - i;

            string replacedText = text.Replace("_countdown_", secondsLeft.ToString());

            LogBattleEffect(replacedText);

            yield return new WaitForSeconds(1);
        }

        eventAfterCountdown.Invoke();
    }

    #region Observer
    List<IObserver> _obs = new List<IObserver>();
    public void AddToObserver(IObserver obs)
    {
        if (!_obs.Contains(obs))
        {
            _obs.Add(obs);
        }
    }

    public void RemoveFromObserver(IObserver obs)
    {
        if (_obs.Contains(obs))
        {
            _obs.Remove(obs);
        }
    }

    public void NotifyObserver(ObservableActionTypes action)
    {
        for (int i = 0; i < _obs.Count; i++)
        {
            _obs[i].Notify(action);
        }
    }
    #endregion
}

[System.Serializable]
public class Message
{
    public string text;
    public TextMeshProUGUI textObject;
    public Type messageType;

    public enum Type
    {
        PlayerMessage,
        AllyTurn,
        EnemyTurn,
        Important,
        BattleStatus,
        BattleEffect
    }
}