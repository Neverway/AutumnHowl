using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class BattleCameraManager : MonoBehaviour
{
    public float doAttackEaseSpeed = 12f;
    public float goHomeEaseSpeed = 3f;

    public Transform cameraTransform;
    public Transform onAttackPositionPivot;
    public Transform onAttackPosition;
    public Transform homePosition;

    public bool goBackHome;

    public void Update()
    {
        if (GameInstance.Playerbody != null)
            onAttackPositionPivot.transform.position = GameInstance.Playerbody.transform.position;

        if (goBackHome) EaseCameraToPosition(homePosition.position, homePosition.rotation, goHomeEaseSpeed);
    }

    public void UpdateCameraOnAttack(float pullbackFactor)
    {
        goBackHome = false;

        Vector3 targetPosition = Vector3.Lerp(homePosition.position, onAttackPosition.position, pullbackFactor + 0.4f);
        Quaternion targetRotation = Quaternion.Lerp(homePosition.rotation, onAttackPosition.rotation, pullbackFactor + 0.4f);

        EaseCameraToPosition(targetPosition, targetRotation, doAttackEaseSpeed);
    }
    public void GoBackHome() => goBackHome = true;

    public void EaseCameraToPosition(Vector3 targetPosition, Quaternion targetRotation, float speed)
    {
        cameraTransform.position = Vector3.Lerp(
            cameraTransform.position, targetPosition, 1f - Mathf.Exp(-speed * Time.deltaTime));
        cameraTransform.rotation = Quaternion.Lerp(
            cameraTransform.rotation, targetRotation, 1f - Mathf.Exp(-speed * Time.deltaTime));
    }
}
