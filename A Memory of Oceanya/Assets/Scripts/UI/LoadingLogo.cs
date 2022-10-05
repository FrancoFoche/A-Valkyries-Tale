using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingLogo : MonoBehaviour
{
    public static LoadingLogo i;

    public Transform fillTransform;
    public Transform maskTransform;
    public RectTransform maskRect;
    public Animator anim;

    public bool testingMode;
    [SerializeField]
    [Range(0f, 1f)]
    float inspectorFillAmount = 0;

    float _fillAmount;
    public float fillAmount { get { return _fillAmount; } set { _fillAmount = value;  UpdateFillAmount(value); } }
    AsyncOperation operation;
    private void Awake()
    {
        if(i == null)
        {
            i = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        fillAmount = 0;
    }
    private void Update()
    {
        if (operation != null)
        {
            //Operation progress can only range from 0 to 0.9, so transform it from 0/0.9 to 0/1
            float newFill = operation.progress * 1 / 0.9f;
            Debug.Log("Loading Screen: Updating fill to " + newFill);
            fillAmount = newFill;
        }

        //tests
        if (fillAmount != inspectorFillAmount && testingMode)
        {
            fillAmount = inspectorFillAmount;
        }
    }
    void UpdateFillAmount(float newValue)
    {
        float maxFill = 1;
        float minPos = maskTransform.localPosition.y - maskRect.rect.height;
        float maxPos = maskTransform.localPosition.y;

        float posDifference = maxPos - minPos;

        float relativePosDifference = ((1-newValue) * posDifference) / maxFill;
        float newPos = maxPos - relativePosDifference;
        fillTransform.transform.localPosition = new Vector3(maskTransform.localPosition.x, newPos);
    }

    public void StartLoad(AsyncOperation asyncOp)
    {
        operation = asyncOp;
        operation.completed += EndLoad;
        anim.Play("FadeIn");
    }

    public void EndLoad(AsyncOperation asyncOp)
    {
        operation = null;

        anim.Play("FadeOut");
    }
}
