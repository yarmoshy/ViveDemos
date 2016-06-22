using UnityEngine;
using System.Collections;

public class PlayerCollider : MonoBehaviour {
    public PlayerSphere playerSphere;

    void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.name == "Terrain")
        {
            playerSphere.ResetPlayer();
        }
        if (col.gameObject.tag == "BoostPlatform")
        {
            playerSphere.BoostPlayer();
        }
    }

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.tag == "BoostPlatform")
        {
            playerSphere.UnboostPlayer();
        }
    }
}
