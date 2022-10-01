using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public static class SavesManager
{

    public static readonly string SAVE_FOLDER = FileManager.savPath + "Saves/";
    public static readonly string SAVE_NAME = "save";

    public static SaveFile loadedFile;
    public static int loadedFileNumber = 0;
    public static string loadedLogName = "";

    public static void Save(SaveFile save)
    {
        if (!Directory.Exists(SAVE_FOLDER))
        {
            Directory.CreateDirectory(SAVE_FOLDER);
        }

        FileManager.SaveEncryptedJSON(SAVE_FOLDER + SAVE_NAME + loadedFileNumber + ".txt", save, FileManager.keys);
    }

    public static SaveFile Load(int fileNumber)
    {
        loadedFileNumber = fileNumber;
        string fullSavePath = SAVE_FOLDER + SAVE_NAME + loadedFileNumber + ".txt";
        if (!File.Exists(fullSavePath))
        {
            Save(new SaveFile());
        }

        SaveFile save = FileManager.LoadEncryptedJSON<SaveFile>(fullSavePath, FileManager.keys);

        loadedFile = save;

        return loadedFile;
    }

    public static void DeleteSave()
    {
        string fullSavePath = SAVE_FOLDER + SAVE_NAME + loadedFileNumber + ".txt";

        if (File.Exists(fullSavePath))
        {
            File.Delete(fullSavePath);
        }
    }
}
[System.Serializable]
public class SaveFile
{
    public string logName;
    public List<LogHandler.ProgressCoroutine> coroutinesStored;
    public string currentDialogueOnScreen;
    public Game_Manager.Dialogue dialogue;

    public SaveFile()
    {
        logName = LogHandler.StartLog;

        coroutinesStored = new List<LogHandler.ProgressCoroutine>();
        currentDialogueOnScreen = "";
        dialogue = new Game_Manager.Dialogue();
    }
}
