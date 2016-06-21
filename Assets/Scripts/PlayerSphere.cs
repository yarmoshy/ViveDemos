using UnityEngine;
using System.Collections;

public class PlayerSphere : VRTK_InteractableObject
{
    [Header("Game Objects", order = 4)]
    public GameObject player;
    public GameState gameState;
    public int touchForceMultiplier = 100;
    public ushort hapticFeedbackStrength = 500;

    VRTK_ControllerActions controllerActions;

    public override void StartTouching(GameObject touchingObject)
    {
        base.StartTouching(touchingObject);
        if (!gameState.GetReady()) return;

        controllerActions = touchingObject.GetComponent<VRTK_ControllerActions>();
        Rigidbody prb = player.GetComponent<Rigidbody>();
        Vector3 impact = touchingObject.transform.localPosition.normalized * touchForceMultiplier;
        prb.AddForce(new Vector3 (impact.x, 0, impact.z));
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
            gameState.SetUnready();
        }
    }
}
