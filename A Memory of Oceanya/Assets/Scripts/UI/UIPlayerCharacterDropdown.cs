using System;
using System.Collections;
using System.Collections.Generic;
public class UIPlayerCharacterDropdown : UICustomDropdown<PlayerCharacter>
{
    public void SetCharacters(List<PlayerCharacter> characters)
    {
        SetOptions(characters, 
            x => new Tuple<string, PlayerCharacter>($"{x.name} - LVL {x.level.Level}", x));
    }
}


