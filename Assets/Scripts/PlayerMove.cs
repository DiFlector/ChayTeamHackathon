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


    [SerializeField] private bool _canJump;

    void Start()
    {
        _canJump = true;
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


        if (Input.GetKey(KeyCode.Space) && _canJump)
        {
            Jump();
        }

        if (Input.GetKey(KeyCode.W))
        {
            transform.Translate(0, 0, _speed * Time.deltaTime);
        }
        if (Input.GetKey(KeyCode.S))
        {
            transform.Translate(0, 0, _speed * Time.deltaTime * -1);
        }
        if (Input.GetKey(KeyCode.A))
        {
            transform.Translate(_speed * Time.deltaTime * -1, 0, 0);

        }
        if (Input.GetKey(KeyCode.D))
        {
            transform.Translate(_speed * Time.deltaTime, 0, 0);
        }
    }

    void Jump()
    {
        if (Input.GetKey(KeyCode.Space) && _canJump == true)
        {
            _canJump = false;
            GetComponent<Rigidbody>().AddForce(0, 1000, 0);
        }
    }

}
