using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;

namespace Sark.RenderUtils
{
    [ExecuteAlways]
    [RequireComponent(typeof(Camera), typeof(PixelPerfectCamera))]
    public class TiledCamera : MonoBehaviour
    {
        [SerializeField]
        [HideInInspector]
        Camera _camera;
        public Camera Camera => _camera;

        [SerializeField]
        [HideInInspector]
        GameObject _clearCameraGO;

        [SerializeField]
        [HideInInspector]
        Camera _clearCamera;
        public Camera ClearCamera => _clearCamera;

        [SerializeField]
        [HideInInspector]
        PixelPerfectCamera _pixelCam;
        public PixelPerfectCamera PixelCamera => _pixelCam;

        [Header("Viewport")]
        [SerializeField]
        int2 _tileCount = new int2(48, 25);
        public int2 TileCount
        {
            get => _tileCount;
            set
            {
                _tileCount = math.max(1, value);
                UpdateState();
            }
        }

        [SerializeField]
        int2 _tileSize = new int2(8, 8);
        public int2 TileSize
        {
            get => _tileSize;
            set
            {
                _tileSize = math.max(1, value);
                UpdateState();
            }
        }


        [SerializeField]
        Color _clearColor = Color.black;
        public Color BackgroundColor
        {
            get => _clearColor;
            set
            {
                _clearColor = value;
                _clearCamera.backgroundColor = value;
            }
        }

        private void OnEnable()
        {
            if (_camera == null)
            {
                _camera = GetComponent<Camera>();
                _camera.clearFlags = CameraClearFlags.SolidColor;
                _camera.orthographic = true;
            }

            if (_pixelCam == null)
            {
                _pixelCam = GetComponent<PixelPerfectCamera>();

                _pixelCam.upscaleRT = true;
                _pixelCam.cropFrameX = true;
                _pixelCam.cropFrameY = true;
            }

            if (_clearCamera == null)
            {
                if (_clearCameraGO == null)
                {
                    _clearCameraGO = new GameObject("ClearCamera");
                    _clearCameraGO.transform.SetParent(transform);
                }
                _clearCamera = _clearCameraGO.AddComponent<Camera>();
                _clearCamera.orthographic = true;
                _clearCamera.cullingMask = 0;
                _clearCameraGO.hideFlags = HideFlags.HideInHierarchy;
                _clearCamera.clearFlags = CameraClearFlags.SolidColor;
                UpdateState();
            }

#if UNITY_EDITOR
            _pixelCam.runInEditMode = true;
#endif

            UpdateState();
        }

        [ContextMenu("SnapViewportCornerToOrigin")]
        public void SnapViewportCornerToOrigin()
        {
            float h = Camera.orthographicSize * 2;
            float w = h * Camera.aspect;

            Vector3 p = transform.position;
            Vector3 target = new Vector3(w * .5f, h * .5f, p.z);
            if (p != target)
            {
                transform.position = target;
            }
        }

        // Update internal values that might be affected by user state changes
        void UpdateState()
        {
            _clearCamera.depth = _camera.depth - 1;
            _clearCamera.backgroundColor = _clearColor;
            _clearCamera.orthographicSize = _camera.orthographicSize;

            _pixelCam.assetsPPU = _tileSize.y;

            _pixelCam.refResolutionX = _tileCount.x * _tileSize.x;
            _pixelCam.refResolutionY = _tileCount.y * _tileSize.y;
        }

#if UNITY_EDITOR

        [Header("Gizmo Grid")]

        [SerializeField]
        bool _drawGrid;

        [SerializeField]
        Color _gridColorOdd = new Color(.75f, 0, 0, .35f);

        [SerializeField]
        Color _gridColorEven = new Color(.1f, .1f, .1f, .1f);

        private void OnValidate()
        {
            TileCount = _tileCount;
            TileSize = _tileSize;
        }
#endif
    }
}