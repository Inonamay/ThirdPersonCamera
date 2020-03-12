using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

enum State
{Idle, Following, Attacking}

public class EnemyController : MonoBehaviour
{
    [SerializeField]
    int hp;
    State state;
    Ray ray;
    RaycastHit hit;
    GameObject player;
    [SerializeField]
    GameObject equippedWeapon;
    WeaponScript equippedWeaponScript;
    NavMeshAgent self;
    NavMeshPath path;
    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        EquipNewWeapon();
        self = GetComponent<NavMeshAgent>();
    }
    private void Update()
    {
        FindPlayer();
        if(hit.collider != null)
        {
            switch (state)
            {
                case State.Idle:
                    if (hit.collider.gameObject.tag == "Player")
                    { state = State.Following; }
                    break;
                case State.Following:
                    if (hit.collider.gameObject.tag == "Player")
                    {
                        if (hit.distance < 1.5f) { state = State.Attacking; }
                        else { PathFind(); }
                    }
                    else { state = State.Idle; self.ResetPath(); }
                    break;
                case State.Attacking:
                    if (hit.distance >= 1.5f) { state = State.Following; }
                    else { self.ResetPath(); transform.LookAt(player.transform); Attack(); }
                    break;
            }
        }
        if (hp < 1) {  Destroy(gameObject); }
    }
    void EquipNewWeapon()
    {
        equippedWeapon = new GameObject("Weapon", typeof(WeaponScript), typeof(BoxCollider));
        equippedWeapon.GetComponent<BoxCollider>().isTrigger = true;
        equippedWeaponScript = equippedWeapon.GetComponent<WeaponScript>();
        equippedWeaponScript.RandomiseWeapon();
        equippedWeapon.transform.parent = transform.Find("HandSocket");
        equippedWeapon.transform.localPosition = Vector3.zero;
    }
    void Attack()
    { equippedWeaponScript.Attack();}
    void FindPlayer()
    {Physics.Raycast(transform.position + Vector3.up, player.transform.position - transform.position + Vector3.down * 0.5f, out hit);}
    public void ChangeHealth(int value, bool damaging)
    {
        if (damaging) { hp -= value; }
        else { hp += value; }
    }
    void PathFind()
    {
        path = new NavMeshPath();
        self.CalculatePath(player.transform.position, path);
        self.isStopped = false;
        self.path = path;
    }
}
