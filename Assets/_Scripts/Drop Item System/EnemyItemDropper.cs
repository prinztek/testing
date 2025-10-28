using System.Collections.Generic;
using UnityEngine;

public class EnemyItemDropper : MonoBehaviour
{
    [SerializeField] private List<DropItem> possibleDrops;
    [SerializeField] private GameObject pickupPrefab; // generic item prefab to instantiate
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
