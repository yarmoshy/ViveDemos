using UnityEngine;
using System.Collections;
using VRTK;

namespace Game
{

    public class PlayerTorus : MonoBehaviour
    {
        [Header("Game Objects", order = 4)]
        public GameObject player;

        // Update is called once per frame
        void FixedUpdate()
        {
            transform.position = new Vector3(player.transform.position.x + 1, player.transform.position.y, player.transform.position.z);
            transform.rotation = Quaternion.identity; 
        }
    }
}