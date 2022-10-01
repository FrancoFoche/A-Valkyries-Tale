using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DialogueEvents
{
    public static void HandleEvent(string _event, LogLineHandler.Line.Segment segment)
    {
        if (_event.Contains("("))
        {
            string[] commands = _event.Split(';');
            for (int i = 0; i < commands.Length; i++)
            {
                Game_Manager.i.dialogue.HandleCommand(commands[i]);
            }

            return;
        }

        string[] eventData = _event.Split(' ');

        switch (eventData[0])
        {
            case "txtSpd":
                Event_TextSpeed(eventData[1], segment);
                break;

            case "/txtSpd":
                segment.architect.speed = Game_Manager.i.scrollSpeed;
                segment.architect.charactersPerFrame = Game_Manager.i.scrollCharactersPerFrame;
                break;
            default:
                break;
        }
    }

    static void Event_TextSpeed(string data, LogLineHandler.Line.Segment segment)
    {
        string[] parts = data.Split(',');

        float delay = float.Parse(parts[0]);
        int charactersPerFrame = int.Parse(parts[1]);

        segment.architect.speed = delay;
        segment.architect.charactersPerFrame = charactersPerFrame;
    }
}
