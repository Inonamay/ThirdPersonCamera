using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType
{ Fist, Sword, Axe, Hammer, Spell }
public class WeaponScript : MonoBehaviour, IAttack
{
    [SerializeField] WeaponType weapon;
    bool isPlayingAnimation = false;
    float timer = 0;
    [SerializeField] List<GameObject> spells = new List<GameObject>();
    int spellSelected;
    PlayerController pc;
#pragma warning disable 0649
    [SerializeField] Text selectedSpellText;
#pragma warning restore 0649
    private void Start()
    {
        pc = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerController>();
    }
    private void Update()
    {
        if (weapon == WeaponType.Spell)
        { selectedSpellText.text = "Selected Spell: " + spells[spellSelected].GetComponent<SpellScript>().GetName; }
    }
    public void Attack()
    {
        if (!isPlayingAnimation)
        {
            isPlayingAnimation = true;
            switch (weapon)
            {
                case WeaponType.Fist: StartCoroutine("Punch"); break;
                case WeaponType.Spell: StartCoroutine("Cast"); break;
                case WeaponType.Sword: StartCoroutine("Punch"); break;
                case WeaponType.Axe: StartCoroutine("Punch"); break;
                case WeaponType.Hammer: StartCoroutine("Punch"); break;
            }
        }
    }
    public void RandomiseWeapon()
    { weapon = (WeaponType)Random.Range(0, 4); }
    IEnumerator Punch()
    {
        GetComponent<BoxCollider>().enabled = true;
        while (timer < .4f)
        {
            Vector3 movement = Vector3.forward * timer;
            if (transform.localPosition.z > 2) { movement = Vector3.zero; }
            transform.localPosition += movement;
            timer += Time.deltaTime * 4;
            yield return null;
        }
        while (timer > 0)
        {
            Vector3 movement = Vector3.back * timer;
            if (transform.localPosition.z < 0) { movement = Vector3.zero; }
            transform.localPosition += movement;
            timer -= Time.deltaTime * 4;
        }
        transform.position = transform.parent.position;
        GetComponent<BoxCollider>().enabled = false;
        isPlayingAnimation = false;
    }
    IEnumerator Cast()
    {
        if (pc.Mana > spells[spellSelected].GetComponent<SpellScript>().ManaCost)
        {
            GameObject temp = Instantiate(spells[spellSelected]);
            temp.GetComponent<BoxCollider>().enabled = false;
            temp.transform.rotation = transform.rotation;
            temp.transform.position = transform.position;
            temp.transform.Translate(Vector3.forward);
            temp.GetComponent<BoxCollider>().enabled = true;
            float cooldown = temp.GetComponent<SpellScript>().Cooldown;
            pc.Mana -= spells[spellSelected].GetComponent<SpellScript>().ManaCost;
            while (cooldown > 0)
            {
                cooldown -= Time.deltaTime;
                yield return null;
            }
            isPlayingAnimation = false;
        }
        else
        { isPlayingAnimation = false; }
    }
    IEnumerator Slam()
    { yield return null; }
    IEnumerator Hack()
    { yield return null; }
    IEnumerator Slash()
    { yield return null; }
    public WeaponType WeaponType { get { return weapon; } }
    public int SpellSelected
    {
        get { return spellSelected; }
        set
        {
            if (value >= 0 && value < spells.Count) { spellSelected = value; }
            else if (value > spells.Count - 1) { spellSelected = 0; }
            else { spellSelected = spells.Count - 1; }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        switch (other.tag)
        {
            case "Movable": other.gameObject.GetComponent<Rigidbody>().AddForce((other.gameObject.transform.position - transform.position) * 50); break;
            case "Player": other.GetComponent<PlayerController>().ChangeHealth(1, true); break;
        }
    }
}
