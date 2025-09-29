using TMPro;
using UnityEngine;

namespace JKTechnologies.CommonPackage
{
    public class FpsController : MonoBehaviour
    {
        [SerializeField] private int target = 120;
        private void Start()
        {
            Application.targetFrameRate = target;
        }
    }
}