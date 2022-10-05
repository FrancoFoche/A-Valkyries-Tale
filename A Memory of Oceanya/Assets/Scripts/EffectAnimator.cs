using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectAnimator : MonoBehaviour
{
    public enum Effects
    {
        Attack,
        Debuff,
        Buff,
        Heal,
        Death,
        Revive,
        Explosion,
        Special
    }

    public BattleUI ui;
    public Animator animator;

    #region Animator Event Actions
    public void PlayEffect(Effects effect)
    {
        Debug.Log("Played " + effect.ToString() + " effect.");
        animator.SetTrigger(effect.ToString());
    }

    public void PlaySound(Sounds sound)
    {
        SFXManager.PlaySound(sound);
    }
    
    public void CheckTeamDeath()
    {
        Debug.Log("Total team death: Act");
        
        BattleManager.i.TotalTeamKill_Act();
    }
    public void UpdateUI()
    {
        ui.UpdateUI();
    }
    #endregion
}
