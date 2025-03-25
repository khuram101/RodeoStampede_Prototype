using System.Collections.Generic;
using UnityEngine;

namespace Pooler.Scripts
{
    public class EnvPooler : MonoBehaviour
    {
        [SerializeField] private EnvFactory env;
        [SerializeField] private int startPoolSize = 5;
        [SerializeField] private float safeZone = 50f;
        

        private List<GameObject> activeObjects = new List<GameObject>();
        private Queue<GameObject> objectPool = new Queue<GameObject>();
        private int currentEnvIndex = 0;
        private int currentPrefabIndex = 0;

        private Transform playerTransform;
        private float lastZPosition;

        private void Start()
        {
            playerTransform = Camera.main.transform;
            lastZPosition = playerTransform.position.z;
            InitializePool();
        }

        private void Update()
        {
            if (playerTransform.position.z - lastZPosition > safeZone)
            {
                lastZPosition = playerTransform.position.z;
                SpawnNext();
                RemoveOld();
            }
        }

        private void InitializePool()
        {
            for (int i = 0; i < startPoolSize; i++)
            {
                SpawnNext();
            }
        }

        private void SpawnNext()
        {
            if (env.Environments.Length == 0) return;
            EnvironmentPrefabsSO currentSO = env.Environments[currentEnvIndex];

            if (currentSO.GetEnvironmentCount() == 0) return;
            EnvironmentSection prefab = currentSO.GetEnvironmentPrefab(currentPrefabIndex);

            GameObject obj;
            if (objectPool.Count > 0)
            {
                obj = objectPool.Dequeue();
                obj.SetActive(true);
                //obj.GetComponent<AnimalSpawner>().SpawnAnimalsOnPatch();
            }
            else
            {
                obj = Instantiate(prefab.gameObject);
               // obj.GetComponent<AnimalSpawner>().SpawnAnimalsOnPatch();
            }

            float spawnZ = activeObjects.Count > 0 ? activeObjects[activeObjects.Count - 1].transform.position.z + activeObjects[activeObjects.Count - 1].GetComponent<IEnvironmentSection>().GetLength() : 0f;
            obj.transform.position = new Vector3(0, 0, spawnZ);
            activeObjects.Add(obj);

            AdvanceIndices();
        }

        private void RemoveOld()
        {
            if (activeObjects.Count > startPoolSize)
            {
                GameObject obj = activeObjects[0];
                activeObjects.RemoveAt(0);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
            }
        }

        private void AdvanceIndices()
        {
            currentPrefabIndex++;
            if (currentPrefabIndex >= env.Environments[currentEnvIndex].GetEnvironmentCount())
            {
                currentPrefabIndex = 0;
                currentEnvIndex++;
                if (currentEnvIndex >= env.Environments.Length)
                {
                    currentEnvIndex = 0;
                }
            }
        }
    }
}
