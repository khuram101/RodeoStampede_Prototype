using UnityEngine;
using System.Collections.Generic;

public class AnimalSpawner : MonoBehaviour
{
    [Header("Spawning Settings")]
    public GameObject[] animalPrefabs;  // List of animal prefabs to spawn
    public Transform patchParent;       // Parent object containing patches
    public Vector2 spawnAreaSize = new Vector2(10f, 5f); // X = Width, Z = Depth
    public int animalCount = 5;         // Number of animals per patch

    private List<GameObject> spawnedAnimals = new List<GameObject>();

    [SerializeField] Transform patchCenter;

    void Start()
    {
        animalCount = Random.Range(5, 10);
        SpawnAnimalsOnPatch();
    }

    public void SpawnAnimalsOnPatch()
    {
        ClearAnimals(); // Remove old animals before spawning new ones

        for (int i = 0; i < animalCount; i++)
        {
            Vector3 spawnPos = GetRandomSpawnPosition();
            GameObject randomAnimal = animalPrefabs[Random.Range(0, animalPrefabs.Length)];
            GameObject spawnedAnimal = Instantiate(randomAnimal, spawnPos, Quaternion.identity);
            spawnedAnimals.Add(spawnedAnimal);
           // spawnedAnimal.transform.SetParent(null);
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        float x = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float z = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        return new Vector3(transform.position.x + x, transform.position.y, patchCenter.position.z + z);
    }

    public void ClearAnimals()
    {
        foreach (GameObject animal in spawnedAnimals)
        {
            if (animal != null)
                Destroy(animal);
        }
        spawnedAnimals.Clear();
    }
}
