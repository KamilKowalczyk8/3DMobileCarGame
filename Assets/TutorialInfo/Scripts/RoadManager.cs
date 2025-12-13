using UnityEngine;
using System.Collections.Generic;

public class RoadManager : MonoBehaviour
{
    [Header("Ustawienia Drogi")]
    public GameObject roadPrefab;
    public int initialSegments = 12;
    public float segmentLength = 10f;

    [Header("Ustawienia Pasów")]
    [Range(3, 9)] 
    public int numberOfLanes = 7; 
    public float laneWidth = 3f;  

    [Header("Ustawienia Przeszkód")]
    public GameObject[] carPrefabs;
    public int minCarsPerSegment = 2; 
    public int maxCarsPerSegment = 5; 

    [Header("Pozycja Aut")]
    public float spawnHeight = 0.55f;

    [Header("Referencje")]
    public Transform player;

    private Queue<GameObject> roadSegments = new Queue<GameObject>();
    private float nextSpawnZ = 0f;

    void Start()
    {
        nextSpawnZ = 0f;
        for (int i = 0; i < initialSegments; i++)
        {
            bool spawnObstacles = i > 5;
            SpawnSegment(spawnObstacles);
        }
    }

    void Update()
    {
        if (player == null) return;

        if (roadSegments.Count > 0 && player.position.z > roadSegments.Peek().transform.position.z + segmentLength + 30f)
        {
            SpawnSegment(true);
            RemoveOldSegment();
        }
    }

    void SpawnSegment(bool withObstacles)
    {
        Vector3 spawnPos = new Vector3(0, 0, nextSpawnZ);
        GameObject newSegment = Instantiate(roadPrefab, spawnPos, Quaternion.identity);
        roadSegments.Enqueue(newSegment);

        if (withObstacles)
        {
            SpawnCarsOnSegment(newSegment);
        }

        nextSpawnZ += segmentLength;
    }

    void SpawnCarsOnSegment(GameObject segmentParent)
    {
        if (carPrefabs.Length == 0) return;

        List<int> availableLanes = new List<int>();
        int centerOffset = numberOfLanes / 2; 

        for (int i = -centerOffset; i <= centerOffset; i++)
        {
            availableLanes.Add(i);
        }

        int carCount = Random.Range(minCarsPerSegment, maxCarsPerSegment + 1);

        if (carCount >= numberOfLanes) carCount = numberOfLanes - 1;

        for (int i = 0; i < carCount; i++)
        {
            if (availableLanes.Count == 0) break;

            int randomIndex = Random.Range(0, availableLanes.Count);
            int selectedLaneMultiplier = availableLanes[randomIndex];
            availableLanes.RemoveAt(randomIndex); 

            int prefabIndex = Random.Range(0, carPrefabs.Length);

            float xPos = selectedLaneMultiplier * laneWidth;

            float zOffset = Random.Range(-0.5f, 0.5f);
            float zPos = segmentParent.transform.position.z + zOffset;

            Vector3 carPos = new Vector3(xPos, spawnHeight, zPos);

            GameObject car = Instantiate(carPrefabs[prefabIndex], carPos, Quaternion.Euler(0, 0, 0));
            car.transform.SetParent(segmentParent.transform);
        }
    }

    void RemoveOldSegment()
    {
        if (roadSegments.Count == 0) return;
        GameObject oldSegment = roadSegments.Dequeue();
        Destroy(oldSegment);
    }
}