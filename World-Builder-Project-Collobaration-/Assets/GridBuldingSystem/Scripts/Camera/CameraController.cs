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
    [SerializeField] AnimationCurve moveSpeedCurve = AnimationCurve.Linear(0, 0.5f, 1, 1);
    [SerializeField] private float acceleration = 10f;
    [SerializeField] private float deceleration = 10f;
    Vector3 velocity = Vector3.zero;

    [Space(10)]
    [SerializeField] private float sprintSpeedMultiplier = 2f;

    [Space(10)]
    [SerializeField] private float edgeScrollingMargin = 15f;

    [Header("Zoom")]
    [SerializeField] private float zoomSpeed = 0.5f;
    [SerializeField] private float zoomSmoothing = 10f;

    Vector2 moveInput;
    Vector2 lookInput;
    public Vector2 scrollInput;
    Vector2 edgeScrollInput;
    float decelerationFactor = 1f;
    bool sprintInput = false;
    bool middleClickInput = false;

    float currentZoomSpeed = 0f;

    public float ZoomLevel //value between 0 (zoom in) and 1 (zoom out)
    {
        get
        {
            float axisValue = orbitalFollow.RadialAxis.Value;
            return Mathf.InverseLerp(orbitalFollow.RadialAxis.Range.x, orbitalFollow.RadialAxis.Range.y, axisValue);
        }
    }

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

    void OnSprint(InputValue value)
    {
        sprintInput = value.isPressed;
    }
    #endregion

    #region Unity Methods

    private void OnEnable()
    {
       GridOfPrefabs.OnGridReady += SetCameraTarget;
    }
    private void OnDisable()
    {
        GridOfPrefabs.OnGridReady -= SetCameraTarget;
    }

    void LateUpdate()
    {
        float deltaTime = Time.unscaledDeltaTime;

        if (!Application.isEditor)
        {
            UpdateEdgeScrollInput();

        }
        UpdateOrbit(deltaTime);
        UpdateMovement(deltaTime);
        UpdateZoom(deltaTime);
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

        Vector3 inputVector = new Vector3(moveInput.x + edgeScrollInput.x, 0, moveInput.y + edgeScrollInput.y);
        inputVector.Normalize();

        float zoomMultiplier = moveSpeedCurve.Evaluate(ZoomLevel);
        Vector3 targetVelocity = inputVector * moveSpeed * zoomMultiplier;

        float sprintFactor = sprintInput ? sprintSpeedMultiplier : 1f;

        if (sprintInput)
        {
            targetVelocity *= sprintSpeedMultiplier;
        }

        if (inputVector.sqrMagnitude > 0.01f)
        {
            velocity = Vector3.MoveTowards(velocity, targetVelocity, acceleration * sprintFactor * deltaTime);

            decelerationFactor = sprintInput ? sprintSpeedMultiplier : 1f;
        }
        else
        {
            velocity = Vector3.MoveTowards(velocity, Vector3.zero, deceleration * decelerationFactor * deltaTime);
        }

        Vector3 motion = velocity * deltaTime;
        cameraTarget.position += forward * motion.z + right * motion.x;

        if(velocity.sqrMagnitude < 0.01f)
        {
            decelerationFactor = 1f;
        }
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

        //horizontalAxis.Value = Mathf.Clamp(horizontalAxis.Value, horizontalAxis.Range.x, horizontalAxis.Range.y);
        verticalAxis.Value = Mathf.Clamp(verticalAxis.Value, verticalAxis.Range.x, verticalAxis.Range.y);

        orbitalFollow.HorizontalAxis = horizontalAxis;
        orbitalFollow.VerticalAxis = verticalAxis;
    }

    void UpdateZoom(float deltaTime)
    {

        InputAxis axis = orbitalFollow.RadialAxis;
        float targetZoomSpeed = 0;

        if(Mathf.Abs(scrollInput.y) >= 0.01f)
        {
            targetZoomSpeed = scrollInput.y * zoomSpeed;
        }

        currentZoomSpeed = Mathf.Lerp(currentZoomSpeed, targetZoomSpeed, zoomSmoothing * deltaTime);
        axis.Value -= currentZoomSpeed;
        axis.Value = Mathf.Clamp(axis.Value, axis.Range.x, axis.Range.y);
        orbitalFollow.RadialAxis = axis;    
    }

    void UpdateEdgeScrollInput()
    {
        Vector2 mousePosition = Mouse.current.position.ReadValue();
        edgeScrollInput = Vector2.zero;

        if (mousePosition.x <= edgeScrollingMargin)
        {
            edgeScrollInput.x = -1;
        }
       else if(mousePosition.x >= Screen.width - edgeScrollingMargin)
        {
            edgeScrollInput.x = 1;
        }
        else
        {
            edgeScrollInput.x = 0;
        }
        if(mousePosition.y <= edgeScrollingMargin)
        {
            edgeScrollInput.y = -1;
        }
        else if(mousePosition.y >= Screen.height - edgeScrollingMargin)
        {
            edgeScrollInput.y = 1;
        }
        else
        {
            edgeScrollInput.y = 0;
        }
    }


    void SetCameraTarget()
    {
        cameraTarget.position = GridOfPrefabs.Instance.GetCenterOnGridSurface();
    }

    #endregion
}
