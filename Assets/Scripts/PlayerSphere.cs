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

    Hashtable brakingPressures = new Hashtable(); 
    VRTK_ControllerActions controllerActions;
    private float strength = 0;
    private float distanceToGround;
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

    public void AddBraker(uint sender)
    {
        brakingPressures.Add(sender, 0);
    }

    public void RemoveBraker(uint sender)
    {
        brakingPressures.Remove(sender);
    }

    public void SetBrakerPressure(object sender, float pressure)
    {
        if (brakingPressures.ContainsKey(sender))
            brakingPressures[sender] = pressure;
    }

    public void ResetPlayer()
    {
        reset = true;
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
        }
    }

    private void DoStop()
    {
        if (!IsGrounded()) return;
        float pressureSumation = 0f;
        foreach (float pressure in brakingPressures.Values)
        {
            pressureSumation += pressure;
        }
        float smoothTime = minBrakeTime + ((maxBrakeTime - minBrakeTime) * pressureSumation/2.0f);
        Rigidbody prb = player.GetComponent<Rigidbody>();
        Mathf.SmoothDamp(0.0f, 1.0f, ref strength, smoothTime);
        Vector3 f = -(prb.mass * prb.velocity) * strength;
        Vector3 t= -(prb.mass * prb.angularVelocity) * strength;
        prb.AddForce(f, ForceMode.Impulse);
        prb.AddTorque(t, ForceMode.Impulse);
    }

    private bool IsGrounded()
    {
        return Physics.Raycast(transform.position, -Vector3.up, distanceToGround + 0.1f);
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
