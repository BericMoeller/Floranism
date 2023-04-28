using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestructableObject : MonoBehaviour
{
    public int health = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
        {
            Destroy(gameObject);
        }
    }
    public void Attacked(float damage, string debuff = "", int debuffTime = 0)
    { // enemy is attacked(by player)
        health -= (int)damage; //^ their feelings were hurt v.v
    }
}
