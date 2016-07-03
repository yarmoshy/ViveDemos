using UnityEngine;
using System.Collections;
using System;

public class GameState : MonoBehaviour {

    bool ready = false;
    public Vector3 defaultRestartLocation;
    public CheckPoint[] checkPoints;
    public CheckPoint currentCheckPoint;


    void Start()
    {
        int i = 0;
        foreach (CheckPoint checkPoint in checkPoints)
        {
            if (i == 0)
                checkPoint.gameObject.SetActive(true);
            else
                checkPoint.gameObject.SetActive(false);
            i++;
        }
        if (currentCheckPoint) SetCurrentCheckPoint(currentCheckPoint);
    }

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

    public Vector3 GetCheckPointPosition()
    {
        if (!currentCheckPoint) return defaultRestartLocation;
        return currentCheckPoint.gameObject.transform.position;
    }

    public void SetCurrentCheckPoint(CheckPoint newCheckPoint)
    {
        currentCheckPoint = newCheckPoint;
        currentCheckPoint.gameObject.SetActive(false);
        int index = Array.IndexOf(checkPoints, currentCheckPoint);
        if (checkPoints.Length > index + 1)
        {
            CheckPoint cp = checkPoints[index + 1];
            cp.gameObject.SetActive(true);
        }
    }
}
