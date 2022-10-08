using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Kam.Utils.Metagame;
using System.Linq;

public class LogHandler
{
    List<string> data = new List<string>();
    public Game_Manager.Dialogue dialogue;
    string lastShowname = "";
    public MonoBehaviour coroutineHandler;
    public static readonly string StartLog = "chapter_0.txt";
    
    public LogHandler(MonoBehaviour parent)
    {
        coroutineHandler = parent;
    }

    public void SaveSaveFile()
    {
        SaveFile loadedFile = SavesManager.loadedFile;
        loadedFile.logName = SavesManager.loadedLogName;
        loadedFile.coroutinesStored = coroutinesStored;
        loadedFile.dialogue = dialogue;
        loadedFile.currentDialogueOnScreen = Game_Manager.i.elements.messageText.text;

        SavesManager.Save(loadedFile);
    }
    public void LoadSaveFile(int fileNumber)
    {
        SavesManager.Load(fileNumber);

        SaveFile save = SavesManager.loadedFile;

        //Load data
        string path = Game_Manager.i.LogPath + save.logName;
        TextAsset loaded = Resources.Load<TextAsset>(path.Replace(".txt",""));
        data = FileManager.ReadTextAsset(loaded);

        SavesManager.loadedLogName = save.logName;
        Game_Manager.i.elements.messageText.text = save.currentDialogueOnScreen;
        dialogue = save.dialogue;
        coroutinesStored = save.coroutinesStored;

        Game_Manager.i.SetDialogue(dialogue);

        if(coroutinesStored.Count == 0)
        {
            LoadLog(Game_Manager.i.logName == "" ? SavesManager.loadedLogName : Game_Manager.i.logName);
        }
        else
        {
            for (int i = 0; i < coroutinesStored.Count; i++)
            {
                ProgressCoroutine coroutine = coroutinesStored[i];
                string name = coroutine.name;

                if(coroutine.parent == null)
                {
                    coroutineHandler.StartCoroutine(HandleLineList(name, false));
                }
            }
        }

        Debug.Log("Loaded savefile");
    }

    public void LoadLog(string filename)
    {
        SavesManager.loadedLogName = filename;

        data = FileManager.ReadTextAsset(Resources.Load<TextAsset>(Game_Manager.i.LogPath + filename.Replace(".txt","")));
        dialogue.showname = "";

        string coroutineName = CheckCoroutineName("chapterProgress");
        coroutinesStored.Add(new ProgressCoroutine(coroutineName, data));
        coroutineHandler.StartCoroutine(HandleLineList(coroutineName));
        Next();
    }

    bool _next = false;
    public void Next()
    {
        if (!InputScreen.isWaitingForUserInput)
        {
            _next = true;
        }
    }

    bool _skipLine = false;
    public void SkipLine()
    {
        if (!_skipLine)
        {
            _skipLine = true;
        }
    }
    public static List<ProgressCoroutine> coroutinesStored = new List<ProgressCoroutine>();
    Dictionary<string, Game_Manager.Dialogue> savedShownames = new Dictionary<string, Game_Manager.Dialogue>();

    [System.Serializable]
    public class ProgressCoroutine
    {
        public string name;
        public Progress progress;
        public ProgressCoroutine parent;
        public ProgressCoroutine child;

        public ProgressCoroutine(string name, List<string> data)
        {
            this.name = name;
            progress = new Progress(data);
        }
        public ProgressCoroutine SetChild(ProgressCoroutine child)
        {
            this.child = child;
            return this;
        }
        public ProgressCoroutine SetParent(ProgressCoroutine parent)
        {
            this.parent = parent;
            return this;
        }
    }
    [System.Serializable]
    public struct Progress
    {
        public int current;
        public List<string> data;

        public Progress(List<string> data)
        {
            current = 0;
            this.data = data;
        }
    }
    string CheckCoroutineName(string coroutineName)
    {
        string finalCoroutineName;
        bool contains;
        int passthroughs = 0;
        do
        {
            contains = false;
            finalCoroutineName = coroutineName + "_" + passthroughs;
            for (int i = 0; i < coroutinesStored.Count; i++)
            {
                if (coroutinesStored[i].name == finalCoroutineName)
                {
                    contains = true;
                }
            }
            
            passthroughs++;
        }
        while (contains);

        return finalCoroutineName;
    }
    IEnumerator HandleLineList(string progressName, bool resetProgress = true)
    {
        ProgressCoroutine current = coroutinesStored.First(res => res.name == progressName);
        List<string> lines = current.progress.data;
        Game_Manager.i.elements.chatbox_NextArrow.SetActive(false);
        if (resetProgress)
        {
            current.progress.current = 0;
        }

        if (current.child != null)
        {
            ProgressCoroutine child = current.child;
            current.SetChild(null);
            yield return HandleLineList(child.name, false);
        }

        while (current.progress.current < lines.Count)
        {
            Game_Manager.i.elements.chatbox_NextArrow.SetActive(true);
            if (_next || _skipLine)
            {
                Game_Manager.i.elements.chatbox_NextArrow.SetActive(false);
                _skipLine = false;
                string line = lines[current.progress.current];

                if (line.StartsWith("choice"))
                {
                    yield return HandlingChoiceLine(line, progressName, lines);
                }
                else
                {
                    HandleLine(line);
                    current.progress.current++;
                    while (isHandlingLine)
                    {
                        if (_skipLine)
                        {
                            break;
                        }
                        yield return new WaitForEndOfFrame();
                    }
                }
            }
            yield return new WaitForEndOfFrame();
        }
        coroutinesStored.Remove(current);
    }
    IEnumerator HandlingChoiceLine(string firstLine, string progressName, List<string> dataList)
    {
        string title = firstLine.Split('"')[1];
        ProgressCoroutine current = coroutinesStored.First(res => res.name == progressName);
        List<string> choices = new List<string>();
        List<List<string>> actions = new List<List<string>>();

        bool gatheringChoices = true;
        while (gatheringChoices)
        {
            current.progress.current++;
            string line = dataList[current.progress.current];

            if (line == "{")
            {
                continue;
            }

            line = line.RemoveStartSpaces();


            if (line != "}")
            {
                string choice = line.Split('"')[1];
                List<string> choiceActions = new List<string>();
                bool gatheringActions = true;
                int insideOfAChoice = 0;

                while (gatheringActions)
                {
                    string curAction = "";

                    curAction = dataList[current.progress.current + choiceActions.Count + 1].RemoveStartSpaces();

                    if (curAction == "{")
                    {
                        insideOfAChoice += 1;
                    }

                    if (curAction == "}")
                    {
                        insideOfAChoice -= 1;
                    }


                    if (insideOfAChoice == 0 && curAction == "break;")
                    {
                        gatheringActions = false;
                    }
                    else
                    {
                        choiceActions.Add(curAction);
                    }
                }

                choices.Add(choice);
                actions.Add(choiceActions);
                current.progress.current += choiceActions.Count + 1;
            }
            else
            {
                gatheringChoices = false;
            }
        }

        ChoiceScreen.Show(title, choices.ToArray());
        yield return new WaitForEndOfFrame();

        while (ChoiceScreen.isWaitingForChoice)
        {
            yield return new WaitForEndOfFrame();
        }

        List<string> action = actions[ChoiceScreen.lastChoiceMade.index];

        string coroutineName = CheckCoroutineName("choiceListProgres");
        ProgressCoroutine coroutine = new ProgressCoroutine(coroutineName, action);
        current.SetChild(coroutine);
        coroutine.SetParent(current);
        coroutinesStored.Add(coroutine);
        yield return HandleLineList(coroutineName);
        current.progress.current++;
    }

    
    void HandleLine(string rawLine)
    {
        LogLineHandler.Line line = LogLineHandler.Interpret(rawLine);

        StopHandlingLine();
        handlingLine = coroutineHandler.StartCoroutine(HandlingLine(line));
    }

    void StopHandlingLine()
    {
        if(isHandlingLine)
        {
            coroutineHandler.StopCoroutine(handlingLine);
        }
        handlingLine = null;
    }
    public bool isHandlingLine { get { return handlingLine != null; } }
    Coroutine handlingLine = null;

    //These variables exist because command lines are supposed to take effect on the next dialogue line, and not the last.
    bool lastLineChangeCourtroom = false;
    bool lastLineChangeEmote = false;
    IEnumerator HandlingLine(LogLineHandler.Line line)
    {
        if (line.type == LogLineHandler.Line.Types.Dialogue)
        {
            //its a dialogue line
            #region Load the dialogue of the character with this showname, if any
            string showname;
            if (line.showname != "")
            {
                lastShowname = line.showname;
            }

            showname = lastShowname;
            if (savedShownames.ContainsKey(showname))
            {
                if (!lastLineChangeCourtroom && !lastLineChangeEmote)
                {
                    //It was a command which wasn't set emote or set background
                    //You can pull from memory safely.
                    dialogue = savedShownames[showname];
                }
                else 
                {
                    if(!(lastLineChangeCourtroom && lastLineChangeEmote))
                    {
                        if (lastLineChangeCourtroom)
                        {
                            //last lines had the command set background in them
                            //you need to pull the emote from memory
                            dialogue.emote = savedShownames[showname].emote;

                        }
                        else if (lastLineChangeEmote)
                        {
                            //last lines had the command set emote in them
                            //you need to pull the background from memory
                            dialogue.bg = savedShownames[showname].bg;
                        }
                    }

                    dialogue.blip = savedShownames[showname].blip;
                }
                
            }
            lastLineChangeCourtroom = false;
            lastLineChangeEmote = false;
            #endregion

            #region Execute the line instructions
            _next = false;
            int lineProgress = 0;
            Game_Manager.i.elements.chatbox_NextArrow.SetActive(false);
            while (lineProgress < line.segments.Count)
            {
                _next = false;
                LogLineHandler.Line.Segment segment = line.segments[lineProgress];

                if (lineProgress > 0)
                {
                    switch (segment.trigger)
                    {
                        case LogLineHandler.Line.Segment.Trigger.WaitForClick:
                            
                            while (!_next)
                            {
                                Game_Manager.i.elements.chatbox_NextArrow.SetActive(true);
                                yield return new WaitForEndOfFrame();
                            }
                            Game_Manager.i.elements.chatbox_NextArrow.SetActive(false);
                            break;

                        case LogLineHandler.Line.Segment.Trigger.AutoDelay:
                            for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
                            {
                                yield return new WaitForEndOfFrame();
                                if (_next)
                                {
                                    break;
                                }
                            }
                            break;
                        case LogLineHandler.Line.Segment.Trigger.AutoDelay_NoSkip:
                            for (float timer = segment.autoDelay; timer >= 0; timer -= Time.deltaTime)
                            {
                                yield return new WaitForEndOfFrame();
                            }
                            break;
                    }
                }
                _next = false;
                segment.Run();

                while (segment.isRunning)
                {
                    yield return new WaitForEndOfFrame();

                    if (_next)
                    {
                        if (!segment.architect.skip)
                        {
                            segment.architect.skip = true;
                        }
                        else
                        {
                            segment.ForceFinish();
                        }
                        
                        _next = false;
                    }
                }
                lineProgress++;
                yield return new WaitForEndOfFrame();
            }
            #endregion

            #region Save the dialogue to the character
            savedShownames.AddOrOverwrite(showname, dialogue);
            #endregion
        }
        else
        {
            //Its a command line.
            #region Execute the commands
            for (int i = 0; i < line.actions.Count; i++)
            {
                HandleCommand(line.actions[i]);
            }
            #endregion
        }

        handlingLine = null;
    }

    public static Dictionary<string, string> GenerateVariableDictionary()
    {
        Dictionary<string, string> variables = new Dictionary<string, string>()
        {
            { "ping", "pong"},
            { "realName", Windows.userName},
            { "deviceName", Windows.deviceName}
        };

        foreach (var item in InputScreen.inputHistory)
        {
            if (!variables.ContainsKey(item.Key)) 
            {
                variables.Add(item.Key, item.Value);
            }
        }

        return variables;
    }
    public void HandleCommand(string line)
    {
        string[] split = line.RemoveStartAndEndSpaces().Split('(', ')');
        string function = split[0];
        string parameters = split[1];

        switch (function)
        {
            case "SetEmote":
                Command_SetEmote(parameters);
                lastLineChangeEmote = true;
                break;
            case "SetBackground":
                Command_SetBackground(parameters);
                lastLineChangeCourtroom = true;
                break;
            case "PlayMusic":
                Command_PlayMusic(parameters);
                break;
            case "PauseMusic":
                Command_PauseMusic();
                break;
            case "UnPauseMusic":
                Command_UnPauseMusic();
                break;
            case "StopMusic":
                Command_StopMusic();
                break;
            case "ReplayMusic":
                Command_ReplayMusic();
                break;
            case "PlaySFX":
                Command_PlaySFX(parameters);
                break;
            case "WaitForInput":
                Command_WaitForInput(parameters);
                break;
            case "ForceConfirmInput":
                InputScreen.i.Accept();
                break;
            case "Load":
                Command_Load(parameters);
                break;
            case "Next":
                Next();
                break;
            case "SkipLine":
                SkipLine();
                break;
            case "SetBlip":
                Command_SetBlip(parameters);
                break;
            case "RestoreBlip":
                Command_RestoreBlip();
                break;
            case "SetWallpaper":
                Command_SetWallpaper(parameters);
                break;
            case "Quit":
                Application.Quit();
                break;
            case "SetPlayPreanim":
                Command_SetPlayPreanim(parameters);
                break;
            default:
                throw new System.Exception("Function " + function  + " is not a valid command. (From txt line: "+line+")");
        }
    }

    void Command_SetEmote(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string emote*, string character? = ""
        string[] data = parameters.SplitParameters();

        string emoteName = data[0];
        string characterName = data.Length > 1 ? data[1] : "";
        bool preanim = data.Length > 2 ? bool.Parse(data[2]) : false;
        bool immediate = data.Length > 3 ? bool.Parse(data[3]) : false;

        if(emoteName.ToLower() == "empty")
        {
            dialogue.emote = AssetDatabase.i.GetCharacter("Narrator").GetEmote("Empty");
        }
        else
        {
            VisualNovelCharacter character = dialogue.character;

            if (characterName != "")
            {
                character = AssetDatabase.i.GetCharacter(characterName);
                dialogue.character = character;
            }

            Emote emote = character.GetEmote(emoteName);

            dialogue.emote = emote;
        }

        CharacterAnimator.playPreanim = preanim;
        CharacterAnimator.UninterruptedPreanim = immediate;
    }
    void Command_SetBackground(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string position*, string courtroom? = ""
        string[] data = parameters.SplitParameters();

        string positionName = data[0];
        string courtroomName = data.Length > 1 ? data[1] : "";

        Courtroom cr = dialogue.courtroom;

        if (courtroomName != "")
        {
            cr = AssetDatabase.i.GetCourtroom(courtroomName);
        }

        Background bg;
        
        if (positionName == "WindowsBackground")
        {
            bg = new Background() { bg = Windows.Wallpaper.Get(), highRes = true};
        }
        else
        {
            bg = cr.GetBackground(positionName);
        }

        dialogue.courtroom = cr;
        dialogue.bg = bg;
    }
    void Command_PlayMusic(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string name*, bool loop? = true
        string[] data = parameters.SplitParameters();

        string musicName = data[0];
        bool loop = data.Length == 1 ? true : bool.Parse(data[1]);

        AudioManager.instance.PlayMusic(AssetDatabase.i.GetSong(musicName), 1, 1, 0, true, loop);
    }
    void Command_PlaySFX(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string name*, bool loop? = true
        string[] data = parameters.SplitParameters();

        string sfxName = data[0];
        float volume = data.Length > 1 ? 1 : int.Parse(data[1]);
        float pitch = data.Length > 2 ? 1 : int.Parse(data[1]);
        AudioManager.instance.PlaySFX(AssetDatabase.i.GetSFX(sfxName), volume, pitch);
    }
    void Command_WaitForInput(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string variableName = "Last", string title?, bool clearCurrentInput? = true
        string[] data = parameters.SplitParameters(false);

        string varName = data[0] != "" ? data[0].Replace(" ", "") : "Last";
        string title = data.Length > 1 ? data[1].RemoveStartAndEndSpaces() : "";
        bool skipWholeLine = data.Length > 2 ? bool.Parse(data[2].Replace(" ", "")) : true;
        bool clearCurrentInput = data.Length > 3 ? bool.Parse(data[2].Replace(" ", "")) : true;

        System.Action action = delegate { Next(); };

        if(skipWholeLine)
        {
            action += delegate { Game_Manager.i.dialogue.SkipLine(); };
        }

        InputScreen.Show(varName, title, clearCurrentInput, action);
    }
    void Command_Load(string parameters)
    {
        string[] data = parameters.SplitParameters(true);
        string filename = data[0];
        LoadLog(filename);
    }
    void Command_SetBlip(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string characterName*
        string[] data = parameters.SplitParameters();
        string name = data[0];

        AudioClip blip;
        
        try
        {
            VisualNovelCharacter character = AssetDatabase.i.GetCharacter(name);
            blip = character.blip;
        }
        catch
        {
            blip = AssetDatabase.i.GetBlip(name);
        }

        dialogue.blip = blip;
    }
    void Command_RestoreBlip()
    {
        dialogue.blip = null;
    }
    void Command_SetWallpaper(string parameters)
    {
        //* = obligatory; ? = optional;
        //parameters: string wallpaperName*
        string[] data = parameters.SplitParameters();
        string name = data[0];

        //Windows.Wallpaper.Set();
    }
    void Command_StopMusic()
    {
        AudioManager.activeSong.Stop();
    }
    void Command_PauseMusic()
    {
        AudioManager.activeSong.Pause();
    }
    void Command_UnPauseMusic()
    {
        AudioManager.activeSong.UnPause();
    }
    void Command_ReplayMusic()
    {
        AudioManager.activeSong.Play();
    }
    void Command_SetPlayPreanim(string parameters)
    {
        string[] data = parameters.SplitParameters();
        bool play = bool.Parse(data[0]);

        CharacterAnimator.playPreanim = play;
    }
}
