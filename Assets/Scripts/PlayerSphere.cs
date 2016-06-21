using UnityEngine;
using System.Collections;

public class PlayerSphere : VRTK_InteractableObject
{
    [Header("Game Objects", order = 4)]
    public GameObject player;
    public GameState gameState;
    public int touchForceMultiplier = 100;
    public ushort hapticFeedbackStrength = 500;

    ArrayList brakingControllers = new ArrayList(2); 
    VRTK_ControllerActions controllerActions;
    private float strength = 0;
    private float smoothTime = 40.0f;
    private bool reset = false;

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
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        if (reset)
        {
            DoResetPlayer();
        }
        if (brakingControllers.Count > 0)
        {
            DoStop();
        }
    }

    private void DoStop()
    {
        Rigidbody prb = player.GetComponent<Rigidbody>();
        Mathf.SmoothDamp(0.0f, 1.0f, ref strength, smoothTime);
        Vector3 f = -(prb.mass * prb.velocity) * strength;
        Vector3 t= -(prb.mass * prb.angularVelocity) * strength;

        prb.AddForce(f, ForceMode.Impulse);
        prb.AddTorque(t, ForceMode.Impulse);
        //prb.AddForce(-(Physics.gravity) * strength, ForceMode.Acceleration);
        //prb.AddTorque(-(Physics.gravity) * strength, ForceMode.Acceleration);
    }

    public void AddBraker(object sender)
    {
        brakingControllers.Add(sender);
    }

    public void RemoveBraker(object sender)
    {
        brakingControllers.Remove(sender);
    }

    public void ResetPlayer()
    {
        reset = true;
    }

    private void DoResetPlayer()
    {
        reset = false;
        player.transform.position = new Vector3(0, 1, 0);
        //player.transform.position = new Vector3(-30.78f, -72.091f, 303.43f);
        Rigidbody prb = player.GetComponent<Rigidbody>();
        prb.velocity = Vector3.zero;
        prb.angularVelocity = Vector3.zero;
        gameState.SetUnready();
    }
}
