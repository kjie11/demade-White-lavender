using UnityEngine;

public class FootCircleSpawner : MonoBehaviour
{
    public GameObject circlePrefab;
    public float spawnInterval = 0.4f;

    float timer;

    void Update()
    {
        if (IsMoving())
        {
            timer += Time.deltaTime;
            if (timer >= spawnInterval)
            {
                SpawnCircle();
                timer = 0;
            }
        }
    }

    bool IsMoving()
    {
        return new Vector3(
            Input.GetAxis("Horizontal"),
            0,
            Input.GetAxis("Vertical")
        ).magnitude > 0.1f;
    }

    void SpawnCircle()
    {
        Vector3 pos = transform.position;
        pos.y = 0.01f;

        GameObject c = Instantiate(circlePrefab, pos, Quaternion.identity);
        Destroy(c, 1.0f); // 1 秒后消失
    }
}
