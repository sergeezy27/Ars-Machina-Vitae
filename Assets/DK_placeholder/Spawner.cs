using UnityEngine;

public class Spawner : MonoBehaviour
{
    // prefabricated object: barrel
    public GameObject prefab;
    public float minTime = 2f, maxTime = 4f;

    private void Start()
    {
        Spawn();
    }

    private void Spawn()
    {
        // copy an existing object (the prefab)
        Instantiate(prefab, transform.position, Quaternion.identity);
        Invoke(nameof(Spawn), Random.Range(minTime, maxTime));
    }
}
