using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class TextScroller
{
	/// <summary>A dictionary keeping tabs on all architects present in a scene. Prevents multiple architects from influencing the same text object simultaneously.</summary>
	private static Dictionary<TextMeshProUGUI, TextScroller> activeArchitects = new Dictionary<TextMeshProUGUI, TextScroller>();

	string preText;
	string targetText;

	public int charactersPerFrame = 1;
	public float speed = 1f;

	public bool skip = false;

	/// <summary>
	/// An action that is done at every reveal time
	/// </summary>
	public Action<int> everyCharacter;

	public bool isConstructing { get { return buildProcess != null; } }
	Coroutine buildProcess = null;

	TextMeshProUGUI tmpro;
	MonoBehaviour coroutineHandler;
	public TextScroller(TextMeshProUGUI tmpro, string targetText, MonoBehaviour coroutineHandler, string preText = "", int charactersPerFrame = 1, float speed = 1f, Action<int> everyCharacter = null)
	{
		this.tmpro = tmpro;
		this.targetText = targetText;
		this.preText = preText;
		this.charactersPerFrame = charactersPerFrame;
		this.speed = Mathf.Clamp(speed, 1f, 300f);
		this.coroutineHandler = coroutineHandler;
		this.everyCharacter = everyCharacter;

		Initiate();
	}

	public void Stop()
	{
		if (isConstructing)
		{
			coroutineHandler.StopCoroutine(buildProcess);
		}
		buildProcess = null;
	}

	IEnumerator Construction()
	{
		int runsThisFrame = 0;

		tmpro.text = "";
		tmpro.text += preText;

		tmpro.ForceMeshUpdate(false);
		TMP_TextInfo inf = tmpro.textInfo;
		int vis = inf.characterCount;

		tmpro.text += targetText;

		tmpro.ForceMeshUpdate(false);
		inf = tmpro.textInfo;
		int max = inf.characterCount;

		tmpro.maxVisibleCharacters = vis;

		int cpf = charactersPerFrame;
		float speed = this.speed;
		int i = 0;
		while(vis < max)
		{
			//allow skipping by increasing the characters per frame and the speed of occurance.
			if (skip)
			{
				speed = 1;
				cpf = charactersPerFrame < 5 ? 5 : charactersPerFrame + 3;
			}

			//reveal a certain number of characters per frame.
			while(runsThisFrame < cpf)
			{
				vis++;
				tmpro.maxVisibleCharacters = vis;
				runsThisFrame++;
			}

			//Execute the action saved
			everyCharacter?.Invoke(i);

			//wait for the next available revelation time.
			runsThisFrame = 0;
			yield return new WaitForSeconds(0.01f * speed);

			i++;
		}

		//terminate the architect and remove it from the active log of architects.
		Terminate();
	}

	void Initiate()
	{
		//check if an architect for this text object is already running. if it is, terminate it. Do not allow more than one architect to affect the same text object at once.
		TextScroller existingArchitect;
		if (activeArchitects.TryGetValue(tmpro, out existingArchitect))
        {
			existingArchitect.Terminate();
		}

		buildProcess = coroutineHandler.StartCoroutine(Construction());
		activeArchitects.Add(tmpro, this);
	}

	/// <summary>
	/// Terminate this architect. Stops the text generation process and removes it from the cache of all active architects.
	/// </summary>
	public void Terminate()
	{
		activeArchitects.Remove(tmpro);
		if (isConstructing)
        {
			coroutineHandler.StopCoroutine(buildProcess);
		}
			
		buildProcess = null;
	}

	public void ForceFinish()
	{
		tmpro.maxVisibleCharacters = tmpro.text.Length;
		Terminate();
    }

	public void Renew(string target, string preText, Action<int> everyCharacter)
    {
		targetText = target;
		this.preText = preText;
		this.everyCharacter = everyCharacter;
		skip = false;

        if (isConstructing)
        {
			coroutineHandler.StopCoroutine(buildProcess);
        }

		buildProcess = coroutineHandler.StartCoroutine(Construction());
    }

	public void ShowText(string show)
    {
		if (isConstructing)
		{
			coroutineHandler.StopCoroutine(buildProcess);
		}

		targetText = show;
		tmpro.text = show;

		tmpro.maxVisibleCharacters = tmpro.text.Length;
	}
}