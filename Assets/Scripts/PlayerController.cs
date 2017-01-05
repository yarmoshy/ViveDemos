using UnityEngine;
using System.Collections;

namespace Game
{
    public class PlayerController : MonoBehaviour
    {
        public GameObject player;

        private Vector3 offset;

        void Start()
        {
            offset = transform.position - player.transform.position;
        }

        void LateUpdate()
        {
            transform.position = player.transform.position + offset;
        }
    }
}
