using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;

namespace ZhTool
{
    [RequireComponent(typeof(Camera))]
    public class CameraController : MonoBehaviour
    {
        Vector3 anchor;
        Vector3 anchorCamera;
        bool pulling;

        CameraBoundary boundary;
        [SerializeField] float maxZoomSize = 10, minZoomSize = 3, bleeding = 0;

        Camera cameraA;

        void Awake()
        {
            cameraA = GetComponent<Camera>();
        }

        public void SetBoundary(CameraBoundary newBoundary)
        {
            boundary = new CameraBoundary(
                newBoundary.minX - bleeding,
                newBoundary.maxX + bleeding,
                newBoundary.minY - bleeding,
                newBoundary.maxY + bleeding
            );

            // make sure the max zoom size is not larger than the screen
            float xSpan = boundary.maxX - boundary.minX;
            float ySpan = boundary.maxY - boundary.minY;
            float xSize = xSpan / (2f * cameraA.aspect);
            float ySize = ySpan / 2f;
            maxZoomSize = math.min(maxZoomSize, math.min(xSize, ySize));
        }

        void Update()
        {
            CheckScroll();
            Pull();
        }

        void Pull()
        {
            if (Input.GetKeyDown(KeyCode.Mouse2))
            {
                pulling = true;
                anchor = Input.mousePosition;
                anchorCamera = transform.position;
            }
            else if (Input.GetKeyUp(KeyCode.Mouse2))
            {
                pulling = false;
                return;
            }

            if (!pulling)
                return;

            var deltaWorld = cameraA.ScreenToWorldPoint(Input.mousePosition) - cameraA.ScreenToWorldPoint(anchor);
            Vector3 newPos = anchorCamera - deltaWorld;
            float halfHeight = cameraA.orthographicSize;
            float halfWidth = halfHeight * cameraA.aspect;

            newPos.x = math.clamp(newPos.x, boundary.minX + halfWidth, boundary.maxX - halfWidth);
            newPos.y = math.clamp(newPos.y, boundary.minY + halfHeight, boundary.maxY - halfHeight);

            transform.position = newPos;
            // EventManager.OnCameraMoved?.Invoke();
        }

        void CheckScroll()
        {
            float axis = Input.GetAxis("Mouse ScrollWheel") * 10;
            if (axis < 0.01f && axis > -0.01f)
                return;

            if (axis > 0)
            {
                cameraA.orthographicSize = math.max(minZoomSize, cameraA.orthographicSize - 1);
            }

            if (axis < 0)
            {
                float newSize = math.min(maxZoomSize, cameraA.orthographicSize + 1);
                float halfHeight = newSize;
                float halfWidth = newSize * cameraA.aspect;
                Vector3 pos = transform.position;

                if (pos.x - halfWidth >= boundary.minX &&
                    pos.x + halfWidth <= boundary.maxX &&
                    pos.y - halfHeight >= boundary.minY &&
                    pos.y + halfHeight <= boundary.maxY)
                {
                    cameraA.orthographicSize = newSize;
                }
            }
            // EventManager.OnCameraMoved?.Invoke();
        }

        public struct CameraBoundary
        {
            public float minX;
            public float maxX;
            public float minY;
            public float maxY;


            public CameraBoundary(float minX, float maxX, float minY, float maxY)
            {
                this.minX = minX;
                this.maxX = maxX;
                this.minY = minY;
                this.maxY = maxY;
            }
        }
    }
}