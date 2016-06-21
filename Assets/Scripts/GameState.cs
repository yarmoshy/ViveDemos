using UnityEngine;
using System.Collections;

public class GameState : MonoBehaviour {

    bool ready = false;

    public void SetReady()
    {
        ready = true;
    }

    public void SetUnready()
    {
        ready = false;
    }

    public bool GetReady()
    {
        return ready;
    }
}
