using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//i didn't even end up using these.
public class Buff : CharacterDecorator
{
    int buff;
    Stats stat;

    public Buff(Character c, int buff, Stats stat) : base(c)
    {
        this.buff = buff;
        this.stat = stat;
    }

    public override Stat GetStat(Stats stat)
    {
        if(this.stat == stat)
        {
            return base.GetStat(stat) + buff;
        }

        return base.GetStat(stat);
    }
}

public class Debuff : CharacterDecorator
{
    int debuff;
    Stats stat;

    public Debuff(Character c, int debuff, Stats stat) : base(c)
    {
        this.debuff = debuff;
        this.stat = stat;
    }

    public override Stat GetStat(Stats stat)
    {
        if (this.stat == stat)
        {
            return base.GetStat(stat) - debuff;
        }

        return base.GetStat(stat);
    }
}
