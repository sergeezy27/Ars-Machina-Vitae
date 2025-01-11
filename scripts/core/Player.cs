using Godot;
using System;

public partial class Player : CharacterBody2D
{
    public const float SPEED = 140.0f;
	public const float DECELERATION_SPEED = 3000.0f;
    private Vector2 _spawnPositionInNewScene = new Vector2();

    public override void _Ready()
    {
        // Make sure the Camera2D follows the player
        Camera2D camera = GetNode<Camera2D>("Camera2D");
        camera.Enabled = true;
    }

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

    public void TransitionToNewScene(string scenePath, Vector2 spawnPosition)
    {
        // Set the spawn position for the player in the new scene
        _spawnPositionInNewScene = spawnPosition;

        // Change the scene
        GetTree().ChangeSceneToFile(scenePath);
    }

    // Method to set the player's spawn position (called after the scene is loaded)
    public void SetSpawnPosition(Vector2 spawnPosition)
    {
        Position = spawnPosition;
    }
}