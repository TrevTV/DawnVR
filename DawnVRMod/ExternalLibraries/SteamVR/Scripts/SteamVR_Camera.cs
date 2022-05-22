//======= Copyright (c) Valve Corporation, All rights reserved. ===============
//
// Purpose: Adds SteamVR render support to existing camera objects
//
//=============================================================================

using UnityEngine;
using System.Collections;
using System.Reflection;
using Valve.VR;

#if REMASTER
using UnityEngine.XR;
#else
using XRSettings = UnityEngine.VR.VRSettings;
using XRDevice = UnityEngine.VR.VRDevice;
#endif

namespace Valve.VR
{
    //[RequireComponent(typeof(Camera))]
    public class SteamVR_Camera : MonoBehaviour
    {
#if REMASTER
        public SteamVR_Camera(System.IntPtr ptr) : base(ptr) { }
#endif

        //[SerializeField]
        private Transform _head;
        public Transform head { get { return _head; } }
        public Transform offset { get { return _head; } } // legacy
        public Transform origin { get { return _head.parent; } }

        public Camera camera { get; private set; }

        //[SerializeField]
        private Transform _ears;
        public Transform ears { get { return _ears; } }

        public Ray GetRay()
        {
            return new Ray(_head.position, _head.forward);
        }

        public bool wireframe = false;

#if REMASTER
        static public float sceneResolutionScale
    {
        get { return XRSettings.eyeTextureResolutionScale; }
        set { XRSettings.eyeTextureResolutionScale = value; }
    }
#else
        static public float sceneResolutionScale
        {
            get { return XRSettings.renderScale; }
            set { if (value == 0) return; XRSettings.renderScale = value; }
        }
#endif

        #region Enable / Disable

        void OnDisable()
        {
            return;
            SteamVR_Render.Remove(this);
        }

        void OnEnable()
        {
            return;
            // Bail if no hmd is connected
            var vr = SteamVR.instance;
            if (vr == null)
            {
                if (head != null)
                {
                    head.GetComponent<SteamVR_TrackedObject>().enabled = false;
                }

                enabled = false;
                return;
            }

            // Convert camera rig for native OpenVR integration.
            var t = transform;
            if (head != t)
            {
                //Expand();

                t.parent = origin;

                while (head.childCount > 0)
                    head.GetChild(0).parent = t;

                // Keep the head around, but parent to the camera now since it moves with the hmd
                // but existing content may still have references to this object.
                head.parent = t;
                head.localPosition = Vector3.zero;
                head.localRotation = Quaternion.identity;
                head.localScale = Vector3.one;
                head.gameObject.SetActive(false);

                _head = t;
            }

            if (ears == null)
            {
                var e = transform.GetComponentInChildren<SteamVR_Ears>();
                if (e != null)
                    _ears = e.transform;
            }

            if (ears != null)
                ears.GetComponent<SteamVR_Ears>().vrcam = this;

            SteamVR_Render.Add(this);
        }

        #endregion

        #region Functionality to ensure SteamVR_Camera component is always the last component on an object

        void Awake()
        {
            camera = GetComponent<Camera>(); // cached to avoid runtime lookup
            ForceLast();
        }

        static Hashtable values;

        public void ForceLast()
        {
            if (values != null)
            {
                // Restore values on new instance
                foreach (DictionaryEntry entry in values)
                {
                    var f = entry.Key as FieldInfo;
                    f.SetValue(this, entry.Value);
                }
                values = null;
            }
            else
            {
                // Make sure it's the last component
                var components = GetComponents<Component>();

                // But first make sure there aren't any other SteamVR_Cameras on this object.
                for (int i = 0; i < components.Length; i++)
                {
                    var c = components[i] as SteamVR_Camera;
                    if (c != null && c != this)
                    {
                        DestroyImmediate(c);
                    }
                }

                components = GetComponents<Component>();

                if (this != components[components.Length - 1])
                {
                    // Store off values to be restored on new instance
                    values = new Hashtable();
                    var fields = GetType().GetFields(BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.Public);
                    foreach (var f in fields)
                        if (f.IsPublic || f.IsDefined(typeof(SerializeField), true))
                            values[f] = f.GetValue(this);

                    var go = gameObject;
                    DestroyImmediate(this);
                    go.AddComponent<SteamVR_Camera>().ForceLast();
                }
            }
        } 

        #endregion
    }
}