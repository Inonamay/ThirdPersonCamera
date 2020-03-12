using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GameController : MonoBehaviour
{
    [SerializeField] GameObject enemy;
    private void Start()
    {
        if (enemy == null) { AssetDatabase.FindAssets("Assets/Prefabs/Characters/Enemy.prefab"); }
        //InvokeRepeating("SpawnEnemies", 1, 1);
    }
    void SpawnEnemies()
    {
        GameObject temp = Instantiate(enemy);
        temp.transform.position = Vector3.right * Random.Range(1, 90) + Vector3.forward * -Random.Range(1, 90);
    }
}
