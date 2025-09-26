using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCam : MonoBehaviour {
    static public GameObject POI; // The static point of interest

    [Header("Inscribed")]
    public float easing = 0.05f;             // How fast the camera follows
    public Vector2 minXY = Vector2.zero;     // Minimum x and y limits

    [Header("Dynamic")]
    public float camZ;                       // The desired Z pos of the camera 

    void Awake() {
        camZ = this.transform.position.z;
    }

    void FixedUpdate () {
        // Start with destination at [0,0,0] (the slingshot/origin)
        Vector3 destination = Vector3.zero;

        if (POI != null) {
            // If the POI has a Rigidbody, check if it’s sleeping
            Rigidbody poiRigid = POI.GetComponent<Rigidbody>();
            if ((poiRigid != null) && poiRigid.IsSleeping()) {
                POI = null; // Reset POI once projectile stops moving
            }
        }

        if (POI != null) {
            // Follow the POI while it’s active
            destination = POI.transform.position;
        }

        // Limit the minimum values of destination.x & destination.y
        destination.x = Mathf.Max(minXY.x, destination.x);
        destination.y = Mathf.Max(minXY.y, destination.y);

        // Smoothly interpolate toward the destination
        destination = Vector3.Lerp(transform.position, destination, easing);

        // Keep camera’s z fixed
        destination.z = camZ;

        // Move camera
        transform.position = destination;

        // 👇 Dynamically adjust zoom so ground always stays in view
        Camera.main.orthographicSize = destination.y + 10;
    }

    // void Start() {…}  // Remove unused methods
    // void Update() {…}
}
