using UnityEngine;
using System.Collections;
using VRTK;

namespace Game
{
    public class PlayerSphere : VRTK_InteractableObject
    {
        [Header("Game Objects", order = 4)]
        public GameObject player;
        public GameState gameState;
        public int touchForceMultiplier = 100;
        public ushort hapticFeedbackStrength = 500;
        public float maxBrakeTime = 1000.0f;
        public float minBrakeTime = 20.0f;
        public float boostMultiplier = 1.0f;

        ArrayList brakingPressures = new ArrayList();
        ArrayList touchingControllers = new ArrayList();
        ArrayList deadControllers = new ArrayList();
        Hashtable deadControllerMoveDistances = new Hashtable();
        Hashtable deadControllerPreviousVector3s = new Hashtable();

        VRTK_ControllerActions controllerActions;
        private float strength = 0;
        private float distanceToGround;
        private bool reset = false;
        private bool boost = false;
        private float maxBoostMagnitude;
        private float minDeadControllerMoveDistance = 0.25f;
        private float highestVelocityMagnitude;

        public override void StartTouching(GameObject touchingObject)
        {
            base.StartTouching(touchingObject);
            if (!gameState.GetReady()) return;
            if (!touchingControllers.Contains(touchingObject) && !deadControllers.Contains(touchingObject))
            {
                touchingControllers.Add(touchingObject);
            }
        }

        public override void StopTouching(GameObject previousTouchingObject)
        {
            base.StopTouching(previousTouchingObject);
            if (touchingControllers.Contains(previousTouchingObject))
                touchingControllers.Remove(previousTouchingObject);
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
            BoostPlayer(-1);
        }

        public void BoostPlayer(float boostPower)
        {
            boost = true;
            maxBoostMagnitude = boostPower;
            highestVelocityMagnitude = 0;
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
          //  interactableRigidbody.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }

        protected void Start()
        {
            MeshCollider collider = GetComponent<MeshCollider>();
            distanceToGround = collider.bounds.extents.y;
        }

        protected override void Update()
        {
            base.Update();
            ProcessDeadControllers();
        }

        protected override void FixedUpdate()
        {
            base.FixedUpdate();
            if (reset)
            {
                DoResetPlayer();
            }
            Rigidbody prb = player.GetComponent<Rigidbody>();
            if (!gameState.GetReady())
            {
                prb.velocity = Vector3.zero;
                prb.angularVelocity = Vector3.zero;
            }

            foreach (GameObject touchingObject in touchingControllers)
            {
                controllerActions = touchingObject.GetComponent<VRTK_ControllerActions>();
                Vector3 impact = touchingObject.transform.localPosition.normalized * touchForceMultiplier;
                prb.AddForce(new Vector3(impact.x, 0, impact.z));
                controllerActions.TriggerHapticPulse(hapticFeedbackStrength, 0.1f, 0.01f);
                deadControllers.Add(touchingObject);
            }
            touchingControllers.Clear();

            if (brakingPressures.Count > 0)
            {
                DoStop();
            }
            else if (boost && (maxBoostMagnitude == -1 || prb.velocity.magnitude < maxBoostMagnitude))
            {
                float multiplier = boostMultiplier;
                if (maxBoostMagnitude != -1 && maxBoostMagnitude - prb.velocity.magnitude < 1) multiplier = maxBoostMagnitude - prb.velocity.magnitude;
                Vector3 force = prb.velocity.normalized * multiplier;
                prb.AddForce(force, ForceMode.Impulse);
            }
            else if (boost && prb.velocity.magnitude > maxBoostMagnitude)
            {
                prb.velocity = prb.velocity.normalized * maxBoostMagnitude;

            }

            if (prb.velocity.magnitude >= highestVelocityMagnitude)
            {
                highestVelocityMagnitude = prb.velocity.magnitude;
                Debug.Log("highestVelocityMagnitude:" + highestVelocityMagnitude);
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
            float smoothTime = minBrakeTime + ((maxBrakeTime - minBrakeTime) * (1 - (pressureSumation / 2.0f)));
            Rigidbody prb = player.GetComponent<Rigidbody>();
            Mathf.SmoothDamp(0.0f, 1.0f, ref strength, smoothTime);
            Vector3 f = -(prb.mass * prb.velocity) * strength;
            Vector3 t = -(prb.mass * prb.angularVelocity) * strength;
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
            player.transform.position = gameState.GetCheckPointPosition();

            Rigidbody prb = player.GetComponent<Rigidbody>();
            prb.velocity = Vector3.zero;
            prb.angularVelocity = Vector3.zero;
            gameState.SetUnready();
        }

        private void ProcessDeadControllers()
        {
            ArrayList aliveControllers = new ArrayList();
            foreach (GameObject deadController in deadControllers)
            {
                if (!deadControllerMoveDistances.ContainsKey(deadController))
                {
                    deadControllerMoveDistances[deadController] = 0f;
                    deadControllerPreviousVector3s[deadController] = deadController.transform.localPosition;
                }
                else
                {
                    Vector3 newPosition = deadController.transform.localPosition;
                    float currentDistance = (float)deadControllerMoveDistances[deadController];
                    float newDistance = currentDistance + Mathf.Abs(Vector3.Distance(newPosition, (Vector3)deadControllerPreviousVector3s[deadController]));
                    deadControllerMoveDistances[deadController] = newDistance;
                    deadControllerPreviousVector3s[deadController] = newPosition;
                    if (newDistance > minDeadControllerMoveDistance)
                    {
                        //Debug.Log("newDistance = " + newDistance);
                        aliveControllers.Add(deadController);
                    }
                }
            }
            foreach (GameObject aliveController in aliveControllers)
            {
                deadControllers.Remove(aliveController);
                deadControllerMoveDistances.Remove(aliveController);
                deadControllerPreviousVector3s.Remove(aliveController);
            }
        }
    }
}