using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TitleHeader : MonoBehaviour
{
    public RawImage banner;
    public TextMeshProUGUI titleText;
    public CanvasGroup canvasGroup;

    public string title { get { return titleText.text; } set { titleText.text = value; } }
    public DisplayMethod displayMethod = DisplayMethod.Instant;
    public float revelationSpeed = 1;
    public enum DisplayMethod
    {
        Instant,
        Fade,
        TypeWriter,
        FloatFade
    }

    public void Show(string displayText)
    {
        title = displayText;

        if (isRevealing)
        {
            StopCoroutine(revealing);
        }

        if (!cachedBannerPos)
        {
            cachedBannerOriginalPosition = banner.transform.position;
            cachedBannerPos = true;
        }

        revealing = StartCoroutine(Revealing());
    }

    public void Hide()
    {
        if (isRevealing)
        {
            StopCoroutine(revealing);
        }

        if (cachedBannerPos)
        {
            banner.transform.position = cachedBannerOriginalPosition;
        }

        revealing = null;

        banner.enabled = false;
        titleText.enabled = false;
    }

    bool cachedBannerPos = false;
    Vector3 cachedBannerOriginalPosition = Vector3.zero;
    public bool isRevealing { get { return revealing != null; } }
    Coroutine revealing = null;
    IEnumerator Revealing()
    {
        banner.enabled = true;
        titleText.enabled = true;

        switch (displayMethod)
        {
            case DisplayMethod.Instant:
                canvasGroup.alpha = 1;
                break;

            case DisplayMethod.Fade:
                canvasGroup.alpha = 0;

                while (canvasGroup.alpha < 1)
                {
                    canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1, revelationSpeed * Time.unscaledDeltaTime);
                    yield return new WaitForEndOfFrame();
                }
                break;

            case DisplayMethod.TypeWriter:
                canvasGroup.alpha = 1;

                TextScroller architect = new TextScroller(titleText, title, this,"",Game_Manager.i.scrollCharactersPerFrame, Game_Manager.i.scrollSpeed);

                while (architect.isConstructing)
                {
                    yield return new WaitForEndOfFrame();
                }
                break;

            case DisplayMethod.FloatFade:
                canvasGroup.alpha = 0;
                float amount = 25f * ((float)Screen.height / 720f);
                Vector3 downPos = new Vector3(0, amount, 0);
                banner.transform.position = cachedBannerOriginalPosition - downPos;

                while(canvasGroup.alpha < 1 || banner.transform.position != cachedBannerOriginalPosition)
                {
                    canvasGroup.alpha = Mathf.MoveTowards(canvasGroup.alpha, 1, revelationSpeed * Time.unscaledDeltaTime);

                    banner.transform.position = Vector3.MoveTowards(banner.transform.position, cachedBannerOriginalPosition, 11 * revelationSpeed * Time.unscaledDeltaTime);
                    yield return new WaitForEndOfFrame();
                }
                break;
        }

        revealing = null;
    }
}
