using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DBSkills : MonoBehaviour
{
    public static List<Skill> skills = new List<Skill>();

    public static void          BuildDatabase   ()                                  
    {

        for (int i = 0; i < DBClasses.classes.Count; i++)
        {
            BaseSkillClass currentClass = DBClasses.classes[i];

            for (int j = 0; j < currentClass.skillList.Count ; j++)
            {
                Skill currentSkill = currentClass.skillList[j];
                currentSkill.skillClassID = currentClass.ID;
                skills.Add(currentSkill);
            }
        }
    }
}
