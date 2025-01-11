using Godot;

public partial class Portal : StaticBody2D {
    private Area2D _playerDetector;
    private bool _playerInPortalArea = false;

    // Added properties for the connected rooms and spawn points
    [Export] public string ConnectedMapPath { get; set; } // Path to the connected room scene
    [Export] public string ConnectedSpawnPointPath { get; set; } // Path to the spawn point in the connected room

    // To track the current room instance
    private Node _currentMapInstance;

    public override void _Ready() {
        // Get the Area2D node (make sure it's a child or sibling of this node in the scene tree)
        _playerDetector = GetNode<Area2D>("PlayerDetector");

        // Connect the body_entered signal to a method
        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
        _playerDetector.BodyExited += OnPlayerDetectorBodyExited;
    }

    public override void _Process(double delta) {
        // Check if the player is in the area and presses the interact key
        if(_playerInPortalArea && Input.IsActionJustPressed("ui_interact")) {
            LoadScene();
        }
    }

    private void OnPlayerDetectorBodyEntered(Node body) {
        // Check if the body is the player
        if(body is Player) {
            _playerInPortalArea = true;
        }
    }

    private void OnPlayerDetectorBodyExited(Node body) {
        // Check if the body is the player
        if(body is Player) {
            _playerInPortalArea = false;
        }
    }

    private void LoadScene() {
        if (string.IsNullOrEmpty(ConnectedMapPath)) {
            GD.PrintErr("Connected room path is not set!");
            return;
        }

        // Load the connected room scene
        PackedScene connectedRoomScene = (PackedScene)GD.Load(ConnectedMapPath);
        if (connectedRoomScene == null) {
            GD.PrintErr($"Failed to load scene at {ConnectedMapPath}");
            return;
        }

        // Use ChangeSceneToPacked to switch to the new scene
        GetTree().ChangeSceneToPacked(connectedRoomScene);

        GD.Print($"Switched to scene: {ConnectedMapPath}");

        // Use a deferred call to place the player at the spawn point after the scene is fully loaded
        CallDeferred(nameof(PostSceneLoad));
    }

    private void PostSceneLoad() {
        GD.Print("Scene fully loaded. Setting up player and spawn point.");

        // Get the current scene
        Node currentScene = GetTree().CurrentScene;

        // Ensure the player is present in the new scene
        Player player = currentScene.GetNodeOrNull<Player>("Player");
        if (player == null) {
            GD.PrintErr("Player node not found in the connected room scene.");
            return;
        }

        GD.Print("Player node found.");

        // If a spawn point is specified, find the spawn point and move the player
        if (!string.IsNullOrEmpty(ConnectedSpawnPointPath)) {
            // Find the spawn point in the current room
            Node2D spawnPoint = currentScene.GetNodeOrNull<Node2D>(ConnectedSpawnPointPath);
            if (spawnPoint == null) {
                GD.PrintErr("Spawn point not found in the connected room.");
                return;
            }

            GD.Print("Spawn point found.");

            // Place the player at the spawn point in the new scene
            player.GlobalPosition = spawnPoint.GlobalPosition;
            GD.Print("Player has been placed at the spawn point in the connected room.");
        } else {
            GD.Print("No spawn point specified, player remains in their current position.");
        }
    }
}