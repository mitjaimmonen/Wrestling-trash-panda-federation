using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public Transform[] players;
    public float lerpSpeed = 10f;
    public float distanceMargin = 1.0f;
    public float minDistance = 5f;

    private Vector3 middlePoint;
    private float distanceFromMiddlePoint;
    private float distanceBetweenPlayers;
    private float cameraDistance;
    private float aspectRatio;
    private float fov;
    private float tanFov;

    void Start() {
        aspectRatio = Screen.width / Screen.height;
        tanFov = Mathf.Tan(Mathf.Deg2Rad * Camera.main.fieldOfView / 2.0f);
    }

    void LateUpdate () {
        // Position the camera in the center.
        Vector3 newCameraPos = transform.position;
        newCameraPos.x = middlePoint.x;
        transform.position = newCameraPos;

        // Find the middle point between players.
        Vector3 sum = Vector3.zero;
        float maxMagnitude = 0;
        foreach (var player in players)
        {
            sum += player.position;
        }
        sum /= players.Length;
        foreach (var player in players)
        {
            maxMagnitude = Mathf.Max(maxMagnitude, (player.position-sum).magnitude);
        }
        middlePoint = sum;
        // Vector3 vectorBetweenPlayers = player2.position - player1.position;
        // middlePoint = player1.position + 0.5f * vectorBetweenPlayers;

        // Calculate the new distance.
        // distanceBetweenPlayers = vectorBetweenPlayers.magnitude;
        distanceBetweenPlayers = maxMagnitude*2f;
        cameraDistance = Mathf.Max(minDistance, (distanceBetweenPlayers / 2.0f / aspectRatio) / tanFov);

        // Set camera to new position.
        Vector3 dir = (transform.position - middlePoint).normalized;
        transform.position = Vector3.Lerp(transform.position, middlePoint + dir * (cameraDistance + distanceMargin), Time.deltaTime * lerpSpeed);
        transform.LookAt(middlePoint, Vector3.up);
    }
}
