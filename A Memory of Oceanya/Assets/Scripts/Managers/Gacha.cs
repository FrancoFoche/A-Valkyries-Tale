using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GachaPool<T>
{
    List<T> list;
    List<float> importances;
    public List<float> Importances { get { return importances; } }
    public List<T> List { get { return list; } }
    bool removeFromListWhenPulled;

    public GachaPool(Dictionary<T, float> a, bool removeFromListWhenPulled = false)
    {
        this.removeFromListWhenPulled = removeFromListWhenPulled;
        a.CopyValuesToLists(out list, out importances);
        importances = ConvertToEquivalentPercentage(importances);
    }

    public List<float> ConvertToEquivalentPercentage(List<float> originalImportances)
    {
        List<float> percentImportance = new List<float>();
        float maxPercentage = 100;
        float sumOfAllImportances = 0;

        #region Sum all
        for (int i = 0; i < importances.Count; i++)
        {
            sumOfAllImportances += originalImportances[i];
        }
        #endregion

        #region Convert each entry into its equivalent percentage (to not go over max percentage)
        for (int i = 0; i < originalImportances.Count; i++)
        {
            float equivalentPercentage = (originalImportances[i] * maxPercentage) / sumOfAllImportances;
            percentImportance.Add(equivalentPercentage);
        }
        #endregion

        return percentImportance;
    }

    public T Pull()
    {
        if(list.Count != importances.Count)
        {
            throw new System.Exception("Gacha System: Objects and importance counts didn't match up.");
        }

        float pull = Random.Range(0f, 100f);

        for (int i = 0; i < list.Count; i++)
        {
            if(pull <= importances[i])
            {
                T result = list[i];
                if (removeFromListWhenPulled)
                {
                    list.RemoveAt(i);
                    importances.RemoveAt(i);
                }
                return result;
            }
            else
            {
                pull -= importances[i];
            }
        }

        throw new System.Exception("Gacha System: Couldn't pull anything from its list.");
    }

    public List<T> PullXTimes(int pullAmount)
    {
        List<T> pulls = new List<T>();
        for (int i = 0; i < pullAmount; i++)
        {
            pulls.Add(Pull());
        }

        return pulls;
    }
}
