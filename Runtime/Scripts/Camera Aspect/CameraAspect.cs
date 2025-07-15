namespace SAGE.Framework.Extensions
{
    using UnityEngine;

    [RequireComponent(typeof(Camera)), ExecuteInEditMode]
    public class CameraAspect : MonoBehaviour
    {
        [SerializeField] float minWidth = 6.75f;

        Camera Camera { get; set; }

        Vector2 screenDimensions = Vector2.zero;

        void Start()
        {
            UpdateCameraAspect();
        }

        void ApplyAspectRatio()
        {
            Camera = GetComponent<Camera>();
            Camera.rect = new Rect(0, 0, 1, 1);
            Camera.ResetAspect();

            float cameraWidth = Camera.GetWidth();
            if (cameraWidth < minWidth)
            {
                ApplyLetterbox(cameraWidth / minWidth);
            }
        }

        void ApplyLetterbox(float ratio)
        {
            Rect rect = new Rect(0, 0, 1, 1);

            rect.height *= ratio;
            rect.y = (1 - rect.height) / 2;
            rect.y = (float)System.Math.Round(rect.y, 5);
            rect.height = (float)System.Math.Round(rect.height, 5);

            Camera.rect = rect;
        }

        private void Update()
        {
            if (screenDimensions.x != Screen.width || screenDimensions.y != Screen.height)
            {
                UpdateCameraAspect();
            }
        }

        void UpdateCameraAspect()
        {
            screenDimensions = new Vector2()
            {
                x = Screen.width,
                y = Screen.height
            };

            ApplyAspectRatio();
        }
    }
}