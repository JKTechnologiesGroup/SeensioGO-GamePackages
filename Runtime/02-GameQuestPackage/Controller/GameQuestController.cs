using UnityEngine;
using UnityEngine.Events;

namespace JKTechnologies.CommonPackage
{
    public class GameQuestController : MonoBehaviour
    {
        [SerializeField] private GameQuestManager gameQuestManager;
        [SerializeField] private UnityEvent OnQuestServiceStarted = new();
        private void Start()
        {
            gameQuestManager.StartQuestSession();
            OnQuestServiceStarted.Invoke();
        }
        private void OnDestroy()
        {
            gameQuestManager.StopQuestSession();
        }
    }
}