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
    public Vector2 playerChasingVector = new Vector2(0,0);
    public int ticksSinceLastScan = 0;
    private int paranoiaLevel; //on scale from 0 to 99
    // Start is called before the first frame update

    void Start()
    {
        gameObject.tag = "Enemy";
        paranoiaLevel = 30;
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
        ticksSinceLastScan++;
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
            Debug.Log("Doing stuff");
        } 
    }

    RaycastHit2D VisionCheck(bool override = false, Vector2 scanAngle = Vector2.zero){
        Vector2 angle;
        if(!override){
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
        }else if(targetedPlayer != null){
            angle = playerChasingVector;
        }else{
            angle = scanAngle;
        }
        RaycastHit2D colliderHit = Raycast(transform.position, angle);
        return colliderHit;
    }
    RaycastHit2D[] CheckWalkingIntoSomething() //returns empty array if not walking into something, array with ranked distances if yes
    {
        RaycastHit2D[] hitsArray;
        //IComparer reverseComparer = new myReverserClass();
        RaycastHit2D colliderHit = VisionCheck();
        if(colliderHit.distance < 1F){
            hitsArray = new RaycastHit2D[4];
            int origDirFacing = 0+ dirFacing;
            for(int i = 0; i < 4; i++){
                dirFacing = i;
                RaycastHit2D colliderHit = VisionCheck();
                hitsArray[i] = colliderHit;
            }
            dirFacing = origDirFacing;
        }
        return hitsArray;
    }
    void MakeMovementDecision(){
        if(targetedPlayer == null){
            RaycastHit2D colliderHit = VisionCheck();
            if(colliderHit.collider.tag == "Player"){
                targetedPlayer = Raycast.collider;
            }else{
                RaycastHit2D[] hits = CheckWalkingIntoSomething()[0];
                if(hits != null){
                    float largestDist = 0F;
                    int largestIndex;
                    for(int i = 0; i < 4; i++){
                        if(hits[i].distance > largestDist){
                            largestIndex = i;
                            largestDist = hits[i].distance;
                        }
                    }
                    dirFacing = largestIndex;
                    dirWalking = ChangeDirection();
                }
                if(ticksSinceLastScan > 150 && Random.Range(0,100)<paranoiaLevel){
                    ScanMode();
                }
                transform.position += dirWalking * MOVEMENT_SPEED;
            }
        }else{
            ChasePlayer();
        }
    }
    Vector2 ChangeDirection(Vector2 direction = dirFacing){
        switch(direction){
            case 0:
                dirWalkingExit = Vector2.up;
                break;
            case 1:
                dirWalkingExit = Vector2.right;
                break;
            case 2:
                dirWalkingExit = Vector2.down;
                break;
            case 3:
                dirWalkingExit = Vector2.left;
                break;
            default:
                dirWalkingExit = Vector2.up;
                break;
        }
        return dirWalkingExit;
    }
    void ChasePlayer(){
        Vector3 relPos = targetedPlayer.transform.localPosition;
        Vector2 playerChasingVector = relPos;
        RaycastHit2D colliderHit = VisionCheck(true);
        if(colliderHit.collider.tag != "Player"){
            if(colliderHit.distance < 1){
                int bestScore = 0;
                int bestScoreIndex;
                int score;
                RaycastHit2D[] hitsArray = CheckWalkingIntoSomething();  
                for(int i = 0; i < 4; i++){
                    score = RateMovementDirection(hitsArray[i].distance, playerChasingVector, ChangeDirection(i));
                    if(bestScore < score){
                        bestScore = score;
                        bestScoreIndex = i;
                    }
                }
                dirFacing = bestScoreIndex;
                dirWalking = ChangeDirection();
            }
        }
        transform.position += dirWalking * MOVEMENT_SPEED;
    }
    int RateMovementDirection(float distance, Vector2 idealDirection, Vector2 direction){
        int score = 0;
        if(Mathf.Sign(dirWalkingTest.x)==Mathf.Sign(playerChasingVector.x)){
            score += 20;
        }if(Mathf.Sign(dirWalkingTest.y)==Mathf.Sign(playerChasingVector.y)){
            score += 20;
        }
        score += (int)(distance)*5;
        return score;
    }
    void ScanMode(){
        RaycastHit2D itemScanned;
        for(float i = 0; i < 2*Mathf.PI; i += Mathf.PI*0.2){
            itemScanned = VisionCheck(true, i);
            if(itemScanned.collider.tag == "Player"){
                targetedPlayer = itemScanned.collider;
                break;
            }
        }
    }
}
