using UnityEngine;
using UnityEngine.UI;

namespace Michsky.DreamOS
{
    [RequireComponent(typeof(Image))]
    public class HoverEffect : MonoBehaviour
    {
        public Image targetImage;
        [HideInInspector] public float speed;
        [HideInInspector] public float transitionAlpha;
        [HideInInspector] public bool fadeIn;
        [HideInInspector] public bool fadeOut;

        Color32 inColor = new Color32(1, 1, 1, 1);
        Color32 outColor = new Color32(1, 1, 1, 1);

        void Start()
        {
            inColor = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, transitionAlpha);
            outColor = new Color(targetImage.color.r, targetImage.color.g, targetImage.color.b, 0);
        }

        void Update()
        {
            targetImage.transform.position = Input.mousePosition;

            if (fadeIn == true)
            {
                targetImage.color = Color.Lerp(targetImage.color, inColor, Time.unscaledDeltaTime * speed);

                if (targetImage.color == inColor)
                    fadeIn = false;
            }

            if (fadeOut == true)
            {
                targetImage.color = Color.Lerp(targetImage.color, outColor, Time.unscaledDeltaTime * speed);

                if (targetImage.color == outColor)
                    gameObject.SetActive(false);
            }
        }
    }
}