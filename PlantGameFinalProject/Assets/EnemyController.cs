using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class EnemyController : MonoBehaviour
{
    public GameObject healthObject; 
    public int health;
    private int maxHealth;
    public List<string[]> debuffs;
    //public Text healthStat;
    public Collider2D targetedPlayer;
    public int dirFacing;
    public Vector2 dirWalking;
    public float MOVEMENT_SPEED;
    public Vector2 playerChasingVector = new Vector2(0,0);
    public int ticksSinceLastScan = 0;
    private int paranoiaLevel;
    public bool playerInSight;
    public bool wandering;

    void Start()
    {
        gameObject.tag = "Enemy";
        paranoiaLevel = 150; //decreasing this increases the speed that scans occur
        //^ I can modify this later so it triggers more often when players are being loud
        maxHealth = 100;
        health = maxHealth + 0;
        MOVEMENT_SPEED = 0.02F;
        debuffs = new List<string[]>();
        dirFacing = 0;
        dirWalking = ChangeDirection();
        wandering = true;
    }

    void Update()
    {
        //healthStat.text = ""+health;
    }

    void FixedUpdate(){
        MakeMovementDecision(); //all of this runs once per tick. jeez
        ticksSinceLastScan++;
    }

    public float GetHealth(){ //returns the health values
        return health;
    }

    public void ChangeHealth(int healthModifier){ //for like funky things in the future dunno
        health += healthModifier;
    }

    public void Attacked(float damage, string debuff = "", int debuffTime = 0){ // enemy is attacked(by player)
        health -= (int)damage;
        if(debuffTime != 0){
        }
    }


    RaycastHit2D CheckVision(bool changePower = false, Vector2 scanAngle = default)
    { //checks vision at angle or direction facing
        Vector2 angle;
        Vector3 offsetDistance;
        if(!changePower){ 
            switch(dirFacing){
                case 0:
                    angle = Vector2.up;
                    offsetDistance = (Vector2)transform.position + Vector2.up / 2;
                    break;
                case 1:
                    angle = Vector2.right;
                    offsetDistance = (Vector2)transform.position + Vector2.right / 2;
                    break;
                case 2:
                    angle = Vector2.down;
                    offsetDistance = (Vector2)transform.position + Vector2.down / 2;
                    break;
                case 3:
                    angle = Vector2.left;
                    offsetDistance = (Vector2)transform.position + Vector2.left / 2;
                    break;
                default:
                    angle = Vector2.up;
                    offsetDistance = (Vector2)transform.position + Vector2.up / 2;
                    break;
            }
        }
        else {
            angle = scanAngle;
            offsetDistance = (Vector2)transform.position + scanAngle.normalized / 2;
        }
        RaycastHit2D colliderHit = Physics2D.Raycast(((Vector3)offsetDistance), angle); //gets the collider directly in front of
        return colliderHit; //returns
    }

    RaycastHit2D[] CheckWalkingIntoSomething(bool returnAll = false) //returns null if not walking into something, array with distances if yes
    {
        RaycastHit2D[] hitsArray;
        RaycastHit2D colliderHit = CheckVision();
        if(returnAll || colliderHit.distance < 1F){
            hitsArray = new RaycastHit2D[4];
            int origDirFacing = 0+ dirFacing;
            for(int i = 0; i < 4; i++){ //checks all directions and throws it in a funky lil' array
                dirFacing = i;
                colliderHit = CheckVision();
                hitsArray[i] = colliderHit;
            }
            dirFacing = origDirFacing;
            return hitsArray;
        }else{
            return null;
        }
    }

    void MakeMovementDecision(){ // BIG METHOD that controls all movement
        if(wandering){
            RaycastHit2D colliderHit = CheckVision();
            if(colliderHit.collider.CompareTag("Player")){ // checks for player
                targetedPlayer = colliderHit.collider;
                wandering = false;
            }else{ // usual result- does normal walking around and vibing
                RaycastHit2D[] hits = CheckWalkingIntoSomething();
                if(hits != null){
                    float largestDist = 0F;
                    int largestIndex = -1;
                    for(int i = 0; i < 4; i++){
                        if(hits[i].distance > largestDist){
                            Debug.Log(hits[i].distance);
                            largestIndex = i;
                            largestDist = hits[i].distance;
                        }
                    }
                    dirFacing = largestIndex;
                    dirWalking = ChangeDirection();
                }
                if(ticksSinceLastScan > paranoiaLevel){ //scans regurally for the player(scans more commonly when paranoid)
                    ScanMode();
                    ticksSinceLastScan = 0;
                }
                transform.position += (Vector3)(dirWalking * MOVEMENT_SPEED);
            }
        }else{
            ChasePlayer(); // if the enemy sees the player it goes for that (yuppers)
        }
    }

    Vector2 ChangeDirection(int direction = -1){ //converts my dumb dirFacing variable to the smart Vector2 dirWalking value
        if(direction == -1){
            direction = dirFacing;
        }
        Vector2 dirWalkingExit;
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

    void ChasePlayer(){ // Chases player(duh)
        this.playerChasingVector = targetedPlayer.transform.position - transform.position;
        RaycastHit2D[] hitsArray = CheckWalkingIntoSomething(true);
        dirWalking = ChasingMovementDecision(this.playerChasingVector);    
        if (CheckVision(true, dirWalking).distance > 0.1F)
        {
            transform.position += (Vector3)(dirWalking * MOVEMENT_SPEED);
        }
    }

    Vector2 ChasingMovementDecision(Vector2 idealDirection){ //rates the quality of the direction
        RaycastHit2D[] scanHits = ScanMode(90);
        if (scanHits == null)
        {
            playerInSight = true;
            return idealDirection.normalized;
        }
        else // object overcovering manuvers
        {
            /*
             * ok plan for this(this is a very complicated algorithm in of itself
             * group together colliders in a list
             * if it's in the way of the player, choose the side with the quicker exit
             * exit through that direction(or slightly off so enemy doesn't collide through walls)
             */
            playerInSight = false;
            List<int> colliderJumps = new List<int>();
            float distanceJump;
            bool colliderSwitch;
            for(int i = 1; i < scanHits.Length; i++)
            {
                distanceJump = Mathf.Abs(scanHits[i].distance - scanHits[i - 1].distance);
                colliderSwitch = scanHits[i].collider != scanHits[i - 1].collider;
                Debug.Log("Hit index: "+i+"/"+scanHits.Length+"; Distance Jump: "+(float)distanceJump+"; Colliders Switched:"+ colliderSwitch);
                if (colliderSwitch && distanceJump > 1F)
                {
                    colliderJumps.Add(i);
                    //Debug.Log("ColliderJump rel location: " + scanHits[i].collider.transform.localPosition);
                }
            }
            float rad;
            Vector2 spaceVector;
            int closestIndex = -1;
            float m_angle = 360.0F;
            Vector2 closestVector = new Vector2(0, 0);
            for(int i = 0; i < colliderJumps.Count; i++)
            {
                rad = colliderJumps[i]*((2 * Mathf.PI) / scanHits.Length);
                spaceVector = new Vector2(Mathf.Cos(rad),Mathf.Sin(rad));
                //Debug.Log("Radians: " + rad + "; CurrentVector: " + spaceVector + "; IdealVector: " + idealDirection +"; Possibilities: "+ colliderJumps.Count);
                if ((Mathf.Abs(Vector2.SignedAngle(spaceVector, idealDirection)) < m_angle) && CheckVision(true, spaceVector).distance > 0.1F)
                {
                     closestIndex = colliderJumps[i];
                     closestVector = spaceVector;
                }
               
            }
            Debug.Log("Winning Vector: "+ BoundsOffset(closestVector.normalized)+"; Options: "+colliderJumps.Count);
            return BoundsOffset(closestVector.normalized);
        }
    }


    RaycastHit2D[] ScanMode(int scanDensity = 20){ //well. Scans.
        RaycastHit2D itemScanned;
        bool foundPlayer = false;
        RaycastHit2D[] itemsScanned = new RaycastHit2D[scanDensity];
        float scanMultiplier = 2F / scanDensity;
        for(float i = 0; i < 2F*Mathf.PI; i += Mathf.PI*scanMultiplier){
            itemScanned = CheckVision(true, new Vector2(Mathf.Cos(i),Mathf.Sin(i)));
            Debug.Log(i);
            if(itemScanned.collider.CompareTag("Player")){
                targetedPlayer = itemScanned.collider;
                wandering = false;
                foundPlayer = true;
                break;
            }
            itemsScanned[(int)(i/Mathf.PI)] = itemScanned;
        }
        if (foundPlayer)
        {
            return null;
        }
        else
        {
            return itemsScanned;
        }
    }
    Vector2 BoundsOffset(Vector2 direction)
    {
        float offset = 0.5F;
        if (direction.x > 0)
        {
            direction.x += offset;
        }
        else if (direction.x < 0)
        {
            direction.x -= offset;
        }
        if (direction.y > 0)
        {
            direction.y += offset;
        }
        else if (direction.y < 0)
        {
            direction.y -= offset;
        }
        return direction;
    }
}