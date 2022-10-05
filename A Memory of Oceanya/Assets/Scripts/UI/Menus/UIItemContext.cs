using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Kam.TooltipUI;
public class UIItemContext : ButtonList
{
    public Color selectedColor;
    static Character loadedCharacter;
    public static UIItemContext instance;
    List<IObserver> _obs = new List<IObserver>();

    private void Awake()
    {
        instance = this;
        Hide();
        AddToObserver(BattleManager.i);
    }

    public void LoadItems(Character character)
    {
        ClearList();
        buttons = new List<Button>();
        loadedCharacter = character;
        for (int a = 0; a < character.inventory.Count; a++)
        {
            AddItem(character.inventory[a]);
        }
    }
    public void AddItem(ItemInfo item)
    {
        GameObject newEntry = AddObject(); ;
        newEntry.GetComponent<UIItemButton>().LoadItem(item.item, item.amount);
    }
    public void Show()
    {
        Character character = BattleManager.caster;
        LoadItems(character);
        DeactivateVisualSelect();
        gameObject.SetActive(true);
        NotifyObserver(ObservableActionTypes.ItemContextActivated);
    }
    public void Hide()
    {
        TooltipPopup.instance.HideInfo();
        
        gameObject.SetActive(false);

        NotifyObserver(ObservableActionTypes.ItemContextDeActivated);
    }

    public override void InteractableButtons(bool state)
    {
        base.InteractableButtons(state);

        if (state && gameObject.activeSelf)
        {
            for (int i = 0; i < list.Count; i++)
            {
                list[i].GetComponent<UIItemButton>().UpdateFormat();
            }
        }
    }

    public void VisualSelectButton(Item item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            UIItemButton current = list[i].GetComponent<UIItemButton>();
            if (current.loadedItem == item)
            {
                current.ActivateColorOverlay(selectedColor);
            }
            else
            {
                current.DeactivateColorOverlay();
            }
        }
    }

    public void DeactivateVisualSelect()
    {
        for (int i = 0; i < list.Count; i++)
        {
            UIItemButton current = list[i].GetComponent<UIItemButton>();

            current.DeactivateColorOverlay();
        }
    }

    public UIItemButton GetSkillButton(Item item)
    {
        for (int i = 0; i < list.Count; i++)
        {
            UIItemButton current = list[i].GetComponent<UIItemButton>();
            if (current.loadedItem == item)
            {
                return current;
            }
        }

        throw new System.Exception("Skill button of " + item.name + " skill could not be found.");
    }
    public void ActivateButtonInPosition(int position)
    {
        int index = position - 1;
        if(index < list.Count)
        {
            Character caster = BattleManager.caster;
            UIItemButton current = list[index].GetComponent<UIItemButton>();
            ActivatableInfo info = caster.GetItemFromInventory(current.loadedItem);

            if (info.activatable && !current.loadedItem.behaviors.Contains(Activatables.Behaviors.Passive))
            {
                current.ActivateLoadedItem();
            }
        }
    }

    #region Observer
    public void AddToObserver(IObserver obs)
    {
        if (!_obs.Contains(obs))
        {
            _obs.Add(obs);
        }
    }

    public void RemoveFromObserver(IObserver obs)
    {
        if (_obs.Contains(obs))
        {
            _obs.Remove(obs);
        }
    }

    public void NotifyObserver(ObservableActionTypes action)
    {
        for (int i = 0; i < _obs.Count; i++)
        {
            _obs[i].Notify(action);
        }
    }
    #endregion
}