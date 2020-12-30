using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private void OnEnable()
    {
#if UNITY_EDITOR
        LevelEditorUIController.OnWidthAndHeightSet += OnWidthAndHeightSet;
#endif
        LevelManager.OnWidthAndHeightSet += OnWidthAndHeightSet;
    }

    private void OnDisable()
    {
#if UNITY_EDITOR
        LevelEditorUIController.OnWidthAndHeightSet -= OnWidthAndHeightSet;
#endif
        LevelManager.OnWidthAndHeightSet -= OnWidthAndHeightSet;
    }
    private void OnWidthAndHeightSet(int width, int height)
    {
        Camera cam = Camera.main;
        //camera x,z  Pos
        float cameraXPos = -99;
        float cameraZPos = -99;
        if (width % 2 == 0) cameraXPos = width / 2f - 1.5f;
        else cameraXPos = width / 2 - 1;
        if (height % 2 == 0) cameraZPos = height / 2f - 1.5f;
        else cameraZPos = height / 2 - 1;

        //camera y Pos
        float aspectRatio = (float)Screen.height / Screen.width;
        float minVerticalLongitude = (width + 2) * aspectRatio; // +2 .. sağdan 1 soldan 1 demek
        if (height + 6 > minVerticalLongitude) minVerticalLongitude = height + 6; // +6 sağdan 3 soldan 3 demek
        float cameraYPos = minVerticalLongitude * Mathf.Cos((cam.fieldOfView / 2) * Mathf.Deg2Rad);

        //camera X angle 
        float angle = 10f;
        cameraZPos -= Mathf.Sin(angle * Mathf.Deg2Rad) * cameraYPos;
        cam.transform.eulerAngles = new Vector3(90 - angle, 0, 0);

        //Set Camera position
        cam.transform.position = new Vector3(cameraXPos, cameraYPos, cameraZPos);
    }
}
