using System.Collections;
using System.Collections.Generic;
using UnityEngine;
enum Range { Touch, Short, Medium, Long, Self}
enum SpellType { SingleTarget, AOE }
enum EffectType { Damage, Heal }
[RequireComponent(typeof(BoxCollider))]
public class SpellScript : MonoBehaviour
{
    [SerializeField] Range range = Range.Medium;
    [SerializeField] SpellType type = SpellType.SingleTarget;
    [SerializeField] EffectType effect = EffectType.Damage;
    [SerializeField] float duration = 1f;
    [SerializeField] int effectTypeValue = 1;
    [SerializeField] float aoeSize = 2f;
    [SerializeField] float speed = 20;
    [SerializeField] bool playerDamaging = false;
    [SerializeField] float cooldown = 2f;
    [SerializeField] string spellName = "Spell";
    [SerializeField] bool piercing = false;
    [SerializeField] int manaCost = 2;
    bool stop;
    Vector3 targetPos;
    Vector3 startPos;
    private void Start()
    {
        GetComponent<BoxCollider>().isTrigger = true;
        int rangeUnits = 1;
        switch (range)
        {
            case Range.Short: rangeUnits = 3; break;
            case Range.Medium: rangeUnits = 10; break;
        }
        targetPos = transform.position + transform.forward * rangeUnits;
        startPos = transform.position;
    }
    void Update()
    {
        if (!stop)
        {
            switch (range)
            {
                case Range.Touch:
                    transform.position = GameObject.FindGameObjectWithTag("Player").transform.position;
                    transform.rotation = GameObject.FindGameObjectWithTag("Player").transform.rotation;
                    transform.Translate(Vector3.forward);
                    if (duration > 0) { duration -= Time.deltaTime; }
                    else { Destroy(gameObject); }
                    break;
                case Range.Self:
                    if(type == SpellType.AOE) { StartCoroutine(Effect(CheckAOE())); }
                    else { StartCoroutine(Effect(GameObject.FindGameObjectsWithTag("Player"))); }
                    stop = true;
                    break;
                case Range.Long: transform.Translate(Vector3.forward * Time.deltaTime * speed); break;
                default: transform.position = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * speed); if (transform.position == targetPos) { Destroy(gameObject); } break;
            }
        }
        if(Vector3.Magnitude(transform.position - startPos) > 50) { Destroy(gameObject); }
    }
    private void OnTriggerEnter(Collider other)
    {
        GameObject[] targets;
        if(other.gameObject.tag == "Player" && !playerDamaging)
        {targets = new GameObject[0]; }
        else
        {
            if (type == SpellType.AOE)
            {targets = CheckAOE();}
            else
            {
                targets = new GameObject[1];
                if (playerDamaging) { targets[0] = other.gameObject; }
                else if (other.gameObject.tag != "Player") { targets[0] = other.gameObject; }
            }
        }
        if(targets.Length != 0) { StartCoroutine(Effect(targets)); }
    }
    GameObject[] CheckAOE()
    {
        GameObject[] targets;
        Collider[] col = Physics.OverlapSphere(transform.position, aoeSize);
        targets = new GameObject[col.Length];
        for (int i = 0; i < col.Length; i++)
        {
            if (!playerDamaging && col[i].gameObject != GameObject.FindGameObjectWithTag("Player"))
            {targets[i] = col[i].gameObject;}
            else if (playerDamaging) { targets[i] = col[i].gameObject; }
        }
        return targets;
    }
    IEnumerator Effect(GameObject[] targets)
    {
        float timer = 0;
        ExecuteEffect(targets);
        while (duration > 0)
        {
            duration -= Time.deltaTime;
            timer += Time.deltaTime;
            if(type == SpellType.AOE && timer > 1) { timer = 0; ExecuteEffect(CheckAOE()); }
            if (!piercing) { stop = true; }
            yield return null;
        }
        Destroy(gameObject);
    }
    void ExecuteEffect(GameObject[] targets)
    {
        bool damage;
        if (effect == EffectType.Damage) { damage = true; }
        else { damage = false; }
        for (int i = 0; i < targets.Length; i++)
        {
            if(targets[i] != null)
            {
                if (targets[i].tag == "Enemy") { targets[i].GetComponent<EnemyController>().ChangeHealth(effectTypeValue, damage); }
                if (targets[i].tag == "Player") { targets[i].GetComponent<PlayerController>().ChangeHealth(effectTypeValue, damage); }
            }
        }
    }
    public float Cooldown { get { return cooldown; } }
    public string GetName { get { return spellName; } }
    public int ManaCost { get { return manaCost; } }
}
