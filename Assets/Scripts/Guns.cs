using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HIghTechGuns : MonoBehaviour
{
    public GameObject player;
    public MouseButton buttonToShoot = MouseButton.Left;
    public float raycastDistance = 100f;
    public string enemyTag = "Enemy";
    public float fireRate = 0.5f;
    private float lastFireTime;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        lastFireTime = -fireRate;
    }

    // Update is called once per frame
    void Update()
    {
        string currentWeapon = player.GetComponent<PlayerController>().currentWeapon;
        /* if CurrentWeapon != PreviousWeapon
                
                
                
                
            */
        
        if (Input.GetMouseButtonDown((int)buttonToShoot))
        {
            if (Time.time > lastFireTime + fireRate)
            {
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
                // Получаем направление луча
                RaycastHit hit;
                // Сохранение информации об объекте, на котором остановился луч
                if (Physics.Raycast(ray, out hit, raycastDistance) && hit.collider.CompareTag(enemyTag))
                {
                }
                lastFireTime = Time.time;
            }
            else
            {

            }
        }
    }
}
