//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Example menu using OnGUI with SteamVR_Camera's overlay support
//
//=============================================================================

using UnityEngine;
using Valve.VR;

namespace Valve.VR
{
    public class SteamVR_Menu : MonoBehaviour
    {
#if REMASTER
        public SteamVR_Menu(System.IntPtr ptr) : base(ptr) { }
#endif

        public Texture cursor, background, logo;
        public float logoHeight, menuOffset;

        public Vector2 scaleLimits = new Vector2(0.1f, 5.0f);
        public float scaleRate = 0.5f;

        SteamVR_Overlay overlay;
        Camera overlayCam;
        Vector4 uvOffset;
        float distance;

        public RenderTexture texture { get { return overlay ? overlay.texture as RenderTexture : null; } }
        public float scale { get; private set; }

        string scaleLimitX, scaleLimitY, scaleRateText;

        CursorLockMode savedCursorLockState;
        bool savedCursorVisible;

        void Awake()
        {
            scaleLimitX = string.Format("{0:N1}", scaleLimits.x);
            scaleLimitY = string.Format("{0:N1}", scaleLimits.y);
            scaleRateText = string.Format("{0:N1}", scaleRate);

            var overlay = SteamVR_Overlay.instance;
            if (overlay != null)
            {
                uvOffset = overlay.uvOffset;
                distance = overlay.distance;
            }
        }

        public void ShowMenu()
        {
            var overlay = SteamVR_Overlay.instance;
            if (overlay == null)
                return;

            var texture = overlay.texture as RenderTexture;
            if (texture == null)
            {
                MelonLoader.MelonLogger.Error("[SteamVR] Menu requires overlay texture to be a render texture.", this);
                return;
            }

            SaveCursorState();

            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            this.overlay = overlay;
            uvOffset = overlay.uvOffset;
            distance = overlay.distance;

            // If an existing camera is rendering into the overlay texture, we need
            // to temporarily disable it to keep it from clearing the texture on us.
            var cameras = Object.FindObjectsOfType<Camera>();
            foreach (var cam in cameras)
            {
                if (cam.enabled && cam.targetTexture == texture)
                {
                    overlayCam = cam;
                    overlayCam.enabled = false;
                    break;
                }
            }

            var tracker = SteamVR_Render.Top();
            if (tracker != null)
                scale = tracker.origin.localScale.x;
        }

        public void HideMenu()
        {
            RestoreCursorState();

            if (overlayCam != null)
                overlayCam.enabled = true;

            if (overlay != null)
            {
                overlay.uvOffset = uvOffset;
                overlay.distance = distance;
                overlay = null;
            }
        }

        void Update()
        {
        }

        void SetScale(float scale)
        {
            this.scale = scale;

            var tracker = SteamVR_Render.Top();
            if (tracker != null)
                tracker.origin.localScale = new Vector3(scale, scale, scale);
        }

        void SaveCursorState()
        {
            savedCursorVisible = Cursor.visible;
            savedCursorLockState = Cursor.lockState;
        }

        void RestoreCursorState()
        {
            Cursor.visible = savedCursorVisible;
            Cursor.lockState = savedCursorLockState;
        }
    }
}