using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


namespace Game.JumpJump
{
    public class LevelGenerator : MonoBehaviour
    {
        public int seed;
        public List<GameObject> cubePrefs;
        public Transform firstCube;
        public int totalCubes;

        private Transform lastCube;

        private int cubeIdx = 2;

        public static List<Transform> cubes = new List<Transform>();

        private void Awake()
        {
            foreach (GameObject go in cubePrefs)
            {
                cubes.Add(go.transform);
            }
        }

        public void GenerateGame()
        {
            Random.InitState(seed);

            lastCube = firstCube;
            while (cubeIdx < totalCubes)
            {
                int cubeInGroup = Random.Range(2, 10);
                float totalRotation = Random.Range(-90, 90);
                for (int i = 0; i < cubeInGroup; i++)
                {
                    if (cubeIdx == totalCubes) return;

                    GameObject cubePref = cubePrefs[Random.Range(0, cubePrefs.Count)];
                    GameObject newCube = Instantiate(cubePref, cubePref.transform.parent);
                    cubeIdx++;

                    newCube.transform.position = lastCube.position +
                                                 lastCube.right * Random.Range(-4f, 4f) +
                                                 lastCube.forward * Random.Range(7f, 15f) +
                                                 new Vector3(0, Random.Range(-5f, 5f), 0);
                    newCube.transform.Rotate(0, totalRotation / cubeInGroup * i, 0);
                    newCube.name = "Cube" + cubeIdx;

                    cubes.Add(newCube.transform);
                    lastCube = newCube.transform;
                }
            }
        }

    }
}