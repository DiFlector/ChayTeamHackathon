using System.Collections;
using System.Collections.Generic;
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

    private float angle;
    private Rigidbody rb;
    private float releaseTimer;
    private float cooldownTimer;
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
        cooldownTimer = Time.realtimeSinceStartup - cooldownTime;
        rb = GetComponent<Rigidbody>();
        hookGrabbed = false;
        hookActive = false;
        wasGrabbed = false;
        currentSpeed = Vector3.zero;
        _canJump = true;
        jetpackFuel = maxFuel;
        Cursor.visible = false;
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Floor")
        {
            _canJump = true;
        }
    }

    void Update()
    {
        cx += Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        transform.rotation = Quaternion.Euler(0, cx, 0);
        cy -= Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;
        cy = Mathf.Clamp(cy, -90, 90);
        _camera.transform.rotation = Quaternion.Euler(cy, cx, 0);

        if (Input.GetKey(KeyCode.Space))
        {
            if (_canJump)
            {
                Jump();
            }
        }

        if (_canJump)
        {
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
        }
        else
        {
            rb.AddForce(0, -gravityPower, 0, ForceMode.Impulse);
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
        }
        if (hookActive)
        {
            rope.SetPosition(0, gameObject.transform.position);
            rope.SetPosition(1, hook.transform.position);
            if (hookGrabbed != wasGrabbed)
            {
                hookLenght = (hook.transform.position - gameObject.transform.position).magnitude;
                wasGrabbed = true;
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
                print($"{hookMaxLenght}, {hookLenght}, {(hook.transform.position - gameObject.transform.position).magnitude}");
                if ((hook.transform.position - gameObject.transform.position).magnitude >= hookLenght)
                {
                    /*
                    angle = Vector3.Angle(rb.velocity, hook.transform.position - gameObject.transform.position);
                    if (angle > 90)
                    {
                        rb.velocity = Vector3.Slerp(rb.velocity, hook.transform.position - gameObject.transform.position, angle - 90) +
                            hook.transform.position + (gameObject.transform.position - hook.transform.position).normalized * hookLenght - gameObject.transform.position;
                    }
                    else
                    {
                        rb.velocity += hook.transform.position + (gameObject.transform.position - hook.transform.position).normalized * hookLenght - gameObject.transform.position;
                    }
                    */
                    rb.velocity += hook.transform.position + (gameObject.transform.position - hook.transform.position).normalized * hookLenght - gameObject.transform.position;
                }
            }
            if (Input.GetMouseButtonDown(1) && Time.realtimeSinceStartup - releaseTimer > 1)
            {
                hookGrabbed = false;
                hookActive = false;
                wasGrabbed = false;
                rope.gameObject.SetActive(false);
                hook.SetActive(false);
                cooldownTimer = Time.realtimeSinceStartup;
            }
        }

        if (!_canJump)
        {
            if (jetpackFuel > 0)
            {
                JetpackFly();
            }
        }
        else
        {
            if (jetpackFuel < maxFuel)
            {
                jetpackFuel += Time.deltaTime;
            }
        }
    }

    private void FixedUpdate()
    {
        
    }

    void Jump()
    {
        _canJump = false;
        rb.AddForce(0, jumpPower, 0, ForceMode.Impulse);
    }

    void JetpackFly()
    {
        if (Input.GetKey(KeyCode.Space))
        {
            rb.AddForce(0, jetpackPower * Time.deltaTime, 0, ForceMode.Impulse);
            jetpackFuel -= 5f * Time.deltaTime;
        }
        if (Input.GetKey(KeyCode.LeftShift))
        {
            rb.velocity -= currentSpeed;
            lastSpeed = currentSpeed;
            currentSpeed = Vector3.zero;
            if (Input.GetKey(KeyCode.W))
            {
                currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.S))
            {
                currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * transform.rotation.eulerAngles.y), 0, Mathf.Cos(Mathf.Deg2Rad * transform.rotation.eulerAngles.y));
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.A))
            {
                currentSpeed -= new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
                jetpackFuel -= 5 * Time.deltaTime;
            }
            if (Input.GetKey(KeyCode.D))
            {
                currentSpeed += new Vector3(Mathf.Sin(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)), 0, Mathf.Cos(Mathf.Deg2Rad * (90 + transform.rotation.eulerAngles.y)));
                jetpackFuel -= 5 * Time.deltaTime;
            }
            currentSpeed = jetpackHorizontalPower * currentSpeed.normalized;
            if (currentSpeed == Vector3.zero)
            {
                currentSpeed = lastSpeed;
            }
            rb.velocity += currentSpeed;
        }
    }
}