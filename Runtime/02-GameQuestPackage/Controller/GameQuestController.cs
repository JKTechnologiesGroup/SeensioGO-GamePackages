using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class GameQuestController : MonoBehaviour
    {
        [SerializeField] private GameQuestManager gameQuestManager;
        private void Start()
        {
            gameQuestManager.StartQuestSession();
        }
        private void OnDestroy()
        {
            gameQuestManager.StopQuestSession();
        }
    }
}