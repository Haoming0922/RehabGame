using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

public class TrackGenerator : MonoBehaviour
{
    public GameObject trackSegmentPrefab;
    public GameObject trackEndPrefab;
    public int totalSegments = 200;
    private int numberOfSegments = 50;

    private float perlinNoiseScale = 0.1f;
    private float directionChangeIntensity = 2f;
    
    public Terrain terrain;
    private float[,] originalHeights;

    private Vector3 lastPosition;
    private Vector3 lastDirection;

    private float segmentLength;

    private Queue<GameObject> tracks = new Queue<GameObject>();

    public Transform userbike; 
    public Transform ghostBike;
    
    private Transform slowBike;
    private Transform fastBike;
    private int totalSegmentsCount;

    public GameObject finishMark;
    
    void Start()
    {
        totalSegmentsCount = numberOfSegments;
        lastDirection = trackSegmentPrefab.transform.forward;
        lastPosition = trackSegmentPrefab.transform.position;
        segmentLength = trackSegmentPrefab.transform.localScale.z * 4f;
        // terrain.terrainData.size = new Vector3(1000, terrain.terrainData.size.y, 1000);
        // originalHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        
        StartCoroutine(HandleTrack());
    }

    void Update()
    {
        if (userbike.position.z > ghostBike.position.z)
        {
            slowBike = ghostBike;
            fastBike = userbike;
        }
        else
        {
            slowBike = userbike;
            fastBike = ghostBike;
        }
    }

    public IEnumerator HandleTrack()
    {
        yield return StartCoroutine(GenerateTrack());
        StartCoroutine(RegenerateTracks());
    }
    
    public IEnumerator GenerateTrack()
    {
        Vector3 currentPosition = lastPosition;
        Vector3 currentDirection = lastDirection;
        
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 50f);

        for (int i = 0; i < numberOfSegments; i++)
        {
            float perlinX = Mathf.PerlinNoise(noiseOffsetX + i * perlinNoiseScale, 0) - 0.5f;
            float perlinY = Mathf.PerlinNoise(0, noiseOffsetY + i * perlinNoiseScale) - 0.5f;
            float perlinZ = currentDirection.z;
            perlinY *= 0.3f;

            Vector3 directionChange = new Vector3(perlinX, perlinY, perlinZ) * directionChangeIntensity;
            currentDirection += directionChange;
            currentDirection.Normalize();

            // Vector3 nextPosition = currentPosition + currentDirection * segmentLength;
            // nextPosition.y = (lastPosition + lastDirection * segmentLength).y;
            Vector3 nextPosition = currentPosition + lastDirection * segmentLength;
            GameObject segment = Instantiate(trackSegmentPrefab, nextPosition, Quaternion.LookRotation(currentDirection));
            
            currentPosition = nextPosition;
            lastDirection = currentDirection;
            
            tracks.Enqueue(segment);
            
            yield return null;
        }
        
        lastPosition = currentPosition;
    }

    public IEnumerator RegenerateTracks()
    {
        Vector3 currentPosition = lastPosition;
        Vector3 currentDirection = lastDirection;
        
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 50f);
        
        int count = 0;
        
        while (true)
        {
            if (count > 500) count = 0;
            
            GameObject segment = tracks.Peek();
            if ((segment.transform.position - slowBike.transform.position).magnitude < 300f)
            {
                yield return null;
                continue;
            }
                
            float perlinX = Mathf.PerlinNoise(noiseOffsetX + count * perlinNoiseScale, 0) - 0.5f;
            float perlinY = Mathf.PerlinNoise(0, noiseOffsetY + count * perlinNoiseScale) - 0.5f;
            float perlinZ = currentDirection.z;
            perlinY *= 0.3f;

            Vector3 directionChange = new Vector3(perlinX, perlinY, perlinZ) * directionChangeIntensity;
            currentDirection += directionChange;
            currentDirection.Normalize();

            // Vector3 nextPosition = currentPosition + currentDirection * segmentLength;
            // nextPosition.y = (lastPosition + lastDirection * segmentLength).y;
            Vector3 nextPosition = currentPosition + lastDirection * segmentLength;
            
            tracks.Dequeue();
            segment.transform.position = nextPosition;
            segment.transform.rotation = Quaternion.LookRotation(currentDirection);
            tracks.Enqueue(segment);
            
            currentPosition = nextPosition;
            lastDirection = currentDirection;
            count++;
            totalSegmentsCount ++;

            if (totalSegmentsCount > totalSegments) // finish
            {
                trackEndPrefab.transform.position = segment.transform.position + segment.transform.forward * segmentLength;
                trackEndPrefab.transform.rotation = segment.transform.rotation;
                trackEndPrefab.tag = "Finish";
                break;
            }
            
            yield return null;
        }
        
    }

    private Vector3 PerlinNoiseDirection(int offset, Vector3 currentDirection)
    {
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 100f);
        float perlinX = Mathf.PerlinNoise(noiseOffsetX + offset * perlinNoiseScale , 0) - 0.5f;
        float perlinY = Mathf.PerlinNoise(0, noiseOffsetY + offset * perlinNoiseScale) - 0.5f;
        float perlinZ = currentDirection.z;
        // float perlinZ = Mathf.PerlinNoise(noiseOffsetZ + i * perlinNoiseScale, 0) - 0.5f;
            
        perlinY *= 0.6f;
            
        return (currentDirection + new Vector3(perlinX, perlinY, perlinZ) * directionChangeIntensity).normalized;
    }
    

    void ModifyTerrainUnderTrack(Vector3 position)
    {
        Vector3 terrainPosition = position - terrain.transform.position;
        int x = (int)(terrainPosition.x / terrain.terrainData.size.x * terrain.terrainData.heightmapResolution);
        int z = (int)(terrainPosition.z / terrain.terrainData.size.z * terrain.terrainData.heightmapResolution);

        int size = 5; // Size of the area to modify
        float[,] heights = new float[size, size];
        float height = position.y / terrain.terrainData.size.y;

        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                heights[i, j] = height;
            }
        }
        int xBase = Mathf.Clamp(x - size / 2, 0, terrain.terrainData.heightmapResolution - size);
        int zBase = Mathf.Clamp(z - size / 2, 0, terrain.terrainData.heightmapResolution - size);


        terrain.terrainData.SetHeights(xBase, zBase, heights);
    }
    
    void ModifyTerrainToTrack(Vector3 trackPosition, float influenceRadius, float elevation)
    {
        TerrainData terrainData = terrain.terrainData;

        Vector3 terrainPos = terrain.transform.position;
        int mapWidth = terrainData.heightmapResolution;
        int mapHeight = terrainData.heightmapResolution;

        // Convert the track position to a coordinate on the heightmap
        int x = (int)(((trackPosition.x - terrainPos.x) / terrainData.size.x) * mapWidth);
        int z = (int)(((trackPosition.z - terrainPos.z) / terrainData.size.z) * mapHeight);

        // Define the area affected by the track
        int terrainPatchSize = (int)(influenceRadius / terrainData.size.x * mapWidth);
        float[,] heights = terrainData.GetHeights(x - terrainPatchSize, z - terrainPatchSize, terrainPatchSize * 2, terrainPatchSize * 2);

        // Modify the heights to create a path
        for (int i = 0; i < terrainPatchSize * 2; i++)
        {
            for (int j = 0; j < terrainPatchSize * 2; j++)
            {
                heights[i, j] = elevation / terrainData.size.y;
                // heights[i, j] = elevation;
            }
        }

        // Apply the modified heights back to the terrain
        terrainData.SetHeights(x - terrainPatchSize, z - terrainPatchSize, heights);
    }
    
    void ResetTerrain()
    {
        // Reset the heightmap to the original data
        terrain.terrainData.SetHeights(0, 0, originalHeights);
    }

}