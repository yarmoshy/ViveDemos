using UnityEngine;
using System.Collections;

public class PlayerSphere : VRTK_InteractableObject
{
    [Header("Game Objects", order = 4)]
    public GameObject player;
    public GameState gameState;
    public int touchForceMultiplier = 100;
    public ushort hapticFeedbackStrength = 500;
    public float maxBrakeTime = 1000.0f;
    public float minBrakeTime = 20.0f;

    ArrayList brakingPressures = new ArrayList(); 
    VRTK_ControllerActions controllerActions;
    private float strength = 0;
    private float distanceToGround;
    private bool reset = false;
    private bool boost = false;

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

    public void AddBraker(object sender)
    {
        brakingPressures.Add(sender);
    }

    public void RemoveBraker(object sender)
    {
        brakingPressures.Remove(sender);
    }

    public void ResetPlayer()
    {
        reset = true;
    }

    public void BoostPlayer()
    {
        boost = true;
    }

    public void UnboostPlayer()
    {
        boost = false;
    }

    public void SetCheckpoint(CheckPoint checkPoint)
    {
        gameState.SetCurrentCheckPoint(checkPoint);
    }

    protected override void Awake()
    {
        base.Awake();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    protected override void Start()
    {
        base.Start();
        MeshCollider collider = GetComponent<MeshCollider>();
        distanceToGround = collider.bounds.extents.y;
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
        if (brakingPressures.Count > 0)
        {
            DoStop();
        } else if (boost)
        {
            Rigidbody prb = player.GetComponent<Rigidbody>();
            Vector3 force = prb.velocity.normalized * 1;
            prb.AddForce(force, ForceMode.Impulse);
        }
        
    }

    private void DoStop()
    {
        if (!IsGrounded()) return;

        float pressureSumation = 0f;
        foreach (VRTK_ControllerEvents controller in brakingPressures)
        {
           pressureSumation += controller.triggerAxis.x;
           // pressureSumation += 1;
        }
        float smoothTime = minBrakeTime + ((maxBrakeTime - minBrakeTime) * (1 - (pressureSumation/2.0f)));
        Rigidbody prb = player.GetComponent<Rigidbody>();
        Mathf.SmoothDamp(0.0f, 1.0f, ref strength, smoothTime);
        Vector3 f = -(prb.mass * prb.velocity) * strength;
        Vector3 t= -(prb.mass * prb.angularVelocity) * strength;
        prb.AddTorque(t, ForceMode.Impulse);
        prb.AddForce(f, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
    }

    private void DoResetPlayer()
    {
        reset = false;
        player.transform.position =gameState.GetCheckPointPosition();

        Rigidbody prb = player.GetComponent<Rigidbody>();
        prb.velocity = Vector3.zero;
        prb.angularVelocity = Vector3.zero;
        gameState.SetUnready();
    }
}
