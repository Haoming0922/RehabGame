using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inworld.UI
{
    public class Test : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            StartCoroutine(Record());
        }

        // Update is called once per frame
        void Update()
        {

        }

        IEnumerator Record()
        {
            while (true)
            {
                InworldController.Instance.StartAudio();
                yield return new WaitForSeconds(9);
                InworldController.Instance.PushAudio();
            }
        }
    }
}