using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseMenuA : MonoBehaviour, IObservable
{
    public bool paused;
    public Utilities_Button_BinarySprite buttonSprite;
    public GameObject pauseRoot;
    public GameObject settingsRoot;
    public Toggle manualMode;
    public Toggle confirmActions;
    public Slider volume;
    public GameObject generalRoot;
    private void Start()
    {
        if(BattleManager.i != null)
        {
            AddToObserver(BattleManager.i);
        }

        NotifyObserver(ObservableActionTypes.UnPaused);
    }
    public void TogglePause()
    {
        if (paused)
        {
            Hide();
        }
        else
        {
            Show();
        }

        buttonSprite.BinaryToggleSprite();
    }

    public void Show()
    {
        paused = true;
        NotifyObserver(ObservableActionTypes.Paused);
        SettingsManager.LoadSettings(SavesManager.loadedFile);
        LoadSettings();

        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(true);
        }

        if (pauseRoot != null)
        {
            pauseRoot.SetActive(true);
        }
    }

    public void Hide()
    {
        paused = false;
        NotifyObserver(ObservableActionTypes.UnPaused);
        pauseRoot.SetActive(false);
        UIActionConfirmationPopUp.i?.Hide();
        SettingsManager.SaveSettings();
    }

    public void VolumeSlider(Slider volumeSlider)
    {
        SettingsManager.volume = volumeSlider.value;
        GameAssetsManager.instance.SetVolume(volumeSlider.value);
    }

    public void SettingsButton()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(true);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(false);
        }
    }

    public void SettingsBackButton()
    {
        if (settingsRoot != null)
        {
            settingsRoot.SetActive(false);
        }

        if (generalRoot != null)
        {
            generalRoot.SetActive(true);
        }

        SettingsManager.SaveSettings();
    }

    public void SaveSettings()
    {
        SettingsManager.manualMode = manualMode.isOn;
        SettingsManager.actionConfirmation = confirmActions.isOn;
        SettingsManager.volume = volume.value;

        SettingsManager.SaveSettings();
    }
    public void LoadSettings()
    {
        manualMode.isOn = SettingsManager.manualMode;
        confirmActions.isOn = SettingsManager.actionConfirmation;
        volume.value = SettingsManager.volume;
    }
    public void SetDebugMode()
    {
        SettingsManager.SetDebugMode(manualMode.isOn);
    }
    public void SetConfirmMode()
    {
        SettingsManager.SetConfirmMode(confirmActions.isOn);
    }

    public void Quit()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.Quit, false, "Are you sure you want to quit?");
    }
    public void MainMenu()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.LoadMainMenu, false, "Are you sure you want to head to the Main Menu?");
    }
    public void Restart()
    {
        UIActionConfirmationPopUp.i.Show(SceneLoaderManager.instance.ReloadScene, false, "Are you sure you want to restart?");
    }

    #region Observer
    List<IObserver> _obs = new List<IObserver>();
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
