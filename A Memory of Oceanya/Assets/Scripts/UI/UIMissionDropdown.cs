using System;
using System.Collections;
using System.Collections.Generic;
public class UIMissionDropdown : UICustomDropdown<LevelManager.BattleLevel>
{
    public void SetLevels(List<LevelManager.BattleLevel> levels)
    {
        SetOptions(levels, 
            x => new Tuple<string, LevelManager.BattleLevel>($"{x.levelNumber} - {x.name}", x));
    }
}
