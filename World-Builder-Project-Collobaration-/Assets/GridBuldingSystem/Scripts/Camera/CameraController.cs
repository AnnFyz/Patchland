using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Cinemachine;

public class CameraController : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private Transform cameraTarget;
    [SerializeField] private CinemachineOrbitalFollow orbitalFollow;

    [Header("Orbit")]
    [SerializeField] private float orbitSensitivity = 0.5f;
    [SerializeField] private float orbitSmoothing = 10f;

    [Header("Movement")]
    [SerializeField] private float moveSpeed = 20f;
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    Vector3 velocity = Vector3.zero;


    Vector2 moveInput;
    Vector2 lookInput;
    Vector2 scrollInput;
    bool middleClickInput = false;

    #region Input
    void OnMove(InputValue value)
    {
        moveInput = value.Get<Vector2>();
    }

    void OnLook(InputValue value)
    {
        lookInput = value.Get<Vector2>();
    }

    void OnScrollWheel(InputValue value)
    {
        scrollInput = value.Get<Vector2>();
    }

    void OnMiddleClick(InputValue value)
    {
        middleClickInput = value.isPressed;
    }
    #endregion

    #region Unity Methods
    void Update()
    {
        float deltaTime = Time.unscaledDeltaTime;
        UpdateOrbit(deltaTime);
        UpdateMovement(deltaTime);
    }
    #endregion

    #region Control Methods

    void UpdateMovement(float deltaTime)
    {
        // Forward movement
        Vector3 forward = Camera.main.transform.forward;
        forward.y = 0;
        forward.Normalize();

        Vector3 right = Camera.main.transform.right;
        right.y = 0;
        right.Normalize();

        Vector3 targetVelocity = new Vector3(moveInput.x, 0, moveInput.y) * moveSpeed;

        if(moveInput.sqrMagnitude > 0.01f)
        {
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * deltaTime);
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * deltaTime);
        }

        Vector3 motion = velocity * deltaTime;
        cameraTarget.position += forward * motion.z + right * motion.x;
    }

    void UpdateOrbit(float deltaTime)
    {
        Vector2 orbitInput = lookInput * (middleClickInput ? 1f : 0f);
        orbitInput *= orbitSensitivity;

        InputAxis horizontalAxis = orbitalFollow.HorizontalAxis;
        InputAxis verticalAxis = orbitalFollow.VerticalAxis;

        //horizontalAxis.Value += orbitInput.x;
        //verticalAxis.Value -= orbitInput.y;

        horizontalAxis.Value = Mathf.Lerp(horizontalAxis.Value, horizontalAxis.Value + orbitInput.x, orbitSmoothing * deltaTime);
        verticalAxis.Value = Mathf.Lerp(verticalAxis.Value, verticalAxis.Value - orbitInput.y, orbitSmoothing * deltaTime);

        horizontalAxis.Value = Mathf.Clamp(horizontalAxis.Value, horizontalAxis.Range.x, horizontalAxis.Range.y);
        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, verticalAxis.Range.x, verticalAxis.Range.y);

        orbitalFollow.HorizontalAxis = horizontalAxis;
        orbitalFollow.VerticalAxis = verticalAxis;

    }
    #endregion
}
