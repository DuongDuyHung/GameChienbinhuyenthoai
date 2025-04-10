using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParallackEffect : MonoBehaviour
{
    public Camera cam;
    public Transform followTarget;
    Vector2 startingPosition;
    float startingZ;
    // Start is called before the first frame update
    Vector2 camMoveSinceStart => (Vector2)cam.transform.position;
    float zDistanceFromtarget =>transform.position.z - followTarget.transform.position.z;

    float clippingPlane => (cam.transform.position.z + (zDistanceFromtarget > 0 ? cam.farClipPlane : cam.nearClipPlane));

    float parallaxFactor => Mathf.Abs(zDistanceFromtarget) / clippingPlane;
    void Start()
    {
        startingPosition = transform.position;
        startingZ = transform.position.z;
        
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 newPosition = startingPosition + camMoveSinceStart *parallaxFactor;

        //vi tri x, y thay doi dua tren toc do di chuyen cua player
        transform.position = new Vector3(newPosition.x, newPosition.y, startingZ);
    }
}
