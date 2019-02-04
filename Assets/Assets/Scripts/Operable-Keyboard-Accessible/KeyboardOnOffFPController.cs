using UnityEngine;
using UnityEngine.UI;

public class KeyboardOnOffFPController : MonoBehaviour {

    public static KeyboardOnOffFPController Instance;

    public bool hasJumped;
    public float speed, sensitivity;
    public float jumpForce = 4f;
    private float runSpeed, oldSpeed;
    private float FBmove, RLmove;
    private float xRot, yRot;
    private float vertVelocity;
    private Vector3 startPoint;
    public GameObject playerCamera, robotGo;
    public GameObject aobj, sobj, mouseobj, spaceobj, cobj, iobj, bobj, lobj, tobj, yobj;
    public CharacterController player;
    public bool a, s, c, i, b, l, t, y;
    public static bool space, mouse;
    public bool _doorLook;
    RaycastHit hit;
	public bool _keyboardOnly, _mouseOnly;
	private Quaternion _rotateLeft;
	private Quaternion _rotateRight;
    private Vector3 _currentCameraPos;
    private Vector3 _currentPlayerPos;
    public InputField passwordInput;
    public static int Deaths;
    public bool facingRobot;

    // Use this for initialization
    public void Start() {
        Instance = this;

        startPoint = transform.position;
        hasJumped = false;
        runSpeed = speed * 2;
        oldSpeed = speed;
        mouse = false;
        space = false;
        Debug.DrawRay(transform.position, transform.forward, Color.green, 5);

    }


    // Update is called once per frame
    private void FixedUpdate() {
        if (MouseBehaviour.Instance.cameraLock) return;
        
            if (c)
            passwordInput.characterLimit = 2;
        if (c && i) 
            passwordInput.characterLimit = 7;
        if (c && i && b)
            passwordInput.characterLimit = 8;
        if (c && i && b && l)
            passwordInput.characterLimit = 10;
        if (c && i && b && l && t)
            passwordInput.characterLimit = 12;
        if (c && i && b && l && t && y)
            passwordInput.characterLimit = 13;


        if (!passwordDoor.paused){

        _currentCameraPos = playerCamera.transform.eulerAngles;
        _currentPlayerPos = transform.eulerAngles;

        if (player.transform.position.y <= -1){
            transform.position = startPoint;
            Deaths = Deaths +1;
            keyboardDialogueSystem._deadDialogue();
        }
        
        if (Physics.Raycast (transform.position, transform.forward, out hit) && hit.transform.tag == "Door") {
            _doorLook = true;
        }
        else
        _doorLook = false;


          
                //Getting input data from WASD, arrow keys and input from a controller
                if (!_mouseOnly)
                {
                    FBmove = Input.GetAxis("Vertical") * speed;
                    RLmove = Input.GetAxis("Horizontal") * speed;

                }

                if (_mouseOnly)
                {
                    if (Input.GetMouseButton(0))
                    {
                        FBmove = 1 * runSpeed;
                    }
                    else
                        FBmove = 0;
                }
            

            if (!a)
           RLmove = Mathf.Clamp(RLmove, 0, 6);

        if (!s)
            FBmove = Mathf.Clamp(FBmove, 0, 6);

                //Getting an x and y input for the mouse so that the player can look around
            xRot = Input.GetAxis("Mouse X");// * sensitivity;
            yRot += Input.GetAxis("Mouse Y");// * sensitivity;
            

            //Clamping the input for the Y axis so that the player can't infinitely spin the playerCamera up and down
        yRot = Mathf.Clamp(yRot, -70, 70);

        //Assigning the WASD input to a Vector 3 variable so that it can be used in a character controller
        Vector3 movement = new Vector3(RLmove, vertVelocity, FBmove);
        movement = transform.rotation * movement;
        player.Move(movement * Time.deltaTime);

        //This respawns the player if they fall off of the world
        if (transform.position.y < -10) {
            transform.position = startPoint;
        }

		if (_keyboardOnly) {
			if (Input.GetKey (KeyCode.Q)) {
				_currentPlayerPos.y -= 2;
                _currentCameraPos.y -= 2;
			}
            if (Input.GetKey (KeyCode.E)){
                _currentPlayerPos.y += 2;
                _currentCameraPos.y += 2;
            }
            playerCamera.transform.eulerAngles = new Vector3(1, _currentCameraPos.y, _currentCameraPos.z);
            transform.eulerAngles = new Vector3(_currentPlayerPos.x, _currentPlayerPos.y, _currentPlayerPos.z);
		}

        //This alocates the player's roatation to the mouse input so that when the player moves the mouse left and right,
        //the character will move left and right and so will the playerCamera.
        //Also, the movement on the Y axis is set to only the playerCamera as we don't want the whole player rotating when looking up and down
		if (mouse && !_keyboardOnly) {
            transform.Rotate(0, xRot, 0);
		    if (MouseBehaviour.Instance.cameraLock) return;
            playerCamera.transform.Rotate(0, xRot, 0);
            playerCamera.transform.localEulerAngles = new Vector3(-yRot, transform.localEulerAngles.y, transform.localEulerAngles.z);
        }

        //This simply makes sure that the playerCamera has the same position as the player
        playerCamera.transform.position = transform.position;

        //When the player presses the run button, in this case Left Shift, it doubles the character's speed variable.
        //Letting go causes the speed to return to what it was when the scene started.
        speed = Input.GetButton("Run") ? runSpeed : oldSpeed;
        }
    }

    private void Update() {
        RaycastHit hit;
        var ray = Camera.main.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0f));
        if (Physics.Raycast(ray, out hit, Mathf.Infinity))
        {
            Robot.Instance.facingPlayer = hit.transform.CompareTag("HelperRobot");
            facingRobot = hit.transform.CompareTag("HelperRobot");
        }

        if (Input.GetKeyDown(KeyCode.T))
        {
            Robot.Instance.facingPlayer = !Robot.Instance.facingPlayer;
            Debug.Log(string.Format("The player is {0} facing the robot", Robot.Instance.facingPlayer ? "now" : "no longer"));
        }



        if (!passwordDoor.paused){
        //Checkes to see if the spacebar has been pressed and actives the bool that will cause the player to jump
        if (space) {
            if (!_mouseOnly){
                if (Input.GetButton("Jump")) {
                    if (RobotDialogue.Instance.IsDialoguePlaying()) return;//stop the player from jumping if they have dialogue.
                        hasJumped = true;
                }
            }
            if (_mouseOnly){
                if (Input.GetMouseButton(1)){
                    hasJumped = true;
                }
            }
        }

        //Checkes to see if the player is grounded and also if the player has pressed the spacebar.
        //If the player is grounded and hasn't jumped, it will enforce gravity.
        //If the player is grounded but has jumped, it will enforce upward force equal to that of jumpForce
        //If the player isn't grounded, gravity will be continuously be enforced until the player is grounded again.
        if (player.isGrounded) {
            if (hasJumped) {
                vertVelocity = jumpForce;
                startPoint = transform.position;
                //print("Has jumped"); --disabling this to stop me from going insane when trying to debug through the console.
            } else {
                vertVelocity = Physics.gravity.y;
            }
        } else {
            vertVelocity += Physics.gravity.y * Time.deltaTime;
        }
        hasJumped = false;
        vertVelocity = Mathf.Clamp(vertVelocity, -50, jumpForce);
    } 
    }


    private void OnTriggerEnter(Collider other){

        switch (other.tag) {
            case "a":
                a = true;
                Destroy(aobj);
                break;
            case "s":
                s = true;
                Destroy(sobj);
                break;
            case "mouse":
                mouse = true;
                Destroy(mouseobj);
                break;
            case "space":
                space = true;
                Destroy(spaceobj);
                break;
            case "keyboardOnly":
                _keyboardOnly = true;
                break;
            case "mouseOnly":
                _mouseOnly = true;
                startPoint = transform.position;
                break;
            case "normal":
                _keyboardOnly = false;
                _mouseOnly = false;
                break;
            case "c":
                c = true;
                Destroy(cobj);
                break;
            case "i":
                i = true;
                Destroy(iobj);
                break;
            case "b":
                b = true;
                Destroy(bobj);
                break;
            case "l":
                l = true;
                Destroy(lobj);
                break;
            case "t":
                t = true;
                Destroy(tobj);
                break;
            case "y":
                y = true;
                Destroy(yobj);
                break;
            default:
                Debug.Log(string.Format("No process defined for {0}", other.tag));
                break;
        }
    }

}
