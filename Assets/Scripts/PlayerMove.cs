using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{

    [SerializeField] float _speed;
    [SerializeField] float DeploymentHeight;
    public GameObject _camera;

    [SerializeField] private float cx;
    [SerializeField] private float cy;
    [SerializeField] private float mouseSensivity;

    [SerializeField] private GameObject hook;
    [SerializeField] private LineRenderer rope;

    public States state;
    private Vector3 lastPos;
    private Vector3 currentPos;
    [SerializeField] private float jetpackReloadSpeed;
    [SerializeField] private float climbSpeed;
    private Rigidbody rb;
    private float releaseTimer;
    private float cooldownTimer;
    private float collisionCount;
    private List<GameObject> collisionList;
    [SerializeField] private float cooldownTime;
    [SerializeField] private float jumpPower;
    [SerializeField] private float jetpackPower;
    [SerializeField] private float jetpackHorizontalPower;
    [SerializeField] private float gravityPower;
    private Vector3 lastSpeed;
    public bool hookGrabbed;
    private bool wasGrabbed;
    [SerializeField] private float hookMaxLenght;
    private float hookLenght;
    [SerializeField] private float hookSpeed;
    private bool hookActive;
    private Vector3 hookDirection;
    private Vector3 currentSpeed;
    [SerializeField] private float jetpackFuel;
    [SerializeField] private float maxFuel;
    [SerializeField] private bool _canJump;

    void Start()
    {
        collisionCount = 0;
        collisionList = new List<GameObject>();
        cooldownTimer = Time.realtimeSinceStartup - cooldownTime;
        rb = GetComponent<Rigidbody>();
        hookGrabbed = false;
        hookActive = false;
        wasGrabbed = false;
        currentSpeed = Vector3.zero;
        _canJump = false;
        jetpackFuel = maxFuel;
        Cursor.visible = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _canJump = true;
            collisionCount += 1;
            collisionList.Add(collision.gameObject);
            currentPos = collisionList[0].transform.position;
            lastPos = currentPos;
        }
    }

    void OnCollisionExit(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            if (collisionCount == 1)
            {
                _canJump = false;
            }
            collisionCount -= 1;
            collisionList.RemoveAt(0);
            currentPos = collisionList[0].transform.position;
            lastPos = currentPos;
        }
    }

    void Update()
    {
        cx += Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, cx, 0);
        cy -= Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        cy = Mathf.Clamp(cy, -90, 90);
        _camera.transform.rotation = Quaternion.Euler(cy, cx, 0);

        print($"{_canJump}, {collisionCount}");
        if (Input.GetKey(KeyCode.Space))
        {
            if (_canJump)
            {
                Jump();
            }
        }

        if (_canJump)
        {
            currentPos = collisionList[0].transform.position;
            gameObject.transform.position += currentPos - lastPos;
            lastPos = currentPos;
            currentSpeed = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
            }
            if (Input.GetKey(KeyCode.S))
            {
                currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
            }
            if (Input.GetKey(KeyCode.A))
            {
                currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
            }
            if (Input.GetKey(KeyCode.D))
            {
                currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
            }
            
            currentSpeed = _speed * currentSpeed.normalized;
            rb.velocity = currentSpeed;

            if (currentSpeed != Vector3.zero)
            {
                state = States.walking;
            }
            else
            {
                state = States.staying;
            }
            if (jetpackFuel < maxFuel)
            {
                jetpackFuel += Time.deltaTime * jetpackReloadSpeed;
            }
        }
        else
        {
            rb.AddForce(0, -gravityPower, 0, ForceMode.Impulse);
            if (jetpackFuel > 0)
            {
                JetpackFly();
            }
        }

        //print($"{Input.GetMouseButtonDown(1)} && {!hookActive} && {Time.realtimeSinceStartup - cooldownTimer > cooldownTime}");
        if (Input.GetMouseButtonDown(1) && !hookActive && Time.realtimeSinceStartup - cooldownTimer > cooldownTime)
        {
            hookActive = true;
            hook.SetActive(true);
            rope.gameObject.SetActive(true);
            hookDirection = _camera.transform.forward;
            hook.transform.position = gameObject.transform.position + hookDirection * hookSpeed * Time.deltaTime;
            hook.transform.rotation = _camera.transform.rotation;
            rope.SetPosition(0, gameObject.transform.position);
            rope.SetPosition(1, hook.transform.position);
            releaseTimer = Time.realtimeSinceStartup;
            state = States.releasing;
        }
        if (hookActive)
        {
            rope.SetPosition(0, gameObject.transform.position);
            rope.SetPosition(1, hook.transform.position);
            if (hookGrabbed != wasGrabbed)
            {
                hookLenght = (hook.transform.position - gameObject.transform.position).magnitude;
                wasGrabbed = true;
                state = States.grappling;
            }
            if (!hookGrabbed)
            {
                hook.transform.position += hookDirection * hookSpeed * Time.deltaTime;
                if ((hook.transform.position - gameObject.transform.position).magnitude > hookMaxLenght)
                {
                    rope.gameObject.SetActive(false);
                    hook.SetActive(false);
                    hookActive = false;
                    cooldownTimer = Time.realtimeSinceStartup;
                }
            }
            else
            {
                if (Input.GetKey(KeyCode.Q))
                {
                    hookLenght -= climbSpeed * Time.deltaTime;
                }
                //print($"{hookMaxLenght}, {hookLenght}, {(hook.transform.position - gameObject.transform.position).magnitude}, {_canJump}, {collisionCount}");
                if ((hook.transform.position - gameObject.transform.position).magnitude >= hookLenght)
                {
                    rb.velocity += Vector3.Slerp((hook.transform.position + (gameObject.transform.position - hook.transform.position).normalized * hookLenght - gameObject.transform.position), rb.GetPointVelocity(Vector3.zero), 0.05f) * 0.25f;
                }
            }
            if (Input.GetMouseButtonDown(1) && Time.realtimeSinceStartup - releaseTimer > 1)
            {
                hookGrabbed = false;
                hookActive = false;
                wasGrabbed = false;
                rope.gameObject.SetActive(false);
                hook.SetActive(false);
                hook.GetComponent<Rigidbody>().isKinematic = false;
                cooldownTimer = Time.realtimeSinceStartup;
            }
        }
    }

    void Jump()
    {
        _canJump = false;
        rb.AddForce(0, jumpPower, 0, ForceMode.Impulse);
    }

    void JetpackFly()
    {
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity -= currentSpeed;
            lastSpeed = currentSpeed;
            currentSpeed = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                //currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
                rb.AddForce(_camera.transform.forward.x * jetpackHorizontalPower * Time.deltaTime, 0, _camera.transform.forward.z * jetpackHorizontalPower * Time.deltaTime, ForceMode.Impulse);
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                //currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
                rb.AddForce(-_camera.transform.forward.x * jetpackHorizontalPower * Time.deltaTime, 0, -_camera.transform.forward.z * jetpackHorizontalPower * Time.deltaTime, ForceMode.Impulse);
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                //currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
                rb.AddForce(_camera.transform.forward.z * jetpackHorizontalPower * Time.deltaTime, 0, _camera.transform.forward.x * jetpackHorizontalPower * Time.deltaTime, ForceMode.Impulse);
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                //currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
                rb.AddForce(-_camera.transform.forward.z * jetpackHorizontalPower * Time.deltaTime, 0, -_camera.transform.forward.x * jetpackHorizontalPower * Time.deltaTime, ForceMode.Impulse);
                jetpackFuel -= 5 * Time.deltaTime;
            }
            currentSpeed = jetpackHorizontalPower * currentSpeed.normalized;
            if (currentSpeed == Vector3.zero)
            {
                state = States.staying;
                currentSpeed = lastSpeed;
            }
            else
            {
                state = States.flying;
            }
            rb.velocity += currentSpeed;
        }

        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(0, jetpackPower * Time.deltaTime, 0, ForceMode.Impulse);
            jetpackFuel -= 5f * Time.deltaTime;
            state = States.flying;
        }
    }
}

public enum States
{
    staying,
    walking,
    flying,
    releasing,
    grappling
}