using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.Samples
{
    public class SceneLoaderByName : MonoBehaviour
    {
        [SerializeField] private string sceneName;

        public void LoadScene()
        {
            GameEngineManager.Instance.LoadSceneByName(sceneName);
        }
    }
}