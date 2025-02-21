using Godot;

public partial class GameManager : Node
{
    // Static instance for easy global access (if AutoLoaded in Project Settings).
    public static GameManager Instance { get; private set; }

    // This will hold the player's latest position.
    private Vector2 _playerPosition = Vector2.Zero;

    public override void _Ready()
    {
        if (Instance == null) {
            Instance = this;
        } else {
            QueueFree();
        }
    }

    public async void SwitchScene(string scenePath, string spawnPointName)
    {
        // 1) Change scene, let Godot handle unloading the old scene
        GetTree().ChangeSceneToFile(scenePath);

        // 2) Awaits a game update
        await ToSignal(GetTree(), "process_frame");

        // 3) The new scene is now the current scene
        var newScene = GetTree().CurrentScene;
        if (newScene == null)
        {
            GD.PrintErr("No new scene after loading!");
            return;
        }

        // 4) Grab the Player node
        Player player = newScene.GetNodeOrNull<Player>("Player");
        if (player == null)
        {
            GD.PrintErr("Player node not found in the new scene.");
            return;
        }

        // 5) If a spawn point name is provided, find that node by name
        if (!string.IsNullOrEmpty(spawnPointName))
        {
            // Find a node named "spawnPointName" anywhere in the new scene's hierarchy
            Node2D spawnNode = newScene.FindChild(spawnPointName, true, false) as Node2D;
            if (spawnNode != null)
            {
                player.GlobalPosition = spawnNode.GlobalPosition;
                GD.Print($"Player placed at spawn point: {spawnPointName}");
            }
            else
            {
                GD.PrintErr($"Spawn node \"{spawnPointName}\" not found. Player not moved.");
            }
        }
        else
        {
            GD.Print("No spawn point specified. Player stays where they are.");
        }
    }


    // Save the player's position into this manager
    public void SavePlayerPosition(Vector2 position)
    {
        _playerPosition = position;
    }

    // Retrieve the stored position
    public Vector2 GetPlayerPosition()
    {
        return _playerPosition;
    }
}