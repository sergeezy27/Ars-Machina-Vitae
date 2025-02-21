using Godot;

public partial class Portal : StaticBody2D {
    private Area2D _playerDetector;
    private bool _playerInPortalArea = false;

    [Export] public string ConnectedMapPath { get; set; }
    [Export] public string ConnectedSpawnPointName { get; set; }

    // Runs once when the node and all its children enter the scene.
    // Good for instantiating variables 
    public override void _Ready() {
        _playerDetector = GetNode<Area2D>("PlayerDetector");

        _playerDetector.BodyEntered += OnPlayerDetectorBodyEntered;
        _playerDetector.BodyExited += OnPlayerDetectorBodyExited;
    }

    // Runs every frame
    public override void _Process(double delta) {
        if (_playerInPortalArea && Input.IsActionJustPressed("ui_interact")) {
            GameManager.Instance.SwitchScene(ConnectedMapPath, ConnectedSpawnPointName);
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
}