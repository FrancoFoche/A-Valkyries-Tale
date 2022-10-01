using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogLineHandler : MonoBehaviour
{
    public static Line Interpret(string rawLine)
    {
        return new Line(rawLine);
    }
    public class Line
    {
        public Types type;

        public string showname = "";
        public List<Segment> segments = new List<Segment>();
        public List<string> actions = new List<string>();
        public string wholeDialogueUpToLastSegment = "";

        public enum Types { Dialogue, Command}
        public Line(string rawLine)
        {
            type = Types.Command;
            string line = rawLine;

            if (line.Contains('"'.ToString()) || line.StartsWith("+"))
            {
                type = Types.Dialogue;
            }

            switch (type)
            {
                case Types.Dialogue:
                    DialogueInfo info = HandleDialogueLine(line);
                    showname = info.showname == "" ? Game_Manager.i.dialogue.dialogue.showname : info.showname;
                    Game_Manager.i.dialogue.dialogue.showname = showname;

                    SegmentDialogue(info.message);
                    break;

                case Types.Command:
                    string[] commands = line.Split(';');
                    for (int i = 0; i < commands.Length; i++)
                    {
                        actions.Add(commands[i].RemoveStartAndEndSpaces());
                    }
                    break;
            }
        }

        void SegmentDialogue(string dialogue)
        {
            segments.Clear();
            string[] parts = dialogue.Split('{', '}');

            for (int i = 0; i < parts.Length; i++)
            {
                bool isOdd = i % 2 != 0;

                Segment segment = new Segment();

                if (isOdd)
                {
                    Segment blankpost = new Segment();
                    string[] commandData = parts[i].Split(' ');
                    switch (commandData[0])
                    {
                        case "c"://wait for input and clear
                            segment.trigger = Segment.Trigger.WaitForClick;
                            break;
                        case "ca"://wait for input and clear (additive)
                            segment.trigger = Segment.Trigger.WaitForClick;
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                        case "w"://wait for x time
                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "wa"://wait for x time (additive)
                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                        case "wc"://wait for x time, skippable by input
                            segment.trigger = Segment.Trigger.AutoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "wca"://wait for x time, skippable by input (additive)
                            segment.trigger = Segment.Trigger.AutoDelay;
                            segment.autoDelay = float.Parse(commandData[1]);
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                        case "bp"://Waits for a click, and then blankposts for X seconds
                            blankpost.dialogue = "";
                            blankpost.line = this;
                            blankpost.trigger = Segment.Trigger.WaitForClick;
                            segments.Add(blankpost);

                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "bpa"://Waits for a click, and then blankposts for X seconds (additive)
                            blankpost.dialogue = "";
                            blankpost.line = this;
                            blankpost.trigger = Segment.Trigger.WaitForClick;
                            segments.Add(blankpost);

                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                        case "abp"://Automatically blankposts for X seconds
                            blankpost.dialogue = "";
                            blankpost.line = this;
                            blankpost.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            blankpost.autoDelay = 0.01f;
                            segments.Add(blankpost);

                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            break;
                        case "abpa"://Automatically blankposts for X seconds (additive)
                            blankpost.dialogue = "";
                            blankpost.line = this;
                            blankpost.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            blankpost.autoDelay = 0.01f;
                            segments.Add(blankpost);

                            segment.trigger = Segment.Trigger.AutoDelay_NoSkip;
                            segment.autoDelay = float.Parse(commandData[1]);
                            segment.pretext = segments.Count > 0 ? segments[segments.Count - 1].dialogue : "";
                            break;
                    }

                    i++;
                }

                string current = "";

                if (parts[i].EndsWith(" "))
                {
                    current = parts[i].Remove(parts[i].Length - 1);
                }
                else
                {
                    current = parts[i];
                }


                segment.dialogue = current;
                
                segment.line = this;

                segments.Add(segment);
            }
        }
        public class Segment
        {
            public Line line;
            public string dialogue = "";
            public string pretext = "";
            public enum Trigger { WaitForClick,AutoDelay, AutoDelay_NoSkip}
            public Trigger trigger = Trigger.WaitForClick;

            public float autoDelay = 0f;
            public List<string> currentlyExecutedEvents = new List<string>();
            
            public void Run()
            {
                if (isRunning)
                {
                    Game_Manager.i.dialogue.coroutineHandler.StopCoroutine(running);
                }

                running = Game_Manager.i.dialogue.coroutineHandler.StartCoroutine(Running());
            }

            public bool isRunning { get { return running != null; } }
            public TextScroller architect = null;
            Coroutine running = null;
            IEnumerator Running()
            {
                currentlyExecutedEvents.Clear();
                dialogue = dialogue.Inject(LogHandler.GenerateVariableDictionary(), "@");

                string[] dialogueEvents = dialogue.Split('@');
                bool onlyActionsNoDialogue = false;

                if(dialogueEvents.Length == 3)
                {
                    if(dialogueEvents[0] == "" || dialogueEvents[0] == " ")
                    {
                        if (dialogueEvents[2] == "" || dialogueEvents[2] == " ")
                        {
                            onlyActionsNoDialogue = true;
                        }
                    }
                }

                for (int i = 0; i < dialogueEvents.Length; i++)
                {
                    bool isOdd = i % 2 != 0;

                    if (isOdd)
                    {
                        DialogueEvents.HandleEvent(dialogueEvents[i], this);
                        currentlyExecutedEvents.Add(dialogueEvents[i]);
                        continue;
                    }
                    else
                    {
                        if(onlyActionsNoDialogue && (dialogueEvents[i] == "" || dialogueEvents[i] == " "))
                        {
                            continue;
                        }

                        dialogueEvents[i] = Text_ColorizedMarkup.i.AllMarkupsCheck(dialogueEvents[i].RemoveEndSpaces());
                    }

                    string target = dialogueEvents[i];
                    Game_Manager.i.dialogue.dialogue.message = target;
                    Game_Manager.i.dialogue.dialogue.additive = i > 0 ? true : pretext != "";
                    Game_Manager.i.SetDialogue(Game_Manager.i.dialogue.dialogue);

                    architect = Game_Manager.i.architect;

                    while (architect.isConstructing)
                    {
                        yield return new WaitForEndOfFrame();
                    }
                }

                running = null;
            }

            public void ForceFinish()
            {
                if (isRunning)
                {
                    Game_Manager.i.dialogue.coroutineHandler.StopCoroutine(running);
                }
                running = null;

                if(architect != null)
                {
                    architect.ForceFinish();

                    if(pretext == "")
                    {
                        line.wholeDialogueUpToLastSegment = "";
                    }

                    string[] parts = dialogue.Split('@');
                    string wholeDialogue = "";
                    for (int i = 0; i < parts.Length; i++)
                    {
                        bool isOdd = i % 2 != 0;
                        if (isOdd)
                        {
                            string e = parts[i];
                            if (currentlyExecutedEvents.Contains(e))
                            {
                                currentlyExecutedEvents.Remove(e);
                            }
                            else
                            {
                                Game_Manager.i.dialogue.HandleCommand(e);
                            }
                            i++;
                        }

                        wholeDialogue += parts[i];
                    }

                    line.wholeDialogueUpToLastSegment += wholeDialogue;

                    architect.ShowText(line.wholeDialogueUpToLastSegment);
                    
                }
            }
        }

        DialogueInfo HandleDialogueLine(string line)
        {
            //EXAMPLE OF A DIALOGUE LINE:
            //ExampleName: "Example dialogue"

            

            //Divide string by :
            string[] split = line.Split(':');

            string newMessage = "";
            string showname = "";
            if (split.Length == 1)
            {
                //its only a dialogue
                newMessage = split[0];
            }
            else if (split.Length >= 2)
            {
                //Length is more than one, first split is showname, rest is dialogue

                if (split[0] != "")
                {

                    showname = split[0];
                }

                for (int i = 1; i < split.Length; i++)
                {
                    newMessage += split[i];

                    if (i != split.Length - 1)
                    {
                        newMessage += ":";
                    }
                }
            }

            if (newMessage.StartsWith(" "))
            {
                newMessage = newMessage.Remove(0, 1);
            }

            if (showname.EndsWith(" "))
            {
                showname = showname.Remove(newMessage.Length - 1, 1);
            }

            DialogueInfo info = new DialogueInfo() { message = newMessage, showname = showname };
            return info;
        }

        struct DialogueInfo
        {
            public string message;
            public string showname;
        }
    }
}
