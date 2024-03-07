using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Game.JumpJump
{
    public class GameManager : MonoBehaviour
    {
        public LevelGenerator levelGenerator;

        // Start is called before the first frame update
        void Start()
        {
            levelGenerator.GenerateGame();
        }

        // Update is called once per frame
        void Update()
        {

        }
    }
}