using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    public Vector3 velocity;
    private float FrictionConstant = 0.007F;
    Animator animator;
    bool running = false;
    int dirFacing = 0; // 0 is up, 1 is right, 2 is down, 3 is left
    SpriteRenderer SpriteRenderer;
    bool hasPressedMove;
    public int health = 0;
    public int maxHealth = 0;
    private bool canGoLeft = true;
    private bool canGoRight = true;
    private bool canGoUp = true;
    private bool canGoDown = true;
    public float movementSpeed;

    // Start is called before the first frame update
    void Start()
    {
        SpriteRenderer = GetComponent<SpriteRenderer>();
        velocity = new Vector3(0F, 0F, 0F);
        movementSpeed = 0.2F;
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        hasPressedMove = false;
        if (Input.GetKey(KeyCode.W))
        {
            if(canGoUp){
                hasPressedMove = true;
                velocity.y += movementSpeed * Time.deltaTime;
                running = true;
            }
        }
        if (Input.GetKey(KeyCode.S))
        {
            if(canGoDown){
                hasPressedMove = true;
                velocity.y -= movementSpeed * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.A))
        {   
            if(canGoLeft){
                hasPressedMove = true;
                velocity.x -= movementSpeed * Time.deltaTime;
            }
        }
        if (Input.GetKey(KeyCode.D))
        {   
            if(canGoRight){
                hasPressedMove = true;
                velocity.x += movementSpeed * Time.deltaTime;
            }
        }
        if(hasPressedMove == false){
            running = false;
        }
    }
    void FixedUpdate(){
        if(!running){
            if(velocity.x > 0)
            {
                velocity.x -= Mathf.Clamp(FrictionConstant, 0, Mathf.Abs(velocity.x));
            }
            else if (velocity.x < 0)
            {
                velocity.x += Mathf.Clamp(FrictionConstant, 0, Mathf.Abs(velocity.x));
            }
        }
        transform.position += velocity;
    }
    void collisionBehavior(Collision2D collision){
        Vector2 collisionVector;
        bool xCollision = false;
        bool yCollision = false;
        for(int i = 0; i < collision.contactCount; i++){
            collisionVector = collision.GetContact(i).normal;
            if(collisionVector.x > 0){
                canGoRight = false;
                velocity.x = 0.0F;
                xCollision = true;
            }else if (collisionVector.x < 0){
                canGoLeft = false;
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
