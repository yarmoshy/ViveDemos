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
    }
}
