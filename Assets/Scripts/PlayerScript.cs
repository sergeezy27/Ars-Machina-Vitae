using UnityEngine;

public class PlayerScript : MonoBehaviour
{
    public Rigidbody2D myRigidbody;
    public float movementVelocityMultiplier, movementTime;

    private float timer = 0f;
    private bool isMoving => timer > 0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Handle movement start
        if (!isMoving)
        {
            Vector2 inputDir = GetInputDirection();
            if (inputDir != Vector2.zero)
            {
                myRigidbody.linearVelocity = inputDir * movementVelocityMultiplier;
                timer = 0.001f; // small non-zero value to indicate movement started
            }
        }
        // Handle movement duration
        else
        {
            timer += Time.deltaTime;

            if (timer >= movementTime)
            {
                myRigidbody.linearVelocity = Vector2.zero;
                timer = 0f;
            }
        }
    }

    private Vector2 GetInputDirection()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow)) return Vector2.up;
        if (Input.GetKeyDown(KeyCode.DownArrow)) return Vector2.down;
        if (Input.GetKeyDown(KeyCode.LeftArrow)) return Vector2.left;
        if (Input.GetKeyDown(KeyCode.RightArrow)) return Vector2.right;
        return Vector2.zero;
    }
}
