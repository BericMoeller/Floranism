using System.Collections.Generic;
using UnityEngine;

public class EnemyController : MonoBehaviour
{  //instance variables (shocker that one is)
    public int health;
    private int maxHealth;
    public List<string[]> debuffs;
    public Collider2D targetedPlayer;
    public int dirFacing;
    public Vector2 dirWalking;
    public float MOVEMENT_SPEED;
    public Vector2 playerChasingVector = new Vector2(0, 0);
    public int ticksSinceLastScan = 0;
    private int paranoiaLevel;
    public bool playerInSight;
    public bool wandering;
    public bool canGoLeft;
    public bool canGoRight;
    public bool canGoUp;
    public bool canGoDown;
    private float RADIUS;
    private float cornerBuffer;
    public Vector2 dirPlayerWhenCommited;
    public int indexCommitted;
    private float ATTACK_RADIUS;
    private int ATTACK_COOLDOWN;
    public int timeSinceLastAttack;
    private int DAMAGE;
    public bool isAttacking = false;
    Animator animator;
    SpriteRenderer SpriteRenderer;


    void Start()
    {
        InitializeEnemyController();
    }

    public void InitializeEnemyController()
    {
        gameObject.tag = "Enemy";
        paranoiaLevel = 150; //decreasing this increases the speed that scans occur
        //^ I can modify this later so it triggers more often when players are being annoying
        maxHealth = 50; //^or if i'm bored it'd be a fun gamemode to have everything hate you
        health = maxHealth + 0; //^ irl simulator
        MOVEMENT_SPEED = 0.02F;
        debuffs = new List<string[]>();
        dirFacing = 0;
        dirWalking = ChangeDirection(); //this just instantilizes it to the dirFacing (0), meaning the vector would be (0,1)
        wandering = true;
        canGoLeft = true;
        canGoRight = true;
        canGoUp = true;
        canGoDown = true;
        RADIUS = 0.55F; // this needs to stay 0.05 above for collision's sake
        cornerBuffer = 0.3F;
        ATTACK_RADIUS = 0.4F;
        ATTACK_COOLDOWN = 50;
        timeSinceLastAttack = 0;
        DAMAGE = 3;
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (health < 0) {
            Destroy(gameObject);
        }else if(health < maxHealth/2)
        {
            SpriteRenderer.color = new Color(255, 0, 0);
        }
    }

    void FixedUpdate()
    {
        
        MakeMovementDecision(); //all of this runs once per tick. jeez
        Attack();
        AnimSetup();
        ticksSinceLastScan++;
        timeSinceLastAttack++;
        
    }

    public float GetHealth()
    { //returns the health values
        return health;
    }

    public void ChangeHealth(int healthModifier)
    { //for like funky things in the future dunno
        health += healthModifier;
    }

    public void Attacked(float damage, string debuff = "", int debuffTime = 0)
    { // enemy is attacked(by player)
        health -= (int)damage; //^ their feelings were hurt v.v
        if (debuffTime != 0)
        {
        }
    }

    void AnimSetup()
    {
        int finalDir = dirFacing;
        if(dirFacing == 3)
        {
            finalDir = 1;
            SpriteRenderer.flipX = true;
        }
        else
        {
            SpriteRenderer.flipX = false;
        }
        animator.SetInteger("facing", finalDir);
        animator.SetBool("isAttacking", isAttacking);
    }
    void Attack()
    {
        bool foundPlayer = false;
        int playerIndex = -1;
        RaycastHit2D[] hits = Physics2D.CircleCastAll(transform.position, ATTACK_RADIUS, Vector2.up);
        for (int i = 0; i < hits.Length; i++)
        {
            if (hits[i].collider.CompareTag("Player"))
            {
                foundPlayer = true;
                playerIndex = i;
                break;
            }
        }
        if (foundPlayer && timeSinceLastAttack > ATTACK_COOLDOWN)
        {
            timeSinceLastAttack = 0;
            isAttacking = true;
            hits[playerIndex].collider.gameObject.GetComponent<PlayerController>().Attacked(DAMAGE);

        }
        else
        {
            isAttacking = false;
        }
    }

    void DirectionCalculate(Vector2 dir)
    {
        bool facingH = false;
        int endDir = 0;
        if (Mathf.Abs(dir.x) > Mathf.Abs(dir.y))
        {
            facingH = true;
        }
        if (facingH && dir.x > 0) { endDir = 3; }
        else if (facingH) { endDir = 1; }
        else if(dir.y > 0) { endDir = 0; }
        else { endDir = 2; }
        dirFacing = endDir;
    }

    RaycastHit2D CheckVision(bool changePower = false, Vector2 scanAngle = default)
    { //checks vision at angle or direction facing
        Vector2 angle;
        Vector3 offsetDistance;
        if (!changePower)
        {
            switch (dirFacing)
            {
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
        else
        {
            angle = scanAngle;
            offsetDistance = (Vector2)transform.position + scanAngle.normalized / 2;
        }
        RaycastHit2D colliderHit = Physics2D.Raycast(((Vector3)offsetDistance), angle); //gets the collider directly in front of it
        return colliderHit; //returns
    }

    RaycastHit2D[] CheckWalkingIntoSomething(bool returnAll = false) //returns null if not walking into something, array with distances if yes
    {
        RaycastHit2D[] hitsArray;
        RaycastHit2D colliderHit = CheckVision();
        if (returnAll || colliderHit.distance < 1F)
        {
            hitsArray = new RaycastHit2D[4];
            int origDirFacing = 0 + dirFacing;
            for (int i = 0; i < 4; i++)
            { //checks all directions and throws it in a funky lil' array
                dirFacing = i;
                colliderHit = CheckVision();
                hitsArray[i] = colliderHit;
            }
            dirFacing = origDirFacing;
            return hitsArray;
        }
        else
        {
            return null;
        }
    }

    void MakeMovementDecision()
    { // BIG METHOD that controls all movement
        if (wandering)
        {
            RaycastHit2D colliderHit = CheckVision();
            if (colliderHit.collider.CompareTag("Player"))
            { // checks for player
                targetedPlayer = colliderHit.collider;
                wandering = false;
            }
            else
            { // usual result- does normal walking around and vibing
                RaycastHit2D[] hits = CheckWalkingIntoSomething();
                if (hits != null)
                {
                    float largestDist = 0F;
                    int largestIndex = -1;
                    for (int i = 0; i < 4; i++)
                    {
                        if (hits[i].distance > largestDist)
                        {
                            //Debug.Log(hits[i].distance);
                            largestIndex = i;
                            largestDist = hits[i].distance;
                        }
                    }
                    dirFacing = largestIndex;
                    dirWalking = ChangeDirection();
                }
                if (ticksSinceLastScan > paranoiaLevel)
                { //scans regularly for the player(scans more commonly when paranoid)
                    ScanMode();
                    ticksSinceLastScan = 0;
                }
                transform.position += (Vector3)(dirWalking * MOVEMENT_SPEED);
            }
        }
        else
        {
            ChasePlayer(); // if the enemy sees the player it goes for that (yuppers)
        }
    }

    Vector2 ChangeDirection(int direction = -1)
    { //converts my dumb dirFacing variable to the smart Vector2 dirWalking value
        if (direction == -1)
        { //dirFacing is mostly used for animations and math- so it's useful but not good for movement
            direction = dirFacing;
        }
        Vector2 dirWalkingExit;
        switch (direction)
        {
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

    void ChasePlayer()
    { // Chases player(duh)
        if (targetedPlayer)
        {
            this.playerChasingVector = targetedPlayer.transform.position - transform.position;
            RaycastHit2D[] hitsArray = CheckWalkingIntoSomething(true);
            dirWalking = ChasingMovementDecision(this.playerChasingVector);
            EnemyMove(dirWalking);
        }
    }

    Vector2 ChasingMovementDecision(Vector2 idealDirection)
    { //evaluates direction moving for AI path
        RaycastHit2D[] scanHits = ScanMode(90); // input... argument! that's what it's called? i think? always forget. it controls the number of times it scans within the circle
        //^360 would be every degree. 90 or more seems to work pretty well though
        if (scanHits == null)
        {
            playerInSight = true;
            dirPlayerWhenCommited = Vector2.zero;
            indexCommitted = 0;
            Vector2 output = MoveAroundObstacle(idealDirection.normalized);
            return output;
        }
        else // object overcovering maneuvers
        {
            /*
             * ok plan for this(this is a very complicated algorithm in of itself)
             * group together colliders in a list
             * if it's in the way of the player, choose the side with the quicker exit
             * exit through that direction(or slightly off so enemy doesn't collide through walls)
             * 
             * edit: It has been roughly 4 days since i wrote that explanation. very complicated does not cover it. i am still debugging.
             */
            int closestIndex;
            if (!dirPlayerWhenCommited.Equals(Vector2.zero))
            {
                closestIndex = indexCommitted;
            }
            else
            {
                dirPlayerWhenCommited = playerChasingVector;
                playerInSight = false;
                List<int> colliderJumps = new List<int>();
                float distanceJump;
                bool colliderSwitch;
                for (int i = 1; i < scanHits.Length; i++)//compiles all suitable jumps into a list (jumps must be substansial and shift colliders)
                {
                    distanceJump = Mathf.Abs(scanHits[i].distance - scanHits[i - 1].distance);
                    colliderSwitch = scanHits[i].collider != scanHits[i - 1].collider;
                    //Debug.Log("Hit index: "+i+"/"+scanHits.Length+"; Distance Jump: "+(float)distanceJump+"; Colliders Switched:"+ colliderSwitch);
                    if (colliderSwitch && distanceJump > RADIUS * 2)
                    {
                        colliderJumps.Add(i);
                        //Debug.Log("ColliderJump rel location: " + scanHits[i].collider.transform.localPosition);
                    }
                }
                float rad;
                Vector2 spaceVector;
                closestIndex = -1;
                float closestAngle = 360.0F;
                for (int i = 0; i < colliderJumps.Count; i++) //finds best jump
                {
                    rad = colliderJumps[i] * ((2 * Mathf.PI) / scanHits.Length);
                    spaceVector = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
                    //Debug.Log("Radians: " + rad + "; CurrentVector: " + spaceVector + "; IdealVector: " + idealDirection +"; Possibilities: "+ colliderJumps.Count);
                    if ((Mathf.Abs(Vector2.SignedAngle(spaceVector, idealDirection)) < closestAngle) && scanHits[colliderJumps[i]].distance > MOVEMENT_SPEED)
                    {
                        closestIndex = colliderJumps[i];
                        closestAngle = 360 / scanHits.Length * closestIndex;
                    }

                }
                indexCommitted = closestIndex;
            }
            float offset = BoundsOffset(closestIndex, scanHits); //calculates offset so it can actually go around corners
            float endRad = closestIndex * ((2 * Mathf.PI) / scanHits.Length) + offset;
            Vector2 endVector = new Vector2(Mathf.Cos(endRad), Mathf.Sin(endRad)); //takes radial output and converts it to the smarter Vector2 for movement
            //Debug.Log("Winning index: "+closestIndex+"; Options: "+colliderJumps.Count);
            return endVector.normalized;
        }
    }

    Vector2 MoveAroundObstacle(Vector2 currentDirectionToMove) // i added this cus it kept getting stuck on corners
    { //idk man i'm so far down this rabbit hole
        int deg = (int)Vector2.SignedAngle(Vector2.up, currentDirectionToMove);
        if (deg < 0)
        {
            deg += 360;
        }
        int scanDensity = 100; //this changes a few things but mostly accuracy of scans
        int degToScanRatio = scanDensity / 360;
        int scanOffset = scanDensity / 4;
        RaycastHit2D[] forwardScan = ScanMode(scanDensity, deg*degToScanRatio-scanOffset, deg*degToScanRatio+scanOffset, true);
        float closestDist = 500F;
        int closestIndex = -1;
        bool objectInRange = false;
        for (int i = 0; i < forwardScan.Length; i++)
        {
            if (forwardScan[i].collider.CompareTag("Wall") && forwardScan[i].distance < 1.5F * RADIUS)
            {
                if (forwardScan[i].distance < closestDist)
                {
                    closestDist = forwardScan[i].distance;
                    closestIndex = i;
                    objectInRange = true;
                }
            }
        }
        if (objectInRange)
        {
            //Debug.Log("ClosestDist: " + closestDist+"; Deg travelling: "+deg+"; ClosestIndex: "+closestIndex);
            int targetDeg;
            int degToAvoid = deg - scanOffset + closestIndex * (360 / scanDensity); // calculated from above
            int angle = (int)(Mathf.Rad2Deg * Mathf.Atan((RADIUS + cornerBuffer) / closestDist));
            if (degToAvoid < deg)
            {
                targetDeg = degToAvoid + angle;
            }
            else 
            {
                targetDeg = degToAvoid - angle;
            }
            float rad = Mathf.Deg2Rad * targetDeg;
            Vector2 directionModded = new Vector2(Mathf.Cos(rad), Mathf.Sin(rad));
            //Debug.Log("Degree Heading: " + degToAvoid+"; Final Direction: " + targetDeg + "; Radial Conversion: " + rad+ "; DirectionModded: " + directionModded);
            return directionModded;
        }
        else
        {
            return currentDirectionToMove;
        }

    }

    RaycastHit2D[] ScanMode(int scanDensity = 20, int startAngle = 0, int endAngle = -1, bool returnAnyways = false)
    { //well. Scans.
        RaycastHit2D itemScanned;
        bool foundPlayer = false;
        if (endAngle == -1)
        {
            endAngle = scanDensity;
        }
        RaycastHit2D[] itemsScanned = new RaycastHit2D[endAngle - startAngle];
        float scanLimiter = 2 * Mathf.PI / scanDensity;
        float angle;
        for (int i = startAngle; i < endAngle; i++)
        {
            angle = scanLimiter * i;
            itemScanned = CheckVision(true, new Vector2(Mathf.Cos(angle), Mathf.Sin(angle)));
            //Debug.Log(angle+"-"+i);
            if (itemScanned && (!returnAnyways && itemScanned.collider.CompareTag("Player")))
            {
                targetedPlayer = itemScanned.collider;
                wandering = false;
                foundPlayer = true;
                break;
            }
            itemsScanned[i - startAngle] = itemScanned;
        }
        if (foundPlayer && !returnAnyways)
        {
            return null;
        }
        else
        {
            return itemsScanned;
        }
    }
    void EnemyMove(Vector2 movementDir) //this checks that the enemy isn't colliding with anything before moving
    {
        if (!canGoLeft && movementDir.x < 0F)
        {
            movementDir.x = 0F;
        }
        if (!canGoRight && movementDir.x > 0F)
        {
            movementDir.x = 0F;
        }
        if (!canGoDown && movementDir.y < 0F)
        {
            movementDir.y = 0F;
        }
        if (!canGoUp && movementDir.y > 0F)
        {
            movementDir.y = 0F;
        }
        DirectionCalculate(movementDir);
        transform.position += (Vector3)(movementDir)*MOVEMENT_SPEED;
    }
    float BoundsOffset(int idealIndex, RaycastHit2D[] allHits) //does a bunch of math to find out how far we need to overshoot to get around corners
    { // i have very strong feelings about trig (and none of them are positive)
        int index;
        float polarity;
        bool condition;
        if (idealIndex == 0)
        {
            condition = allHits[idealIndex].distance > allHits[allHits.Length - 1].distance;
        }
        else
        {
            condition = allHits[idealIndex].distance > allHits[idealIndex - 1].distance;
        }
        if (condition)//checks polarity
        {
            index = idealIndex - 1;
            polarity = 1.0F;
        }
        else
        {
            index = idealIndex;
            polarity = -1.0F;//AND THERE IS SO MUCH TRIG IN THIS
        }
        if (index == -1)
        {
            index = allHits.Length - 1;
        }
        float CornerDist = allHits[index].distance;
        float offset = Mathf.Atan((RADIUS + cornerBuffer) / CornerDist) * polarity;//MMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMMM
        //Debug.Log("polarity = " + offset);
        return offset;
    }
    void CollisionBehavior(Collision2D collision) //just checks what directions we can't move in (yanked this straight from playerController)
    {
        //Debug.Log("collision");
        Vector2 collisionVector;
        bool xCollision = false;
        bool yCollision = false;
        for (int i = 0; i < collision.contactCount; i++)
        {
            collisionVector = collision.GetContact(i).normal;
            if (collisionVector.x > 0)
            {
                canGoLeft = false;
                xCollision = true;
            }
            else if (collisionVector.x < 0)
            {
                canGoRight = false;
                xCollision = true;
            }
            if (collisionVector.y > 0)
            {
                canGoDown = false;
                yCollision = true;
            }
            else if (collisionVector.y < 0)
            {
                canGoUp = false;
                yCollision = true;
            }
        }
        if (!xCollision)
        {
            canGoLeft = true;
            canGoRight = true;
        }
        if (!yCollision)
        {
            canGoUp = true;
            canGoDown = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        CollisionBehavior(collision);
    }
    private void OnCollisionStay2D(Collision2D collision)
    {
        CollisionBehavior(collision);
    }
    private void OnCollisionExit2D(Collision2D collision)
    {
        CollisionBehavior(collision);
    }
}