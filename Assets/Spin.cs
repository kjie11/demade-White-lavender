using System.Numerics;
using UnityEngine;
using Vector3 = UnityEngine.Vector3;

public class Spin : MonoBehaviour
{
    public float speed=90f;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(Vector3.up*speed*Time.deltaTime);
    }
}
