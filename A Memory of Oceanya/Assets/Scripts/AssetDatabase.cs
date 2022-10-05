using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#region Asset Enums
public enum Courtrooms
{
    Anime
}
public enum Characters
{
    GaenIzuko,
    Loremaster
}
public enum SFXs
{

}

public enum Sounds
{
    AttackSlash,
    Hit,
    Buff,
    Debuff,
    Heal,
    Death,
    Explosion,
    Special
}

public enum Music
{
    MainMenu,
    Combat,
    Win,
    Lose,
    Credits,
    GarouTheme,
    GenosTheme,
    GoblinSlayer,
    SasukeTheme,
    ParanormalLiberationFront,
    None
}

public enum Sprites
{
    MagnoDrip,
    Sasque,
    Vergil,
    Obama,
    Kirbo,
    Saber,
    HellTakerDemon,
    Akira
}

public enum ItemIcon
{
    Liquid_green,
    Liquid_red,
    Liquid_yellow,
    Liquid_blue,
}
#endregion

#region CharacterEmote enums
#endregion
public class AssetDatabase : MonoBehaviour
{
    #region singletonSetup
    private static AssetDatabase instance;
    public static AssetDatabase i { get { return instance; } }
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }
    #endregion

    public Courtroom[] courtrooms;
    public VisualNovelCharacter[] characters;
    public SFX[] sfxs;
    public Song[] music;
    public Blip[] blips;

    public Courtroom GetCourtroom(Courtrooms name)
    {
        return GetCourtroom(name.ToString());
    }
    public VisualNovelCharacter GetCharacter(Characters name)
    {
        return GetCharacter(name.ToString());
    }
    public AudioClip GetSFX(SFXs name)
    {
        return GetSFX(name.ToString());
    }
    public AudioClip GetSong(Music name)
    {
        return GetSong(name.ToString());
    }

    public Courtroom GetCourtroom(string name)
    {
        for (int i = 0; i < courtrooms.Length; i++)
        {
            Courtroom cr = courtrooms[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr;
            }
        }

        throw new System.Exception("Id " + name + " not found in courtrooms array.");
    }
    public VisualNovelCharacter GetCharacter(string name)
    {
        for (int i = 0; i < characters.Length; i++)
        {
            VisualNovelCharacter cr = characters[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr;
            }
        }

        throw new System.Exception("Id " + name + " not found in characters array.");
    }
    public AudioClip GetSFX(string name)
    {
        for (int i = 0; i < courtrooms.Length; i++)
        {
            SFX cr = sfxs[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr.sfx;
            }
        }

        throw new System.Exception("Id " + name + " not found in characters array.");
    }
    public AudioClip GetSong(string name)
    {
        for (int i = 0; i < music.Length; i++)
        {
            Song cr = music[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr.music;
            }
        }

        throw new System.Exception("Id " + name + " not found in music array.");
    }
    public AudioClip GetBlip(string name)
    {
        for (int i = 0; i < blips.Length; i++)
        {
            Blip cr = blips[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr.blip;
            }
        }

        throw new System.Exception("Id " + name + " not found in blips array.");
    }
}


[System.Serializable]
public struct SFX
{
    public string name;
    public AudioClip sfx;
}

[System.Serializable]
public struct Song
{
    public string name;
    public AudioClip music;
}

[System.Serializable]
public struct Background
{
    public Texture2D bg;
    public Texture2D desk;
    [Tooltip("Turn on 16:9 ratio")]
    public bool highRes;
}

[System.Serializable]
public struct Courtroom
{
    public string name;
    public position currentPos;
    public Background defense;
    public Background judge;
    public Background prosecution;
    public Background witness;
    public Background helper_def;
    public Background helper_pro;

    public enum position
    {
        defense,
        judge,
        witness,
        prosecutor,
        co_defense,
        co_prosecutor
    }
    public Background GetBackground(position pos)
    {
        return GetBackground(pos.ToString());
    }
    public Background GetBackground(string pos)
    {
        switch (pos.ToLower())
        {
            case "defense":
                return defense;
            case "judge":
                return judge;
            case "witness":
                return witness;
            case "prosecutor":
                return prosecution;
            case "co_defense":
                return helper_def;
            case "co_prosecutor":
                return helper_pro;
        }

        throw new System.Exception("Position doesn't exist in courtroom");
    }
}

[System.Serializable]
public struct Emote
{
    public string name;
    public bool showDesk;
    public List<Texture2D> preanimSprites;
    public List<Texture2D> animSprites;
    public Texture2D sprite;
    [Tooltip("Turn on 16:9 ratio")]
    public bool highRes;
}

[System.Serializable]
public struct VisualNovelCharacter
{
    public string name;
    public AudioClip blip;
    public Courtroom.position pos;

    public Emote[] emotes;
    public Emote GetEmote(string name)
    {
        for (int i = 0; i < emotes.Length; i++)
        {
            Emote cr = emotes[i];

            if (cr.name.ToLower() == name.ToLower())
            {
                return cr;
            }
        }

        throw new System.Exception("Emote of name " + name + " not found in character " + this.name.ToString() +"'s array.");
    }
}
[System.Serializable]
public struct Blip
{
    public string name;
    public AudioClip blip;
}