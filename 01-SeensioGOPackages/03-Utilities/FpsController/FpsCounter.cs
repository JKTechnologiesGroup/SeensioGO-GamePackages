using TMPro;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class FpsCounter : MonoBehaviour
    {
        private static FpsCounter instance;
        [SerializeField] private TextMeshProUGUI fpsText;
        [SerializeField] private float hudRefreshRate = 1f;
        [SerializeField] private float timer;

        private void Start()
        {
            if(instance == null)
            {
                instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
            }
        }
        private void Update()
        {
            if (Time.unscaledTime > timer)
            {
                int fps = (int)(1f / Time.unscaledDeltaTime);
                fpsText.text = "FPS: " + fps;
                timer = Time.unscaledTime + hudRefreshRate;
            }
        }
    }
}