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
        void LateUpdate()
        {
            transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y, player.transform.position.z);
            transform.rotation = Quaternion.identity;
        }
    }
}