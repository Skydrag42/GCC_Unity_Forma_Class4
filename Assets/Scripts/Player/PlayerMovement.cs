using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    // Reference to the component managing the player's movement and physics.
    // Must be initialized either through the inspector or via script.
    public CharacterController Controller;

    [Header("Movement")]
    [Tooltip("Default planar speed, in m.s^-1")] public float speed = 12f;
    [Tooltip("Running planar speed = speed * runSpeedModifier.")] public float runSpeedModifier = 1.5f;
    [Tooltip("Max height of a jump, in m.")] public float jumpHeight = 1.5f;

    [Tooltip("Gravity force, in m.s^-2")] public float gravity = -9.81f;

    private Vector2 moveInput;
    private Vector3 velocity = Vector3.zero;

    private bool jump = false;
    private bool running = false;
    private bool canRun = false;

    private bool canMove = true;

    [Header("Ground checks")]
    [Tooltip("The origin of the sphere used for the ground check.")] public Transform groundCheck;
    [Tooltip("The radius of the sphere.")] public float checkRadius = 0.2f;
    [Tooltip("The layers checked for the ground check.")] public LayerMask groundMask;

    private bool isGrounded = false;
    private RaycastHit[] groundCollisions;

    // Awake is called once, when the gameObject is initialized (woken up).
	private void Awake()
	{
        // We initialize the groundCollisions buffer (with only one slot as we should not encounter multiple ground collsions,
        // and we only care about the first one. This could potentially cause some problems in specific situations with different
        // ground orientations).
        groundCollisions = new RaycastHit[1];
	}

    // Start is called once, just before the first frame update.
	private void Start()
    {
        // we can do some initialisations here, like spawning the player at a specific position.
    }

    // Update is called every frame. It is the game's main loop.
    void Update()
    {
        if (!canMove)
            moveInput = Vector2.zero;

        // Running is only allowed while going forwards
        if (moveInput.y > 0)
            canRun = true;
        else
            canRun = false;


		// When running, we can adjust the field of view to give a sensation of speed
		if (canRun && running)
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 75, Time.deltaTime * 8f);
		else
			Camera.main.fieldOfView = Mathf.Lerp(Camera.main.fieldOfView, 60, Time.deltaTime * 8f);


		// We check for ground collisions by casting a sphere beneath the player against specific layers. If any collision is found,
        // we set the player to grounded and retrieve the ground normal vector.
		int collisions = Physics.SphereCastNonAlloc(groundCheck.position, checkRadius, Vector3.down, groundCollisions, 0.1f, groundMask);
        Vector3 groundNormal = Vector3.up;
        isGrounded = false;
        if (collisions > 0)
		{
            isGrounded = true;
            groundNormal = groundCollisions[0].normal;
		}
        

        // We need to translate the 2D movement inputs to the 3D world, where the Y axis is up, and we apply the necessary modifiers.
        Vector3 targetMove = moveInput.x * transform.right + moveInput.y * transform.forward;
        targetMove *= (running && canRun) ? runSpeedModifier : 1f;
        // We then project the movement vector to the ground plane for easier movement on slopes. that way, running down a slope
        // won't make the player feel like bouncing or floating.
        targetMove = Vector3.ProjectOnPlane(targetMove, groundNormal);
        // Finally, we multiply the movement vector by the speed and the time (in s) between this frame and the last one.
        // Doing the latter makes the movement speed consistent accross all framerates.
        Controller.Move(targetMove * speed * Time.deltaTime);


        // If the player requested a jump and ison the ground, we set the vertical velocity to a force that will match the desired height.
        if (jump && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
            jump = false;
        }
        // we then apply gravity to the vertical velocity
        // (since gravity is an acceleration, we multiply it by the delta time twice)
        velocity.y += gravity * Time.deltaTime;
        // and we move the player accordingly
        Controller.Move(velocity * Time.deltaTime);
    }

    /// <summary>
    /// Callback used to retrieve the movement inputs.
    /// </summary>
    /// <param name="context"></param>
    public void GetMove(InputAction.CallbackContext context)
    {
        moveInput = context.ReadValue<Vector2>();
    }

    /// <summary>
    /// Callback used to retrieve the jump input.
    /// /// </summary>
    /// <param name="context"></param>
    public void GetJump(InputAction.CallbackContext context)
    {
        if (context.performed && isGrounded && canMove)
            jump = true;
    }

    /// <summary>
    /// Callback used to retrieve the run input.
    /// </summary>
    /// <param name="context"></param>
    public void GetRun(InputAction.CallbackContext context)
    {
        if (context.performed)
            running = true;
        if (context.canceled)
            running = false;
    }

    /// <summary>
    /// Defines the movement state for the player.
    /// Can be used for exemple to prevent the player from moving while in a cinematic or when openig some UI.
    /// </summary>
    /// <param name="allowMovement"></param>
    public void SetMovement(bool allowMovement)
    {
        canMove = allowMovement;
    }
}
