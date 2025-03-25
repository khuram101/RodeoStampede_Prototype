using UnityEngine;

namespace Pooler.Scripts
{
    [CreateAssetMenu(fileName = "EnvironmentPrefabs", menuName = "ScriptableObjects/EnvironmentPrefabs")]
    public class EnvironmentPrefabsSO : ScriptableObject
    {
        [SerializeField] EnvironmentSection[] envPrefabs;

        public EnvironmentSection GetEnvironmentPrefab(int index)
        {
            return envPrefabs[index];
        }
        public int GetEnvironmentCount() => envPrefabs.Length;
    }
}