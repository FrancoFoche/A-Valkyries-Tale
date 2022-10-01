using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using UnityEngine.Audio;

public class Game_Manager : MonoBehaviour
{
    #region singletonSetup
    private static Game_Manager instance;
    public static Game_Manager i { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }

        dialogue = new LogHandler(this);
    }
    #endregion

    [System.Serializable]
    public struct Elements { 
        public CharacterAnimator characterImage;
        public CharacterAnimator characterImageHighRes;
        public RawImage backgroundImageHighRes;
        public RawImage backgroundImage;
        public RawImage deskImage;
        public TextMeshProUGUI shownameText;
        public TextMeshProUGUI messageText;
        public GameObject chatbox;
        public GameObject chatbox_NextArrow;
    }

    [Header("References")]
    public Elements elements;
    public AudioMixerGroup blipMixer;
    public AudioClip testClip;

    [Header("Settings")]
    public int scrollCharactersPerFrame = 1;
    [Range(1f, 60f)]
    public float scrollSpeed = 1f;
    [Tooltip("These strings will get removed from shownames when displayed, this is so that you can have different variation of same character with different states.")]
    public string[] shownameIgnoredCharacters;

    [Header("MANUAL START POINT")]
    public string logName;

    [Header("Keybinds")]
    public KeyCode[] continueKey;
    public KeyCode[] skipModeKey;

    [Header("State")]
    public Dialogue currentDialogue;

    public LogHandler dialogue; //created in awake

    string showname;
    [System.Serializable]
    public struct Dialogue
    {
        public string showname;
        public string message;
        public bool additive;
        public Character character;
        public Emote emote;
        public Courtroom courtroom;
        public Background bg;
        public AudioClip blip;
        public Action startAction;
        public Action endAction;

        public Dialogue(string showname, string message)
        {
            this.showname = showname;
            this.message = message;
            additive = false;
            startAction = null;
            endAction = null;
            character = new Character();
            emote = new Emote();
            courtroom = new Courtroom();
            bg = new Background();
            blip = null;
        }

        public Dialogue(string message)
        {
            this.showname = "";
            this.message = message;
            additive = false;
            startAction = null;
            endAction = null;
            character = new Character();
            emote = new Emote();
            courtroom = new Courtroom();
            bg = new Background();
            blip = null;
        }
    }


    private void Start()
    {
        FileManager.Initialize();
        dialogue.LoadSaveFile(0);
    }

    public void FixedUpdate()
    {
        if (CheckInputFromArray(continueKey))
        {
            NextDialogue();
        }

        if (Input.GetKeyDown(KeyCode.F11))
        {
            if (Screen.fullScreen)
            {
                Screen.fullScreen = false;
                Screen.fullScreenMode = FullScreenMode.Windowed;
            }
            else
            {
                Screen.fullScreen = true;
                Screen.fullScreenMode = FullScreenMode.MaximizedWindow;
            }
        }
    }

    public void NextDialogue() 
    {
        dialogue.Next();
    }

    bool CheckInputFromArray(KeyCode[] array)
    {
        bool keyPress = false;
        for (int i = 0; i < array.Length; i++)
        {
            if (Input.GetKeyDown(array[i]))
            {
                keyPress = true;
            }
        }

        return keyPress;
    }
    public void SetBackground(Background bg)
    {
        elements.backgroundImageHighRes.texture = bg.bg;
        elements.backgroundImage.texture = bg.bg;
        if(bg.desk != null)
        {
            elements.deskImage.texture = bg.desk;
        }
        else
        {
            ShowDesk(false);
        }

        if (bg.highRes)
        {
            elements.backgroundImage.gameObject.SetActive(false);
            elements.backgroundImageHighRes.gameObject.SetActive(true);
        }
        else
        {
            elements.backgroundImage.gameObject.SetActive(true);
            elements.backgroundImageHighRes.gameObject.SetActive(false);
        }
    }
    public void TurnOffEmoteImage()
    {
        elements.characterImage.gameObject.SetActive(false);
        elements.characterImageHighRes.gameObject.SetActive(false);
    }

    public void SetEmote(Emote emote)
    {
        ShowDesk(emote.showDesk);

        elements.characterImageHighRes.LoadEmote(emote);
        elements.characterImage.LoadEmote(emote);

        if (emote.highRes)
        {
            elements.characterImage.gameObject.SetActive(false);
            elements.characterImageHighRes.gameObject.SetActive(true);
        }
        else
        {
            elements.characterImage.gameObject.SetActive(true);
            elements.characterImageHighRes.gameObject.SetActive(false);
        }
    }
    
    public void SetShowname(string newName)
    {
        if(newName != null)
        {
            elements.shownameText.text = newName.Remove(shownameIgnoredCharacters);
        }
    }

    void ShowDesk(bool state)
    {
        elements.deskImage.gameObject.SetActive(state);
    }

    public bool isSpeaking { get { return speaking != null; } }
    [HideInInspector] public bool waitingForUserInput = false;
    Coroutine speaking;
    public TextScroller architect = null;
    IEnumerator Speaking(string message, string showname, bool additive, AudioClip blip = null)
    {
        bool showChatbox = true;

        if(message == "")
        {
            showChatbox = false;
        }

        elements.chatbox.SetActive(showChatbox);

        string additiveSpeech = additive ? elements.messageText.text : "";

        Action<int> blipAction = null;
        if (blip != null)
        {
            blipAction = delegate(int i) { if (i % 2 == 0) { AudioManager.instance.PlaySFX(blip, 1, 1, blipMixer); } };
        }

        if (architect == null)
        {
            architect = new TextScroller(elements.messageText, message, this, additiveSpeech, scrollCharactersPerFrame, scrollSpeed, blipAction);
        }
        else
        {
            architect.Renew(message, additiveSpeech, blipAction);
        }
        

        SetShowname(showname);
        waitingForUserInput = false;

        while (architect.isConstructing)
        {
            yield return new WaitForEndOfFrame();
        }

        Action action = currentDialogue.endAction;
        if (action != null)
        {
            action();
        }
        
        waitingForUserInput = true;
        while (waitingForUserInput)
        {
            yield return new WaitForEndOfFrame();
        }

        StopSpeaking();
    }
    public void Say(string message, string showname, bool additive, AudioClip blip)
    {
        StopSpeaking();
        speaking = StartCoroutine(Speaking(message, showname, additive, blip));
    }
    public void StopSpeaking()
    {
        if (isSpeaking)
        {
            StopCoroutine(speaking);
        }

        if(architect != null && architect.isConstructing)
        {
            architect.Stop();
        }
        speaking = null;
    }


    public void SetDialogue(Dialogue newDialogue)
    {
        currentDialogue = newDialogue;

        Action startAction = newDialogue.startAction;
        if (startAction != null)
        {
            startAction();
        }
        
        bool additive = newDialogue.additive;
        string message = newDialogue.message;
        Emote emote = newDialogue.emote;
        Background bg = newDialogue.bg;
        AudioClip blip = newDialogue.blip == null ? newDialogue.character.blip : newDialogue.blip;

        SetEmote(emote);

        if(bg.bg != null)
        {
            SetBackground(bg);
        }

        if(newDialogue.showname != "")
        {
            showname = newDialogue.showname;
        }

        if (!additive)
        {
            message = message.RemoveStartAndEndSpaces();
        }


        Say(message, showname, additive, blip);
    }
 }
