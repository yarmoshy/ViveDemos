using UnityEngine;
using System.Collections;
using VRTK;

namespace Game
{

    public class PlayerTorus : MonoBehaviour
    {
        [Header("Game Objects", order = 4)]
        public GameObject player;
        public PlayerSphere playerSphere;

        private Quaternion startRotation;
        private Quaternion startLocalRotation;


        // Update is called once per frame
        void Start()
        {
            startRotation = transform.rotation;
            startLocalRotation = transform.localRotation;
        }

        void LateUpdate()
        {
            //transform.rotation = playerSphere.transform.rotation;
            //transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y, player.transform.position.z);

            // transform.localRotation = player.transform.localRotation;
            //  transform.position = new Vector3(player.transform.position.x + 1f, player.transform.position.y, player.transform.position.z);
            transform.rotation = startRotation;
                transform.localRotation = startLocalRotation;
        }
    }
}