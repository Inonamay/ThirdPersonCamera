using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    #region Movement
    float moveX;
    float moveZ;
    [Header("Movement")]
    [SerializeField] float speed = 20f;
    [SerializeField] float jumpForce = 500f;
    float initialSpeed;
    bool hasJumped = false;
    #endregion
    #region Stats
    float stamina;
    float hitPoints;
    [Header("Stats")]
    [SerializeField] float staminaRegen = 1f;
    [SerializeField] float maxStamina = 10f;
    [SerializeField] float maxHP = 10;
    [SerializeField]float mana;
    [SerializeField] float maxMana = 10f;
    [SerializeField] float manaRegenPerSeconds = 1;
    [SerializeField] float manaRegenTick = 1f;
    [SerializeField] GameObject equippedWeapon;
    WeaponScript equippedWeaponScript;
    #endregion
    Rigidbody rigidBody;
    Animator playerAnimator;
    bool hasReleasedFire1;
  
    void Start()
    {
        rigidBody = gameObject.GetComponent<Rigidbody>();
       
        initialSpeed = speed;
        stamina = maxStamina;
        hitPoints = maxHP;
        mana = maxMana;
       
        if(equippedWeapon != null) { equippedWeaponScript = equippedWeapon.GetComponent<WeaponScript>(); }
        if(GetComponent<Animator>() != null) { playerAnimator = GetComponent<Animator>(); }
        InvokeRepeating("ManaRegen", 1, manaRegenTick);
       
    }
    void Update()
    {
        if (equippedWeapon == null)
        {
            equippedWeapon = new GameObject("Fists", typeof(WeaponScript), typeof(BoxCollider));
            equippedWeapon.GetComponent<BoxCollider>().isTrigger = true;
            equippedWeapon.transform.parent = transform.Find("HandSocket");
            equippedWeapon.transform.localPosition = Vector3.zero;
        }
       
    }
    private void FixedUpdate()
    {
        PlayerInput();
        rigidBody.AddForce(Physics.gravity * GetComponent<Rigidbody>().mass * 5);
       
        if (rigidBody.velocity.y == 0)
        {
            hasJumped = false;
            playerAnimator.SetBool("Jump", false);
        }
    }
    void ManaRegen()
    {
        if (mana < maxMana)
        { mana += manaRegenPerSeconds; }
        else { mana = maxMana; }
    }
    void PlayerInput()
    {
        if (Input.GetKeyDown(KeyCode.Return))
        {
            Cursor.visible = !Cursor.visible;
            if (!Cursor.visible)
            { Cursor.lockState = CursorLockMode.Locked; }
            else { Cursor.lockState = CursorLockMode.None; }
        }
        Movement();
      
        if(Input.GetAxis("Jump") != 0 && !hasJumped)
        {Jump(); playerAnimator.SetBool("Jump", true); }
        if(Input.GetAxisRaw("Fire1") != 0 && hasReleasedFire1)
        {
            hasReleasedFire1 = false;
            Attack();
        }
        if(Input.GetAxisRaw("Fire1") == 0)
        { hasReleasedFire1 = true; }
        if(equippedWeaponScript.WeaponType == WeaponType.Spell)
        { equippedWeaponScript.SpellSelected += Mathf.RoundToInt(Input.GetAxis("Mouse ScrollWheel") * 10); }
        
    }
    void Jump()
    {
        rigidBody.AddForce(Vector3.up * jumpForce);
        hasJumped = true;
    }
    void Movement()
    {
        moveX = Input.GetAxis("Horizontal");
        moveZ = Input.GetAxis("Vertical");
        if (Input.GetKey(KeyCode.LeftShift) && stamina > 0 && (moveX != 0 || moveZ !=0))
        {
            speed = initialSpeed * 2;
            stamina -= Time.deltaTime;
            playerAnimator.SetBool("IsRunning", true);
        }
        else
        {
            speed = initialSpeed;
            playerAnimator.SetBool("IsRunning", false);
            if (stamina < maxStamina)
            { stamina += Time.deltaTime * staminaRegen; }
        }
        Vector3 movement = Vector3.right * moveX * speed * Time.deltaTime + Vector3.forward * moveZ * speed * Time.deltaTime;
        if(movement != Vector3.zero)
        { playerAnimator.SetBool("IsWalking", true); }
        else { playerAnimator.SetBool("IsWalking", false); }
        transform.Translate(movement);
    }
  
   
    void Attack()
    {
        equippedWeaponScript = equippedWeapon.GetComponent<WeaponScript>();
        equippedWeaponScript.Attack();
    }
    public void ChangeHealth(int value, bool damaging)
    {
        if(damaging)
        { hitPoints -= value; }
        else { hitPoints += value;}
        if(hitPoints > maxHP)
        { hitPoints = maxHP; }
        if(hitPoints < 1)
        { GameOver(); }
    }
    void GameOver()
    { print("You died lol"); }
    #region Getters
    public float GetStaminaPercentage()
    { return stamina / maxStamina; }
    public float GetHPPercentage()
    { return hitPoints / maxHP; }
    public float Mana { get { return mana; } set { mana = value; } }
    public float ManaPercentage { get { return mana/maxMana; } }
    public float ManaTick
    {
        set
        {
            CancelInvoke("ManaRegen");
            manaRegenTick = value;
            InvokeRepeating("ManaRegen", 1 , manaRegenTick);
        }
    }
    public float ManaRegenPerSeconds
    {
        set
        {
            CancelInvoke("ManaRegen");
            manaRegenPerSeconds = value;
            InvokeRepeating("ManaRegen", 1, manaRegenTick);
        }
    }
    #endregion
}
