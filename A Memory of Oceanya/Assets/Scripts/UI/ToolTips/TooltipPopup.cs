using System.Collections;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Kam.TooltipUI
{
    public class TooltipPopup : MonoBehaviour
    {
        public static TooltipPopup instance;
        public  RectTransform   popupObject;
        public  TextMeshProUGUI infoText;
        public  Vector3         offset;
        public  float           padding;

        private Canvas          popupCanvas;

        private void Awake()
        {
            instance = this;
            popupCanvas = GetComponentInParent<Canvas>();
            HideInfo();
        }

        private void Update()
        {
            FollowCursor();
        }

        private void FollowCursor()
        {
            if (!popupObject.gameObject.activeSelf) { return; }

            Vector3 newPos = Input.mousePosition + offset;
            newPos.z = 0;

            float maxXPos = Screen.width - ((popupObject.rect.width * 2.5f) * popupCanvas.scaleFactor) - padding;
            float minXPos = 0;
            float maxYPos = Screen.height - ((popupObject.rect.height * 2.5f) * popupCanvas.scaleFactor) - padding;
            float minYPos = 0;

            newPos.x = Mathf.Clamp(newPos.x, minXPos, maxXPos);
            newPos.y = Mathf.Clamp(newPos.y, minYPos, maxYPos);

            popupObject.position = newPos;
        }

        public void DisplayInfo(StringBuilder text)
        {
            infoText.text = text.ToString();
            popupObject.gameObject.SetActive(true);
            LayoutRebuilder.ForceRebuildLayoutImmediate(popupObject);
        }

        public void HideInfo()
        {
            popupObject.gameObject.SetActive(false);
        }
    }
}
