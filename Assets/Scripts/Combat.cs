using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


/*
 * Привет, вот небольшой гайд если заблудился:
 * 
 * 1.1) Для коректной работы скрипта к объекту игрока должен быть прикреплен невидимый объект,
 * по объему больше, чем сам объект "Игрок". Этот объект толжен иметь тег "Looter"
 * 1.2) Объкеты подбираемые игроком должны иметь теги "_Pistol", "_Lasergun", "_Shotgun",
 * "_Flamethrower" и "_Sword" соответсвенно
 * 
 * 2.1) К объектну игрока должен быть прикреплены объекты с тегами "Lasergun", "Pistol", "Shotgun",
 * "Flamehrower", "DisabledSword", "HighTechSword" и "SteamPunkSword" соответсвенно
 * 2.2) Эти объекты С САМОГО НАЧАЛА должны быть выключены!!!!
 *
 * 3.1) Переменная отслеживающая измерение |public int dimension|
 *      Смысл на значение:
 *      1 - high-tech
 *      2 - steampunk
 * 3.2) Оружие отслеживается при помощи |public int currentWeapon|
 *      Смысл на значение:
 *      0 - отсутсвие оружия (в самом начале)
 *      1 - пистолет/дробовик
 *      3 - лазерган/огнемет
 *      5 - меч
 *
 * 4.1) Методы снизу мэйна
 * 4.2) В метод |switchDimension()| добавить реализацию перехода между мирами
 *                  (планово переадресация на метод ветки World)
 * 
 *
 *
 *
 *
 *                                                                                                                                                                                    Если ты это читаешь, вероятно я сплю (*^.^*)  
 */

public class Combat : MonoBehaviour
{
    public GameObject player;
    public GameObject prefabSphereMelee;
    
    public Transform spawnPointSphereMelee;
    
    public string enemyTag = "Enemy";
    public int currentWeapon = 0;
    public int previousWeapon = 0;
    public int dimension = 1;
    public bool isDimensionChanged = false;
    public float timeTemp = -1f;
    
    public LayerMask layerMask;
    
    public MouseButton buttonToShoot = MouseButton.Left;
    public KeyCode buttonToSwitchDimension = KeyCode.Tab;
    public KeyCode buttonToMainWeapon = KeyCode.Alpha1;
    public KeyCode buttonToAdditionalWeapon = KeyCode.Alpha2;
    public KeyCode buttonToSword = KeyCode.Alpha3;

    public HighTechGun Lasergun, Pistol;
    public SteamPunkGun Shotgun, Flamethrower;
    public BladeGun DisabledSword, HighTechSword, SteamPunkSword;
    
    public class HighTechGun
    {
        public float fireRate;
        public float lastFireTime;
        public GameObject gunObject;
        public bool isOpen;
        public bool isAvailable;
        public bool isVisible;
        public float distance;
        public int damage;

        public HighTechGun(float fireRate, GameObject gunObject, bool isOpen, bool isAvailable, bool isVisible, float distance, int damage)
        {
            this.fireRate = fireRate;
            lastFireTime = -fireRate;
            this.gunObject = gunObject;
            this.isOpen = isOpen;
            this.isAvailable = isAvailable;
            this.isVisible = isVisible;
            this.distance = distance;
            this.damage = damage;
        }
    }

    public class SteamPunkGun : HighTechGun 
    {
        public bool isFireGun;
        public float radius;
        public bool isReloading;

        public SteamPunkGun(float fireRate, GameObject gunObject, bool isOpen, bool isAvailable, bool isVisible, float distance, int damage,
            bool isFireGun, float radius, bool isReloading) : base(fireRate, gunObject, isOpen, isAvailable, isVisible, distance, damage)
        {
            this.isFireGun = isFireGun;
            lastFireTime = -fireRate;
            this.radius = radius;
            this.isReloading = isReloading;
        }
    }
    
    public class BladeGun : HighTechGun
    {
        public float abilityRate;
        public float lastAbilityTime;
        public bool dimensionID;
        public float range;
        public bool isReloading;

        public BladeGun(float fireRate, GameObject gunObject, bool isOpen, bool isAvailable, bool isVisible, float distance, int damage,
            float abilityRate, bool dimensionID, float range, bool isReloading) : base(fireRate, gunObject, isOpen, isAvailable, isVisible, distance, damage)
        {
            this.abilityRate = abilityRate;
            lastAbilityTime = -abilityRate;
            lastFireTime = -fireRate;
            this.dimensionID = dimensionID;
            this.range = range;
            this.isReloading = isReloading;
        }
    }
    
    /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
     
    void Start()
    {
        Lasergun = new HighTechGun(7f, GameObject.FindGameObjectWithTag("Lasergun"), false,
            false, false, 100f, 100);
        
        Pistol = new HighTechGun(0.5f,GameObject.FindGameObjectWithTag("Pistol"),false,
            false,false,100f,8);
        
        Shotgun = new SteamPunkGun(1f,GameObject.FindGameObjectWithTag("Shotgun"),false,
            false,false,12f, 50, false, 3f, false);
        
        Flamethrower = new SteamPunkGun(15f,GameObject.FindGameObjectWithTag("Flamehrower"),false,
            false,false,7f, 50, true, 3.5f, false);
        
        DisabledSword = new BladeGun(1f,GameObject.FindGameObjectWithTag("DisabledSword"),false,
            false,false,3f,34, 0.5f, false, 3f,false);
        
        HighTechSword = new BladeGun(1f,GameObject.FindGameObjectWithTag("HighTechSword"),false,
            false,false,3f,50, 0.5f, false, 3f,false);
        
        SteamPunkSword = new BladeGun(1f,GameObject.FindGameObjectWithTag("SteamPunkSword"),false,
            false,false,3f,50, 0.5f, true, 3f,false);
        
        player = GameObject.FindGameObjectWithTag("Player");
    }
    
    void Update()
    {
        if ((Time.time < timeTemp + DisabledSword.fireRate / 2f) && (timeTemp != -1f))
        {
            
        }
        else if ((Time.time > timeTemp + DisabledSword.fireRate/2f) && (timeTemp != -1f))
        {
            Destroy(prefabSphereMelee.gameObject);
        }
        
            
        
        




        if (!(Lasergun.isAvailable && Pistol.isAvailable && Shotgun.isAvailable && Flamethrower.isAvailable &&
              DisabledSword.isAvailable && HighTechSword.isAvailable && SteamPunkSword.isAvailable))
        {

        }

        if ((currentWeapon !=  previousWeapon) || (isDimensionChanged))
        {
            Lasergun.lastFireTime = Time.time - Lasergun.fireRate + 4f;
            Pistol.lastFireTime = Time.time - Pistol.fireRate + 0.3f;
            Shotgun.lastFireTime = Time.time - Shotgun.fireRate + 0.6f;
            DisabledSword.lastFireTime = Time.time - DisabledSword.fireRate + 0.5f;
            HighTechSword.lastFireTime = Time.time - HighTechSword.fireRate + 0.5f;
            SteamPunkSword.lastFireTime = Time.time - SteamPunkSword.fireRate + 0.5f;
            previousWeapon = currentWeapon;
            isDimensionChanged = false;
        }
        
        /*if (Input.GetMouseButtonDown((int)buttonToShoot))
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
        }*/
    }

    /*private void FixedUpdate()
    {
        RaycastHit[] coneHit = physics.ConeCastAll;
    }*/

    /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// /// ///
    
    void echoInputs()
    {
        if (Input.GetKeyDown(buttonToMainWeapon) && (currentWeapon != 3))
            currentWeapon = 3;
        else if (Input.GetKeyDown(buttonToAdditionalWeapon) && (currentWeapon != 1))
            currentWeapon = 1;
        else if (Input.GetKeyDown(buttonToSword) && (currentWeapon != 5))
            currentWeapon = 5;

        if (Input.GetKeyDown(buttonToSwitchDimension))
            SwitchDimension();
        
        if (Input.GetMouseButtonDown((int)buttonToShoot))
            Shoot();
        
        

    }
    
    void Shoot()
    {
        switch (FindWeaponID(currentWeapon))    // Сам разбирайся в этой магии, если ты сюда залез, ты достаточно крут xD
        {
            case 1: //пистолет
                HighTechAttack(Pistol);
                break;
            
            case 2: //дробовик
                SteamPunkAttack(Shotgun);
                break;
            
            case 3: //лазерган
                HighTechAttack(Lasergun);
                break;
            
            case 6: //огнемет
                SteamPunkAttack(Flamethrower);
                break;
            
            case 5: //HT меч
                SwordAttack(HighTechSword);
                break;
            
            case 10: //SP меч
                SwordAttack(SteamPunkSword);
                break;
        }
    }

    void SwordAttack(BladeGun gunObject)
    {
        if (DisabledSword.isAvailable)
            if (Time.time > DisabledSword.lastFireTime + DisabledSword.fireRate)
            {
                JustSpherecast();
            }
            else if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
            {
                JustSpherecast();
            }
    }
    void JustSpherecast()
    {
        Vector3 spawnPosition = spawnPointSphereMelee.position + spawnPointSphereMelee.forward * 2f;
        GameObject sphereMelee = Instantiate(prefabSphereMelee, spawnPosition, Quaternion.identity);
        sphereMelee.transform.SetParent(player.transform);
        timeTemp = Time.time;
    }

    void HighTechAttack(HighTechGun gunObject)
    {
        if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
        {
            JustRaycast(gunObject);
        }
    }
    void JustRaycast(HighTechGun gunObject)
    {
        if (Time.time > gunObject.lastFireTime + Pistol.fireRate)
        {
            gunObject.lastFireTime = Time.time;
                    
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit, gunObject.distance) && hit.collider.CompareTag(enemyTag))
            {
                
            }
        }
    }
    
    void SteamPunkAttack(SteamPunkGun gunObject)
    {
        if (Time.time > gunObject.lastFireTime + gunObject.fireRate)
        {
            JustConecast(gunObject);
        }
    }
    void JustConecast(SteamPunkGun gunObject)
    {
        if (Time.time > gunObject.lastFireTime + Pistol.fireRate)
        {
            gunObject.lastFireTime = Time.time;
            
            float coneRadius = gunObject.distance * Mathf.Tan(gunObject.radius * Mathf.Deg2Rad);
            Vector3 coneDirection = Camera.main.transform.forward;
            
            RaycastHit[] hits = Physics.SphereCastAll(Camera.main.transform.position, coneRadius, coneDirection, gunObject.distance);
            
            foreach (RaycastHit hit in hits)
            {
                if (hit.collider.CompareTag(enemyTag))
                {
                    
                }
            }
        }
    }
    
    int FindWeaponID(int value)
    {
        return value * dimension;  // умножаю
    }

    void SwitchDimension()
    {
        isDimensionChanged = true;
    }
    
}



