using UnityEngine;
using System.Collections;
using VRTK;

public class StartSphere : VRTK_InteractableObject
{
    public GameState gameState;

    public override void StopUsing(GameObject usingObject)
    {
        base.StopUsing(usingObject);
        gameState.SetReady();
    }

    new void Update()
    {
        if (gameState.GetReady())
        {
            GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            GetComponent<MeshRenderer>().enabled = true;
        }
    }
}
