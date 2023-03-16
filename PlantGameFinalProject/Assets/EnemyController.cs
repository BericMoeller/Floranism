using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    protected int health;
    private int maxHealth;
    public List<string[]> debuffs;
    // Start is called before the first frame update
    void Start()
    {
        gameObject.tag = "Enemy";
        debuffs = new List<string[]>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int GetHealth(){
        return health;
    }
    void ChangeHealth(int healthModifier){
        health += healthModifier;
    }
    public void Attacked(float damage, string debuff = "", int debuffTime = 0){
        health -= (int)damage;
        if(debuffTime != 0){

        } 

    }
}
