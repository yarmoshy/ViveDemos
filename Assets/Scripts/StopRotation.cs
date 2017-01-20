using UnityEngine;
using System.Collections;

namespace Game
{
    public class StopRotation : MonoBehaviour
    {
        public GameObject player;

        public void DoStopRotation()
        {
            Rigidbody prb = player.GetComponent<Rigidbody>();
            //prb.velocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
        }
    }
}
