using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float SPEED = 160.0f;
	public const float DECELERATION_SPEED = 3000.0f;

    public override void _PhysicsProcess(double delta)
    {
        // Get the input direction and normalize it
		// consider using non ui direction maps
        Vector2 direction = Input.GetVector("ui_left", "ui_right", "ui_up", "ui_down");

        // Apply speed to the direction
        Vector2 velocity = direction * SPEED;

        // Smooth deceleration when no input is provided
        if (direction == Vector2.Zero)
        {
            velocity = new Vector2(
                Mathf.MoveToward(Velocity.X, 0, DECELERATION_SPEED * (float)delta),
                Mathf.MoveToward(Velocity.Y, 0, DECELERATION_SPEED * (float)delta)
            );
        }

        // Set the velocity and move the player
        Velocity = velocity;
        MoveAndSlide();
    }
}