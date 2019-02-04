using UnityEngine;

public class FPController : MonoBehaviour {

    public static FPController Instance;

	private bool _hasJumped;
    public bool _doorLook;

	public float speed;
	public float sensitivity;
	public float jumpForce = 4f;
	private float _runSpeed;
	private float _oldSpeed;

	private float _FBmove;
	private float _RLmove;
	private float _xRot;
	private float _yRot;
	private float _vertVelocity;

	private Vector3	_startPoint;

	private GameObject _camera;
    public GameObject robotGo;

    private Ray _ray;
    private RaycastHit _hit;

	public CharacterController player;
    public static bool facingRobot, _paused;

	// Use this for initialization
	private void Awake () {
        _camera = Camera.main.gameObject;
        _startPoint = transform.position;
		_hasJumped = false;
        _paused = false;
		_runSpeed = speed * 2;
		_oldSpeed = speed;

        Instance = this;
	}

    private void Update() {

        // if (SceneManager.GetActiveScene().buildIndex == 0)
        //    return;

        _ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));

        if (Physics.Raycast(_ray, out _hit, 3.5f)) {
            Robot.Instance.facingPlayer = _hit.transform.CompareTag("HelperRobot");
            facingRobot = _hit.transform.CompareTag("HelperRobot");
        } else {
            Robot.Instance.facingPlayer = false;
            facingRobot = false;
        }

    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (_paused == false) {

            if (MouseBehaviour.Instance.cameraLock) return;

            //Getting input data from WASD, arrow keys and input from a controller
            _FBmove = Input.GetAxis("Vertical") * speed;
            _RLmove = Input.GetAxis("Horizontal") * speed;

            //Getting an x and y input for the mouse so that the player can look around
            _xRot = Input.GetAxis("Mouse X");// * sensitivity;
            _yRot += Input.GetAxis("Mouse Y");// * sensitivity;

            //Clamping the input for the Y axis so that the player can't infinitely spin the playerCamera up and down
            _yRot = Mathf.Clamp(_yRot, -70, 70);

            //Assigning the WASD input to a Vector 3 variable so that it can be used in a character controller
            var movement = new Vector3(_RLmove, _vertVelocity, _FBmove);
            movement = transform.rotation * movement;
            player.Move(movement * Time.deltaTime);

            //This respawns the player if they fall off of the world
            if (transform.position.y < -10)
            {
                transform.position = _startPoint;
            }

            //This alocates the player's roatation to the mouse input so that when the player moves the mouse left and right,
            //the character will move left and right and so will the playerCamera.
            //Also, the movement on the Y axis is set to only the playerCamera as we don't want the whole player rotating when looking up and down

            //if (Input.GetMouseButton(1)){
            transform.Rotate(0, _xRot, 0);
            _camera.transform.Rotate(0, _xRot, 0, Space.Self);
            _camera.transform.localEulerAngles = new Vector3(-_yRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
            //}

            //This simply makes sure that the playerCamera has the same position as the player
            //EDIT: Changed the Y position to that the camera is actually near where the head would be and not in the middle of the character.
            _camera.transform.position = new Vector3(transform.position.x, transform.position.y + 0.6f, transform.position.z);

            //When the player presses the run button, in this case Left Shift, it doubles the character's speed variable.
            //Letting go causes the speed to return to what it was when the scene started.
            speed = Input.GetButton("Run") ? _runSpeed : _oldSpeed;

       
            #region Jumping
            //Checkes to see if the spacebar has been pressed and actives the bool that will cause the player to jump
            if (Input.GetButton("Jump"))
                _hasJumped = true;      
       
            //Checkes to see if the player is grounded and also if the player has pressed the spacebar.
            //If the player is grounded and hasn't jumped, it will enforce gravity.
            //If the player is grounded but has jumped, it will enforce upward force equal to that of jumpForce
            //If the player isn't grounded, gravity will be continuously be enforced until the player is grounded again.
            if (player.isGrounded) {
                _vertVelocity = _hasJumped == false ? Physics.gravity.y : jumpForce;
                if (_hasJumped)
                    _startPoint = transform.position;
            } else {
                _vertVelocity += Physics.gravity.y * Time.deltaTime;
                _hasJumped = false;
                _vertVelocity = Mathf.Clamp(_vertVelocity, -50, jumpForce);
            }
            #endregion
        }
    }

}
