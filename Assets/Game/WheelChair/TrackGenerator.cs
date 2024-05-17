using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

namespace Wheelchair
{
    
public class TrackGenerator : MonoBehaviour
{
    public GameObject trackSegmentPrefab;
    public GameObject trackEndPrefab;
    public int totalSegments = 200;

    private float perlinNoiseScale = 0.1f;
    private float directionChangeIntensity = 2f;
    
    public Terrain terrain;
    private float[,] originalHeights;

    private Vector3 lastPosition;
    private Vector3 lastDirection;

    public float segmentLength;

    public float heightVariation;

    private Queue<GameObject> tracks = new Queue<GameObject>();
    
    
    void Start()
    {
        lastDirection = trackSegmentPrefab.transform.forward;
        lastPosition = trackSegmentPrefab.transform.position;
        segmentLength = segmentLength == 0 ? trackSegmentPrefab.transform.localScale.z * 4f : segmentLength;
        // terrain.terrainData.size = new Vector3(1000, terrain.terrainData.size.y, 1000);
        // originalHeights = terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution);
        
        StartCoroutine(HandleTrack());
    }

    public IEnumerator HandleTrack()
    {
        yield return StartCoroutine(GenerateTrack());
    }
    
    public IEnumerator GenerateTrack()
    {
        Vector3 currentPosition = lastPosition;
        Vector3 currentDirection = lastDirection;
        
        float noiseOffsetX = Random.Range(0f, 100f);
        float noiseOffsetY = Random.Range(0f, 50f);

        for (int i = 0; i < totalSegments; i++)
        {
            float perlinX = Mathf.PerlinNoise(noiseOffsetX + i * perlinNoiseScale, 0) - 0.5f;
            float perlinY = Mathf.PerlinNoise(0, noiseOffsetY + i * perlinNoiseScale) - 0.5f;
            float perlinZ = currentDirection.z;
            perlinY *= heightVariation;
            perlinX *= 2;

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

        trackEndPrefab.transform.position = currentPosition;
        // trackEndPrefab.transform.LookAt(lastDirection);
        
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
    


}
}