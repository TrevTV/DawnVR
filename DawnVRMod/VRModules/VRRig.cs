using UnityEngine;
using System.Linq;

namespace DawnVR
{
    internal class VRRig : MonoBehaviour
    {
        public static VRRig Instance;
        public VRCamera Camera;
        public VRInput Input;

        private Camera mainMenuCam;
        private T_C3DD66D9 cachedChloe;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            Camera = transform.Find("Camera").gameObject.AddComponent<VRCamera>();
            Input = new VRInput();
        }

        public void UpdateRigParent(string currentScene)
        {
            // todo: try using the .Where Linq thing to find the clone, not the prefab as that may be the issue
            // maybe just patch CharController's awake or smth
            MelonLoader.MelonLogger.Msg("Current CachedChloe: " + cachedChloe.name ?? "NULL");
            MelonLoader.MelonLogger.Msg("Potential new CachedChloe: " + GameObject.FindObjectOfType<T_C3DD66D9>().name ?? "NULL");
            if (cachedChloe != GameObject.FindObjectOfType<T_C3DD66D9>())
                cachedChloe = GameObject.FindObjectOfType<T_C3DD66D9>();

            // todo: find the player more reliably, this will usually be null due to cutscenes
            if (cachedChloe != null)
            {
                SetParent((Transform)typeof(_1EB728BCC.T_A7E3390E).GetField("_123859A1E").GetValue(cachedChloe.gameObject.GetComponent<_1EB728BCC.T_A7E3390E>()), new Vector3(0, -1, 0));
            }
            else if (currentScene.ToLower().Contains("menu"))
            {
                if (mainMenuCam == null)
                {
                    foreach (Camera cam in GameObject.Find("Scene_Root").GetComponentsInChildren<Camera>())
                        if (cam.name.Contains("LISCamera"))
                            mainMenuCam = cam;
                }

                transform.rotation = Quaternion.Euler(0, 85, 0);
                transform.position = mainMenuCam.transform.position;
            }
        }

        private void SetParent(Transform t, Vector3? newLocalPosition = null, bool resetPos = true)
        {
            transform.parent = t;
            if (resetPos)
            {
                if (newLocalPosition.HasValue)
                    transform.localPosition = newLocalPosition.Value;
                else
                    transform.localPosition = Vector3.zero;
            }
        }

        private void NotALateUpdate()
        {
            if (T_A6E913D1.Instance.m_mainCharacter != null)
                transform.position = T_A6E913D1.Instance.m_mainCharacter.transform.position;
            else if (T_A6E913D1.Instance.m_gameModeManager.CurrentMode == eGameMode.kMainMenu)
            {
                if (mainMenuCam == null)
                {
                    foreach (Camera cam in GameObject.Find("Scene_Root").GetComponentsInChildren<Camera>())
                        if (cam.name.Contains("LISCamera"))
                            mainMenuCam = cam;
                }
                transform.position = mainMenuCam.transform.position;
            }
            else
                transform.position = T_A6E913D1.Instance.m_followCamera.transform.position;
        }
    }
}