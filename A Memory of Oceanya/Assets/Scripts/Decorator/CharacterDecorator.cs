using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterDecorator : Character
{
    protected Character character;

    public CharacterDecorator(Character character)
    {
        this.character = character;
    }
}
