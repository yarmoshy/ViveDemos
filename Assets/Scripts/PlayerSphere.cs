using UnityEngine;
using System.Collections;

public class PlayerSphere : VRTK_InteractableObject
{
    [Header("Game Objects", order = 4)]
    public GameObject player;
    public int touchForceMultiplier = 100;
    public ushort hapticFeedbackStrength = 500;

    VRTK_ControllerActions controllerActions;

    public override void StopTouching(GameObject touchingObject)
    {
        base.StopTouching(touchingObject);
        controllerActions = touchingObject.GetComponent<VRTK_ControllerActions>();
        Rigidbody prb = player.GetComponent<Rigidbody>();
        Vector3 impact = touchingObject.transform.localPosition.normalized * touchForceMultiplier;
        prb.AddForce(impact);
        controllerActions.TriggerHapticPulse(10, hapticFeedbackStrength);
    }

    protected override void Awake()
    {
        base.Awake();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    protected override void Update()
    {
        base.Update();
        if (player.transform.position.y < -100)
        {
            player.transform.position = new Vector3 (0, 1, 0);
            Rigidbody prb = player.GetComponent<Rigidbody>();
            prb.velocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
        }
    }
}
