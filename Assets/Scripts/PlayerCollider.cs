using UnityEngine;
using System.Collections;

namespace Game
{
    public class PlayerCollider : MonoBehaviour
    {
        public PlayerSphere playerSphere;

        ArrayList removedCollideres = new ArrayList();

        void OnCollisionEnter(Collision col)
        {
            if (col.gameObject.name == "Terrain")
            {
                playerSphere.ResetPlayer();
                ResetRemovedColliders();
            }
            if (col.gameObject.tag == "BoostPlatform")
            {
                //Debug.Log("Entered BoostPlatform:" + col.gameObject.name);

                if (col.gameObject.name.Contains("BoostPower:"))
                {
                    playerSphere.BoostPlayer(float.Parse(col.gameObject.name.Substring("BoostPower:".Length)));
                }
                else
                {
                    playerSphere.BoostPlayer();
                }
            }
        }

        void OnTriggerEnter(Collider col)
        {
            if (col.gameObject.tag == "Checkpoint")
            {
                playerSphere.SetCheckpoint(col.gameObject.GetComponent<CheckPoint>());
            }
        }

        void OnCollisionExit(Collision col)
        {
            if (col.gameObject.tag == "BoostPlatform")
            {
                playerSphere.UnboostPlayer();
            }
            if (col.gameObject.tag == "RemoveAfterCollision")
            {
                col.gameObject.SetActive(false);
                if (!removedCollideres.Contains(col))
                { 
                    removedCollideres.Add(col);
                }
            }
        }

        public void ResetRemovedColliders()
        {
            foreach (Collision col in removedCollideres)
            {
                col.gameObject.SetActive(true);
            }
            removedCollideres.Clear();
        }
    }
}