using UnityEngine;

namespace JKTechnologies.SeensioGo.GameEngines.Samples
{
    public class SceneLoaderByIndex : MonoBehaviour
    {
        [SerializeField] private int sceneIndex;
        public void LoadScene()
        {
            GameEngineManager.Instance.LoadSceneByIndex(sceneIndex);
        }
    }
}