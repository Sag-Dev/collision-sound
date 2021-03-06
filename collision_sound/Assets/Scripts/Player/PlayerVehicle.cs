﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class AxleInfo {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class PlayerVehicle : MonoBehaviour
{
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
    public float maxSteeringAngle;
    public float brakeTorque;
    public Rigidbody chasis;
    public float flyUpForce = 80000, flyForwardForce = 2000, flySteertorque = 1000;

    // finds the corresponding visual wheel
    // correctly applies the transform
    public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        if (collider.transform.childCount == 0) {
            return;
        }

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);

        visualWheel.transform.position = position;
        visualWheel.transform.rotation = rotation;
    }

    private void Update() {
        if (Input.GetButton("Fire2")) {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if (Input.GetButton("Cancel")) Application.Quit();
    }

    public void FixedUpdate() {
        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringAngle * Input.GetAxis("Horizontal");
        bool brake = Input.GetButton("Fire1");

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }
            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
                axleInfo.rightWheel.motorTorque = motor;
            }

            axleInfo.rightWheel.brakeTorque = brake ? brakeTorque : 0;
            axleInfo.leftWheel.brakeTorque = brake ? brakeTorque : 0;

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
        }

        if (Input.GetButton("Jump")) {
            chasis.velocity = new Vector3(chasis.velocity.x, 0, chasis.velocity.z);
            chasis.AddForce(flyUpForce * Vector3.up);
            chasis.AddTorque(steering * Vector3.up * flySteertorque);
            Vector3 chasisFixedForward = chasis.transform.forward;
            chasisFixedForward.y = 0;
            chasis.AddForce(chasisFixedForward * motor * flyForwardForce);
        }
    }

    public float getMotorSpeedPercent() {
        return Input.GetAxis("Vertical");
    }
}
