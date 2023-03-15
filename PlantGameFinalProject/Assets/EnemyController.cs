using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{
    private int health;
    private int maxHealth;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    int getHealth(){
        return health;
    }
    void changeHealth(int healthModifier){
        health += healthModifier;
    }
}
