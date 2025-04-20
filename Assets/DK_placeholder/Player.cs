using UnityEngine;
//using System.Collections.Generic;
using UnityEngine.U2D.Animation;

public class Player : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;
    public Sprite[] runSprites;
    public Sprite climbSprite;
    private int spriteIndex;

    private Rigidbody2D myRigidBody;
    private new Collider2D collider;
    private Vector2 direction;

    public float moveSpeed = 3f;
    public float jumpStrength = 4f; 

    private LayerMask groundLayer;
    private LayerMask ladderLayer;
    
    private bool grounded;
    private bool climbing;

    // this is only needed if the player can colide with more than one "Ground" layer object
    // private List<Collider2D> groundCollisions;
    //private ContactFilter2D contactFilter;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        myRigidBody = GetComponent<Rigidbody2D>();
        collider = GetComponent<Collider2D>();
        groundLayer = LayerMask.GetMask("Ground");
        ladderLayer = LayerMask.GetMask("Ladder");

        // only if the player can collide with more than one "Ground" layer object
        // groundCollisions = new List<Collider2D>(2); // start with 4, dynamically resizes if needed
        // contactFilter = new ContactFilter2D();
        // contactFilter.SetLayerMask(groundLayer);
        // contactFilter.useLayerMask = true;
        // contactFilter.useTriggers = false;
    }

    private void OnEnable()
    {
        InvokeRepeating(nameof(AnimateSprite), 1f/12f, 1f/12f);
    }

    private void OnDisable()
    {
        CancelInvoke();
    }

    private void CheckCollision()
    {
        grounded = false;
        climbing = false;

        Vector2 size = collider.bounds.size;
        size.y += 0.1f;
        size.x /= 2f;

        Collider2D collidedGround = Physics2D.OverlapBox(transform.position, size, 0f, groundLayer);
        // groundCollisions.Clear();
        // Physics2D.OverlapBox(transform.position, size, 0f, contactFilter, groundCollisions);

        if (collidedGround != null)
        {
            bool isBelow = collidedGround.transform.position.y < (transform.position.y - 0.5f);

            if (isBelow)
            {
                grounded = true;
            }
            // Clip through platforms above
            Physics2D.IgnoreCollision(collider, collidedGround, !isBelow);
        }

        Collider2D collidedLadder = Physics2D.OverlapBox(transform.position, size, 0f, ladderLayer);
        if (collidedLadder != null)
        {
            climbing = true;
        }
    }

    void Update()
    {
        CheckCollision();

        // Climbing and jumping
        if(climbing) {
            direction.y = Input.GetAxis("Vertical") * moveSpeed;
        }else if(grounded && Input.GetButtonDown("Jump")) {
            direction = Vector2.up * jumpStrength;
            //print("Jump pressed! Direction: " + direction);
        }else {
            direction += Physics2D.gravity * Time.deltaTime;
        }

        // Sideways movement
        direction.x = Input.GetAxis("Horizontal") * moveSpeed;
        // Gravity check
        if(grounded) {
            direction.y = Mathf.Max(direction.y, -1f);
        }

        // Flip the sprite based on direction
        if (direction.x > 0f) {
            transform.eulerAngles = Vector3.zero;
        }else if(direction.x < 0f) {
            transform.eulerAngles = new Vector3(0f, 180f, 0f);
        }
    }

    private void FixedUpdate()
    {
        myRigidBody.MovePosition(myRigidBody.position + direction * Time.fixedDeltaTime);
    }

    private void AnimateSprite()
    {
        if(climbing) {
            spriteRenderer.sprite = climbSprite;
        }else if(direction.x != 0) {
            spriteIndex++;
            if(spriteIndex >= runSprites.Length) {
                spriteIndex = 0;
            }
            spriteRenderer.sprite = runSprites[spriteIndex];
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if(collision.gameObject.CompareTag("Objective")) {
            // Bad to do this in practice because it searches the entire scene for this object each frame
            FindFirstObjectByType<GameManager>().LevelComplete();
        }else if(collision.gameObject.CompareTag("Obstacle")) {
            FindFirstObjectByType<GameManager>().LevelFailed();
        }
    }
}