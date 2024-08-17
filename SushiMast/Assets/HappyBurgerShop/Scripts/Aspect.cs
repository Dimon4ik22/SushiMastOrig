using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraFixedAspectRatio : MonoBehaviour
{
    public Vector2 TargetResolution = new Vector2(720, 1280); // ������� ���������� (������ x ������)

    private Camera componentCamera;

    void Start()
    {
        componentCamera = GetComponent<Camera>();
        UpdateCameraViewport();
    }

    void Update()
    {
        UpdateCameraViewport();
    }

    void UpdateCameraViewport()
    {
        // ����������� ������ �������� ����������
        float targetAspect = TargetResolution.x / TargetResolution.y;

        // ������� ����������� ������ ������
        float windowAspect = (float)Screen.width / (float)Screen.height;

        // ���������� ������� ����������� � �������
        float scaleHeight = windowAspect / targetAspect;

        if (scaleHeight < 1.0f) // ����� ����, ��� ������� �����������
        {
            Rect rect = componentCamera.rect;

            rect.width = 1.0f;
            rect.height = scaleHeight;
            rect.x = 0;
            rect.y = (1.0f - scaleHeight) / 2.0f;

            componentCamera.rect = rect;
        }
        else // ����� ����, ��� ������� �����������
        {
            float scaleWidth = 1.0f / scaleHeight;

            Rect rect = componentCamera.rect;

            rect.width = scaleWidth;
            rect.height = 1.0f;
            rect.x = (1.0f - scaleWidth) / 2.0f;
            rect.y = 0;

            componentCamera.rect = rect;
        }
    }
}
