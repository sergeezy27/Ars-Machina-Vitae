using Godot;

public partial class Portal : StaticBody2D {
    private Area2D _playerDetector;
    private bool _playerInPortalArea = false;

    [Export] public string ConnectedMapPath { get; set; }
    [Export] public string ConnectedSpawnPointPath { get; set; }

    public override void _Ready() {
        _playerDetector = GetNode<Area2D>("PlayerDetector");

        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
        _playerDetector.BodyExited += OnPlayerDetectorBodyExited;
    }

    public override void _Process(double delta) {
        if (_playerInPortalArea && Input.IsActionJustPressed("ui_interact")) {
            LoadScene();
        }
    }

    private void OnPlayerDetectorBodyEntered(Node body) {
        if (body is Player) {
            _playerInPortalArea = true;
        }
    }

    private void OnPlayerDetectorBodyExited(Node body) {
        if (body is Player) {
            _playerInPortalArea = false;
        }
    }

    private void LoadScene() {
        if (string.IsNullOrEmpty(ConnectedMapPath)) {
            GD.PrintErr("Connected room path is not set!");
            return;
        }

        PackedScene connectedRoomScene = (PackedScene)GD.Load(ConnectedMapPath);
        if (connectedRoomScene == null) {
            GD.PrintErr($"Failed to load scene at {ConnectedMapPath}");
            return;
        }

        // Switch to the new scene
        Node newScene = connectedRoomScene.Instantiate();
        GetTree().Root.GetChild(0).QueueFree(); // Free the current scene
        GetTree().Root.AddChild(newScene); // Add the new scene as the main scene
        GetTree().CurrentScene = newScene; // Update the current scene reference

        GD.Print($"Switched to scene: {ConnectedMapPath}");
        SetupPlayer(newScene);
    }

    private void SetupPlayer(Node newScene) {
        GD.Print("Setting up player and spawn point.");

        // Ensure the player is present in the new scene
        Player player = newScene.GetNodeOrNull<Player>("Player");
        if (player == null) {
            GD.PrintErr("Player node not found in the connected room scene.");
            return;
        }

        GD.Print("Player node found.");

        // Find and move the player to the spawn point if specified
        if (!string.IsNullOrEmpty(ConnectedSpawnPointPath)) {
            Node2D spawnPoint = newScene.GetNodeOrNull<Node2D>(ConnectedSpawnPointPath);
            if (spawnPoint == null) {
                GD.PrintErr("Spawn point not found in the connected room.");
                return;
            }

            GD.Print("Spawn point found.");

            player.GlobalPosition = spawnPoint.GlobalPosition;
            GD.Print("Player has been placed at the spawn point in the connected room.");
        } else {
            GD.Print("No spawn point specified, player remains in their current position.");
        }
    }
}