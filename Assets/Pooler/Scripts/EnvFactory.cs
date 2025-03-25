using UnityEngine;

namespace Pooler.Scripts
{
    [CreateAssetMenu(menuName = "Env Factory", fileName = "New Env Factory", order = 0)]
    public class EnvFactory : ScriptableObject
    {
        public EnvironmentPrefabsSO[] Environments;
    }
}