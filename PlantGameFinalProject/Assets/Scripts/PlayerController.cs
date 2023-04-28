using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 velocity;
    private float FrictionConstant = 0.3F;
    Animator animator;
    bool runningX = false;
    bool runningY = false;
    bool facingRight = true;
    public int dirFacing = 0; // 0 is up, 1 is right, 2 is down, 3 is left
    SpriteRenderer SpriteRenderer;
    bool hasPressedMoveY;
    bool hasPressedMoveX;
    public int health;
    public int MAX_HEALTH;
    private bool canGoLeft = true;
    private bool canGoRight = true;
    private bool canGoUp = true;
    private bool canGoDown = true;
    public float MOVEMENT_SPEED;
    public int dashCooldown;
    public int DASH_COOLDOWN_SET;
    public float DASH_SPEED = 0.2F;
    public string lastMove;
    public int lastMoveTime;
    public int attackCooldown;
    public int ATTACK_COOLDOWN_L;
    public int ATTACK_COOLDOWN_H;
    public int specialCooldown;
    public int SPECIAL_COOLDOWN;
    public Vector2 RANGE_OF_ATTACK;
    public float blockCharge;
    public int BLOCK_QUANTITY;
    public bool isBlocking;
    public int animState;
    public int blockState;
    int attacked = 0;

    // Start is called before the first frame update
    void Start()
    {
        MOVEMENT_SPEED = 0.2F;
        velocity = new Vector3(0F, 0F, 0F);
        dashCooldown = 0;
        DASH_COOLDOWN_SET = 150;
        MAX_HEALTH = 100;
        animator = GetComponent<Animator>();
        SpriteRenderer = GetComponent<SpriteRenderer>();
        gameObject.tag = "Player";
        RANGE_OF_ATTACK = new Vector2(1,0);
        ATTACK_COOLDOWN_H = 50;
        ATTACK_COOLDOWN_L = 15;
        SPECIAL_COOLDOWN = 180;
        BLOCK_QUANTITY = 30;
        blockCharge = BLOCK_QUANTITY + 0;
        isBlocking = false;
        health = 0 + MAX_HEALTH;
    }

    // Update is called once per frame
    void Update(){
        MovementMechanism();
        AttackMechanism();
    }

    public void AddHealth(int healthMod)
    {
        health += healthMod;
        if(health > MAX_HEALTH)
        {
            health = MAX_HEALTH + 0;
        }
    }
    void AttackMechanism(){
        bool hasGone = false;
        if(attackCooldown == 0){
            if(Input.GetKey(KeyCode.J)){
                attacked = 2;
                hasGone = true;
                Attack("Light");
            }
            if(Input.GetKey(KeyCode.K)){
                attacked = 3;
                hasGone = true;
                Attack("Heavy");
            }
        }if(specialCooldown == 0){
            if(Input.GetKey(KeyCode.L)){
                attacked = 4;
                hasGone= true;
                Attack("Special");
            }
        }
        if (Input.GetKey(KeyCode.B))
        {
            attacked = 5;
            hasGone = true;
            if (isBlocking)
            {
                blockState = 2;
            }
            else
            {
                blockState = 1;
            }
            isBlocking = true;
        }
        else
        {
            isBlocking = false;
            blockState = 0;
        }
        if (Input.GetKeyDown(KeyCode.Semicolon)){
            //enter /  interact button
        }
        if (!hasGone)
        {
            attacked = 0;
        }
    }
    public void Attacked(float damage, string debuff = "", int debuffTime = 0)
    { // player is attacked(by enemy)
        if (isBlocking && blockCharge > damage)
        {
            blockCharge -= damage;
        }
        else if (isBlocking)
        {
            health -= (int)(damage-blockCharge);
            blockCharge = 0;
        }
        else
        {
            health -= (int)damage; //^ their feelings were hurt v.v
        }
        if (debuffTime != 0)
        {
        }
    }
    void AnimationSetup()
    {
        int finalDir = dirFacing;
        if (dirFacing == 3)
        {
            finalDir = 1;
            SpriteRenderer.flipX = true;
        }
        else
        {
            SpriteRenderer.flipX = false;
        }
        if ((runningX || runningY) && attacked == 0)
        {
            animState = 1;
        }else
        {
            animState = 0;
        }
        if(attacked != 0){
            animState = attacked;
        }
        Debug.Log("animState: " + animState+"; Facing: "+finalDir);
        animator.SetInteger("state", animState);
        animator.SetInteger("facing", finalDir);
        animator.SetInteger("blockState", blockState);
    }
    
    void MovementMechanism()
    {
        hasPressedMoveX = false;
        hasPressedMoveY = false;
        if (Input.GetKey(KeyCode.W))
        {
            if(canGoUp){
                hasPressedMoveY = true;
                if(lastMove.Equals("Up") && lastMoveTime < 20){
                    Dash("Up");
                }else{
                    velocity.y += MOVEMENT_SPEED * Time.deltaTime;
                }
            }
            dirFacing = 0;
        }else if (Input.GetKeyUp(KeyCode.W)){
            lastMove = "Up";
            lastMoveTime = 0;
        }
        if (Input.GetKey(KeyCode.S))
        {
            if(canGoDown){
                hasPressedMoveY = true;
                if(lastMove.Equals("Down") && lastMoveTime < 20){
                    Dash("Down");
                }else{
                    velocity.y -= MOVEMENT_SPEED * Time.deltaTime;
                }
            }
            dirFacing = 2;
        }else if (Input.GetKeyUp(KeyCode.S)){
            lastMove = "Down";
            lastMoveTime = 0;
        }
        if (Input.GetKey(KeyCode.A))
        {   
            if(canGoLeft){
                hasPressedMoveX = true;
                if(lastMove.Equals("Left") && lastMoveTime < 20){
                    Dash("Left");
                }else{
                    velocity.x -= MOVEMENT_SPEED * Time.deltaTime;
                }
            }
            dirFacing = 3;
        }else if (Input.GetKeyUp(KeyCode.A)){
            lastMove = "Left";
            lastMoveTime = 0;
        }
        if (Input.GetKey(KeyCode.D))
        {   
            if(canGoRight){
                hasPressedMoveX = true;
                if(lastMove.Equals("Right") && lastMoveTime < 20){
                    Dash("Right");
                }else{
                    velocity.x += MOVEMENT_SPEED * Time.deltaTime;
                }
            }
            dirFacing = 1;
        }else if (Input.GetKeyUp(KeyCode.D)){
            lastMove = "Right";
            lastMoveTime = 0;
        }
        if(!hasPressedMoveX){
            runningX = false;
        }else{
            runningX = true;
        }
        if(!hasPressedMoveY){
            runningY = false;
        }else{
            runningY = true;
        } 
    }
    void Dash(string Direction){
        if(dashCooldown == 0){
            switch(Direction){
                case "Up":
                    velocity.y += DASH_SPEED;
                    break;
                case "Down":
                    velocity.y -= DASH_SPEED;
                    break;
                case "Left":
                    velocity.x -= DASH_SPEED;
                    break;
                case "Right":
                    velocity.x += DASH_SPEED;
                    break;
            }
            dashCooldown = DASH_COOLDOWN_SET;
        }
    }
    void Attack(string attackStyle){
        float angle;
        float damage = 0.0F;
        string inflict = "";
        switch(dirFacing){
            case 0:
                angle = 0.0F;
                break;
            case 1:
                angle = 90F;
                break;
            case 2:
                angle = 180F;
                break;
            case 3:
                angle = 270F;
                break;
            default:
                angle = 0F;
                break;
        }
        switch(attackStyle){
            case "Heavy":
                damage = 15F;
                attackCooldown = ATTACK_COOLDOWN_H;
                break;
            case "Light":
                damage = 5F;
                attackCooldown = ATTACK_COOLDOWN_L;
                break;
            case "Special":
                damage = 8F;
                specialCooldown = SPECIAL_COOLDOWN;
                break;
            default:
                break;
        }
        Debug.Log("Attack Style = "+attackStyle+"; Damage = "+damage);
        Collider2D[] collidersInRange= Physics2D.OverlapCapsuleAll(new Vector2(transform.position.x,transform.position.y), RANGE_OF_ATTACK, CapsuleDirection2D.Vertical, angle);
        for(int i = 0; i < collidersInRange.Length; i++){
            if(collidersInRange[i].tag == "Enemy"){
                collidersInRange[i].gameObject.GetComponent<EnemyController>().Attacked(damage);
            }else if (collidersInRange[i].tag == "DestructableObject")
            {
                collidersInRange[i].gameObject.GetComponent<DestructableObject>().Attacked(damage);
            }
        }
    }

    void FixedUpdate(){
        if(!runningX){
            if(velocity.x > 0)
            {
                velocity.x -= Mathf.Clamp(Mathf.Abs(velocity.x)*FrictionConstant, 0, Mathf.Abs(velocity.x));
            }
            else if (velocity.x < 0)
            {
                velocity.x += Mathf.Clamp(Mathf.Abs(velocity.x)*FrictionConstant, 0, Mathf.Abs(velocity.x));
            }
        }
        if(!runningY){
            if(velocity.y > 0)
            {
                velocity.y -= Mathf.Clamp(Mathf.Abs(velocity.y)*FrictionConstant, 0, Mathf.Abs(velocity.y));
            }
            else if (velocity.y < 0)
            {
                velocity.y += Mathf.Clamp(Mathf.Abs(velocity.y)*FrictionConstant, 0, Mathf.Abs(velocity.y));
            }
        }
        transform.position += velocity;
        if(dashCooldown > 0){
            dashCooldown--;
        }
        if(attackCooldown > 0){
            attackCooldown--;
        }
        if(specialCooldown > 0){
            specialCooldown--;
        }
        lastMoveTime++;
        if(blockCharge < BLOCK_QUANTITY && !isBlocking)
        {
            blockCharge+=0.5F;
        }
        AnimationSetup();
    }
    void collisionBehavior(Collision2D collision){
        Vector2 collisionVector;
        bool xCollision = false;
        bool yCollision = false;
        for(int i = 0; i < collision.contactCount; i++){
            collisionVector = collision.GetContact(i).normal;
            if(collisionVector.x > 0){
                canGoLeft = false;
                velocity.x = 0.0F;
                xCollision = true;
            }else if (collisionVector.x < 0){
                canGoRight = false;
                velocity.x = 0.0F;
                xCollision = true;
            }
            if(collisionVector.y > 0){
                canGoDown = false;
                velocity.y = 0.0F;
                yCollision = true;
            }else if (collisionVector.y < 0){
                canGoUp = false;
                velocity.y = 0.0F;
                yCollision = true;
            }
        }
        if(!xCollision){
            canGoLeft = true;
            canGoRight = true;
        }
        if(!yCollision)
        {
            canGoUp = true;
            canGoDown = true;
        }
    }
    private void OnCollisionEnter2D(Collision2D collision)
    {
        collisionBehavior(collision);
    }
    private void OnCollisionStay2D(Collision2D collision){
        collisionBehavior(collision);
    }
    private void OnCollisionExit2D(Collision2D collision){
        collisionBehavior(collision);
    }
    public void Run()
    {
        animator.SetInteger("State", 1);
    }
    public void Idle()
    {
        animator.SetInteger("State", 0);
    }
    public void Jump()
    {
        animator.SetInteger("State", 2);
    }
}
