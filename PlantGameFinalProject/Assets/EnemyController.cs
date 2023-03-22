using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public GameObject healthObject; 
    public int health;
    private int maxHealth;
    public List<string[]> debuffs;
    public Text healthStat;
    public Collider2D targetedPlayer;
    public bool wandering;
    public Vector2 dirWalking;
    public float MOVEMENT_SPEED;
    // Start is called before the first frame update

    void Start()
    {
        gameObject.tag = "Enemy";
        maxHealth = 100;
        health = maxHealth + 0;
        MOVEMENT_SPEED = 0.1;
        debuffs = new List<string[]>();
    }

    // Update is called once per frame
    void Update()
    {
        healthStat.text = ""+health;
    }
    void FixedUpdate(){
        MakeMovementDecision();
    }
    public float GetHealth(){
        return health;
    }
    public void ChangeHealth(int healthModifier){
        health += healthModifier;
    }
    public void Attacked(float damage, string debuff = "", int debuffTime = 0){
        health -= (int)damage;
        if(debuffTime != 0){

        } 
        Debug.Log("Enemy Health: "+ health);

    }
    RaycastHit2D CheckInVision(){
        Vector2 angle;
        switch(dirFacing){
            case 0:
                angle = Vector2.up;
                break;
            case 1:
                angle = Vector2.right;
                break;
            case 2:
                angle = Vector2.down;
                break;
            case 3:
                angle = Vector2.left;
                break;
            default:
                angle = Vector2.up;
                break;
        }
        RaycastHit2D colliderHit = Raycast(transform.position, angle);
        return colliderHit;
    }
    int CheckWalkingIntoSomething() //returns -1 if not walking into something, dir with lower distance if yes;
    {
        RaycastHit2D colliderHit = CheckInVision();
        if(colliderHit.distance < 1F){
            float largestDistance = 0;
            int largestDir;
            for(int i = 0; i < 4; i++){
                dirFacing = i;
                RaycastHit2D colliderHit = CheckInVision();
                if(colliderHit.distance > largestDistance){
                    largestDistance = colliderHit.distance;
                     largestDir = i;
                }
            }
            return largestDir;
        }else{
            return -1;
        }
    }
    void MakeMovementDecision(){
        if(targetedPlayer == null){
            RaycastHit2D colliderHit = CheckInVision();
            if(colliderHit.collider.tag == "Player"){
                targetedPlayer = Raycast.collider;
            }else{
                int dir = CheckWalkingIntoSomething();
                if(dir != -1){
                    dirFacing = dir;
                }
                ChangeDirection();
                transform.position += dirWalking * MOVEMENT_SPEED;
            }
        }else{
            ChasePlayer();
        }
    }
    void ChangeDirection(){
        switch(dirFacing){
            case 0:
                dirWalking = Vector2.up;
                break;
            case 1:
                dirWalking = Vector2.right;
                break;
            case 2:
                dirWalking = Vector2.down;
                break;
            case 3:
                dirWalking = Vector2.left;
                break;
            default:
                dirWalking = Vector2.up;
                break;
        }
    }
    void ChasePlayer(){
        Vector3 relPos = targetedPlayer.transform.localPosition;
    
    }
}
