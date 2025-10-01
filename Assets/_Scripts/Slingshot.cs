using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Slingshot : MonoBehaviour {
    [Header("Inscribed")]
    public GameObject projectilePrefab;   // Prefab for the projectile
    public float velocityMult = 10f;      // Multiplier for launch velocity
    public GameObject projLinePrefab;     // Prefab for the projectile trail

    [Header("Dynamic")]
    public GameObject launchPoint;        // Where the projectile spawns
    public Vector3 launchPos;             // Position of the launchPoint
    public GameObject projectile;         // The projectile currently being aimed
    public bool aimingMode;               // Whether the player is aiming
    private Rigidbody projRB;             // Rigidbody of the projectile
    private Vector3 mouseDelta;           // Vector between launchPos and mouse

    void Awake() {
        Transform launchPointTrans = transform.Find("LaunchPoint");
        launchPoint = launchPointTrans.gameObject;
        launchPoint.SetActive(false);
        launchPos = launchPointTrans.position;
    }

    void OnMouseEnter() {
        launchPoint.SetActive(true);
    }

    void OnMouseExit() {
        launchPoint.SetActive(false);
    }

    void OnMouseDown() {
        aimingMode = true;

        // Instantiate a new projectile
        projectile = Instantiate(projectilePrefab) as GameObject;
        projectile.transform.position = launchPos;

        // Make it kinematic so we can drag it without physics interfering
        projRB = projectile.GetComponent<Rigidbody>();
        projRB.isKinematic = true;
    }

    void Update() {
        if (!aimingMode) return;

        // Get current mouse position in 3D
        Vector3 mousePos2D = Input.mousePosition;
        mousePos2D.z = -Camera.main.transform.position.z;
        Vector3 mousePos3D = Camera.main.ScreenToWorldPoint(mousePos2D);

        // Calculate the mouseDelta
        mouseDelta = mousePos3D - launchPos;

        // Clamp the projectile within the launch sphere
        float maxMagnitude = this.GetComponent<SphereCollider>().radius;
        if (mouseDelta.magnitude > maxMagnitude) {
            mouseDelta.Normalize();
            mouseDelta *= maxMagnitude;
        }

        // Move the projectile with the mouse
        projectile.transform.position = launchPos + mouseDelta;

        // Launch on mouse release
        if (Input.GetMouseButtonUp(0)) {
            aimingMode = false;
            projRB.isKinematic = false;

            // Launch the projectile
            projRB.velocity = -mouseDelta * velocityMult;

            // Tell the camera to follow this projectile
            FollowCam.POI = projectile;

            // 👇 Add a ProjectileLine to this projectile
            Instantiate<GameObject>(projLinePrefab, projectile.transform);

            // Reset projectile so we can shoot again later
            projectile = null;
        }
    }
}
