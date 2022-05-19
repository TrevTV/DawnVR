using UnityEngine;
using DawnVR.Modules.VR;
using System;

#if REMASTER
using DataEditor.Graph;
using PrototyperData.GraphObjects;
using CriMana;
#else
using OverlayCookie = T_A7F99C25;
using UITexture = T_D186D1CC;
using Telescope = T_ADD17E7F;
using TelescopePuzzle = T_24E8F007;
using GameMaster = T_A6E913D1;
using EditableDataObjectGraphedConnection = _169E4A3E._165972D5F.T_7808CA07;
using SequenceGraphObject = _15C6DD6D9._14FDF87F1.T_F45060BF;
using FreeroamGraphObject = _15C6DD6D9._14FDF87F1.T_BBB6DDD9;
using FollowCamera = T_884A92DB;
using Player = _1F28E2E62.T_E579AD8A;
using CriManaMovieMaterial = T_DD163FE9;
using DawnMainCamera = T_34182F31;
#endif

namespace DawnVR.Modules
{
    internal static partial class HarmonyPatches
    {
        private static bool hasDisabledMovieWindow;

        public static void DisableSettingCurrentViewCookie(OverlayCookie.eCookieChoices value)
        {
            foreach (var rend in GameObject.Find("/UIRoot/Camera/OverlayCookie/").GetComponentsInChildren<UITexture>())
                rend.enabled = false;

            SetCurrentViewCookie(value);
        }

        public static void SetCurrentViewCookie(OverlayCookie.eCookieChoices cookie)
        {
            switch (cookie)
            {
                case OverlayCookie.eCookieChoices.kNone:
                    VRRig.Instance.CutsceneHandler.EndCutscene();
                    if (VRRig.Instance?.ChloeComponent?.Camera != null)
                        VRRig.Instance.ChloeComponent.Camera.enabled = false;
                    break;
                case OverlayCookie.eCookieChoices.kBinoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case OverlayCookie.eCookieChoices.kE3Binoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene();
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
                case OverlayCookie.eCookieChoices.kE4Binoculars:
                    VRRig.Instance.CutsceneHandler.SetupCutscene(true);
                    VRRig.Instance.ChloeComponent.Camera.enabled = true;
                    break;
            }
        }

        public static void TelescopeRotate(Telescope __instance)
            => __instance.m_bottle = VRRig.Instance.CutsceneHandler.AmuletGlassTransform.gameObject;

        public static bool TelescopePuzzleUpdate(TelescopePuzzle __instance)
        {
            if (ObfuscationTools.GetFieldValue<bool>(__instance, "initialized"))
            {
                if (ObfuscationTools.GetFieldValue<GameObject>(__instance, "m_Scope").activeInHierarchy)
                {
                    Vector3 localEulerAngles = VRRig.Instance.CutsceneHandler.AmuletGlassTransform.localEulerAngles;
                    float num = __instance.m_targetZRotation + __instance.m_errorRange + 1f;
                    if (localEulerAngles.z >= 180f)
                        num = Mathf.Abs(localEulerAngles.z - 360f - __instance.m_targetZRotation);
                    else
                        num = Mathf.Abs(localEulerAngles.z - __instance.m_targetZRotation);

                    GameStateModel currentModel = GameMaster.Instance.m_gameStateManager.GetCurrentModel();
                    if (currentModel != null)
                        currentModel.SetValue(__instance.m_lensAngleVariable, (int)Mathf.Floor(num), false);

                    num -= __instance.m_errorRange;
                    if (num <= 0f)
                    {
                        Transform camTrans = VRRig.Instance.CutsceneHandler.CurrentCamera.transform;
                        Ray ray = new Ray(camTrans.position, camTrans.forward);
                        ObfuscationTools.SetFieldValue(__instance, "ray", ray);
                        RaycastHit[] array = Physics.RaycastAll(ray, __instance.m_maxRaycastDistance);
                        bool flag = false;
                        for (int i = 0; i < array.Length; i++)
                        {
                            if (array[i].transform.gameObject.GetComponent<TelescopePuzzle>() != null)
                            {
                                flag = true;
                                break;
                            }
                        }
                        if (!flag)
                        {
                            ObfuscationTools.SetFieldValue(__instance, "m_timeHovering", 0f);
                            return false;
                        }
                        ObfuscationTools.SetFieldValue(__instance, "m_timeHovering", ObfuscationTools.GetFieldValue<float>(__instance, "m_timeHovering") + Time.deltaTime);
                        if (ObfuscationTools.GetFieldValue<float>(__instance, "m_timeHovering") >= __instance.m_hoverTime)
                        {
                            var outputConnections = GameMaster.Instance.m_graphManager.CurrentGraphNode.outputConnections;
                            if (outputConnections != null && outputConnections.Count != 0)
                            {
                                SequenceGraphObject t_F45060BF = null;
                                for (int j = 0; j < outputConnections.Count; j++)
                                {
                                    SequenceGraphObject t_F45060BF2 = outputConnections[j].to as SequenceGraphObject;
                                    if (t_F45060BF2 != null)
                                    {
                                        string text = (t_F45060BF2.sequence == null) ? string.Empty : t_F45060BF2.sequence.nodeName;
                                        if (!string.IsNullOrEmpty(text))
                                        {
                                            if (string.Equals(text, __instance.m_nodeName, StringComparison.InvariantCultureIgnoreCase))
                                            {
                                                t_F45060BF = t_F45060BF2;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (t_F45060BF == null)
                                    return false;

                                FreeroamGraphObject frGraphObj = GameMaster.Instance.m_graphManager.CurrentGraphNode as FreeroamGraphObject;
                                if (frGraphObj != null)
                                {
                                    frGraphObj.m_exitGraphObject = t_F45060BF;
                                    GameMaster.Instance.m_graphManager.ExitCurrentNode();
                                }
                            }
                        }
                    }
                }
            }
            else
                //__instance._1B350D183();
                typeof(TelescopePuzzle).GetMethod(ObfuscationTools.GetRealMethodName("Init")).Invoke(__instance, null);

            return false;
        }

        public static bool SetupFollowCameraMatrix(FollowCamera __instance, Vector4 __0, Vector4 __1)
        {
            if (__instance.m_isLineLocked)
            {
                if (__instance.GetFieldValue<bool>("m_reverseControls"))
                {
                    __instance.SetFieldValue("m_rightH", __instance.GetFieldValue<float>("m_rightH") * -1f);
                }
                if (__instance.GetFieldValue<bool>("m_flattenControls"))
                {
                    __instance.MultiplyFloatField("m_rightV", 0f);
                }
                if (__instance.GetFieldValue<float>("m_rightH") >= 0f && __instance.GetFieldValue<float>("m_rightV") >= 0f)
                {
                    __instance.SubtractFromFloatField("m_lineLockLerp", Mathf.Max(__instance.GetFieldValue<float>("m_rightH"), __instance.GetFieldValue<float>("m_rightV")));
                }
                else if (__instance.GetFieldValue<float>("m_rightH") <= 0f && __instance.GetFieldValue<float>("m_rightV") <= 0f)
                {
                    __instance.SubtractFromFloatField("m_lineLockLerp", Mathf.Min(__instance.GetFieldValue<float>("m_rightH"), __instance.GetFieldValue<float>("m_rightV")));
                }
                __instance.SetFieldValue("m_lineLockLerp", Mathf.Clamp01(__instance.GetFieldValue<float>("m_lineLockLerp")));
                __0 = Vector3.Lerp(__instance.GetFieldValue<Vector3>("m_lineFirstLockPoint"), __instance.GetFieldValue<Vector3>("m_lineLastLockPoint"), __instance.GetFieldValue<float>("m_lineLockLerp"));
                //_1DD947C88 += __instance._111890643;
                __instance.SubtractFromV3Field("m_lineLockOffset", __instance.GetFieldValue<Vector3>("m_lineLockOffset") * ((Mathf.Abs(__instance.GetFieldValue<float>("m_rightH")) + Mathf.Abs(__instance.GetFieldValue<float>("m_rightV"))) * Time.deltaTime / 2f));
            }
            else if (__instance.IsLocked)
            {
                float num = -__instance.GetFieldValue<float>("m_rightH") * 57.29578f;
                float num2 = __instance.GetFieldValue<float>("m_rightV") * 57.29578f;
                __instance.AddToFloatField("m_currentLockedHorizontal", num);
                __instance.AddToFloatField("m_currentLockedVertical", num2);
                Transform transform = __instance.GetFieldValue<GameObject>("templateObject").transform;
                if (__instance.GetFieldValue<Vector3>("m_centeredAimDirection") != Vector3.zero)
                {
                    Vector3 cad = __instance.GetFieldValue<Vector3>("m_centeredAimDirection");
                    transform.forward = cad;
                    __instance.SetFieldValue("m_lockedAimAngleOffset", Vector3.Angle(new Vector3(cad.x, 0f, cad.z), __instance.centeredAimDirection));
                    if (cad.y < 0f)
                    {
                        __instance.MultiplyFloatField("m_lockedAimAngleOffset", -1f);
                    }
                }
                else
                {
                    transform.forward = new Vector3(0f, 0f, 1f);
                    __instance.SetFieldValue("m_lockedAimAngleOffset", 0f);
                }
                __instance.SetFieldValue("m_currentLockedVertical", Mathf.Clamp(__instance.GetFieldValue<float>("m_currentLockedVertical"), -89.99f + __instance.GetFieldValue<float>("m_lockedAimAngleOffset"), 89.99f + __instance.GetFieldValue<float>("m_lockedAimAngleOffset")));
                if (__instance.GetFieldValue<bool>("m_isRotationLimited"))
                {
                    Vector4 rpl = __instance.GetFieldValue<Vector4>("m_rotationPivotLimit");
                    if (rpl.x >= 0f && rpl.y >= 0f)
                    {
                        __instance.SetFieldValue("m_currentLockedHorizontal", Mathf.Clamp(__instance.GetFieldValue<float>("m_currentLockedHorizontal"), -rpl.x, rpl.y));
                    }
                    if (rpl.z >= 0f && rpl.w >= 0f)
                    {
                        __instance.SetFieldValue("m_currentLockedVertical", Mathf.Clamp(__instance.GetFieldValue<float>("m_currentLockedVertical"), -rpl.z, rpl.w));
                    }
                }
                if (__instance.GetFieldValue<bool>("m_isPCInput"))
                {
                    __instance.SetFieldValue("m_sAltitudeAngle", __instance.GetFieldValue<float>("m_currentLockedVertical"));
                    __instance.SetFieldValue("m_sOrientAngle", __instance.GetFieldValue<float>("m_currentLockedHorizontal"));
                    transform.forward = Quaternion.AngleAxis(__instance.GetFieldValue<float>("m_currentLockedHorizontal"), Vector3.up) * transform.forward;
                    Vector3 axis = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance.GetFieldValue<float>("m_currentLockedVertical"), axis) * transform.forward;
                }
                else
                {
                    float aav = 0;
                    float oav = 0;
                    __instance.SetFieldValue("m_sAltitudeAngle", Mathf.SmoothDamp(__instance.GetFieldValue<float>("m_sAltitudeAngle"), __instance.GetFieldValue<float>("m_currentLockedVertical"), ref aav, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, __instance.m_maxViewChangeSpeed, __instance.CallMethod<float>("InternalDeltaTime")));
                    __instance.SetFieldValue("AltitudeAngleVelocity", aav);
                    __instance.SetFieldValue("m_sOrientAngle", Mathf.SmoothDamp(__instance.GetFieldValue<float>("m_sOrientAngle"), __instance.GetFieldValue<float>("m_currentLockedHorizontal"), ref oav, __instance.m_camMomentumCarrythrough / __instance.m_joystickSensitivity, float.PositiveInfinity, __instance.CallMethod<float>("InternalDeltaTime")));
                    __instance.SetFieldValue("OrientAngleVelocity", oav);
                    transform.forward = Quaternion.AngleAxis(__instance.GetFieldValue<float>("m_sOrientAngle"), Vector3.up) * transform.forward;
                    Vector3 axis2 = Vector3.Cross(Vector3.up, transform.forward);
                    transform.forward = Quaternion.AngleAxis(__instance.GetFieldValue<float>("m_sAltitudeAngle"), axis2) * transform.forward;
                }
                __instance.transform.forward = transform.forward;
                return false;
            }
            Vector4 vector = __0 - __1;
            vector.Normalize();
            Vector4 vector2 = new Vector4(-vector.z, 0f, vector.x);
            vector2.Normalize();
            Vector4 vector3 = Vector3.Cross(vector2, vector);
            vector3.Normalize();
            vector2 = Vector3.Cross(vector3, vector);
            vector2.Normalize();
            __instance.CallMethod("SetCameraMatrix", setCamMatrixTypes, __1, vector2, vector3, vector);

            // don't ask me what this does because i have no idea. it only exists in the linux port
#if !REMASTER
            if (!__instance.IsLocked)
            {
                __instance._1DD2696DE = 0f;
                __instance._1CE82F9BB = 0f;
            }
#endif
            return false;
        }

        private static readonly Type[] setCamMatrixTypes = new Type[] { typeof(Vector4), typeof(Vector4), typeof(Vector4), typeof(Vector4) };

        public static void OnMovieWillRenderObject(Player __instance, CriManaMovieMaterial __0)
        {
            var status = __instance.GetFieldValue<Player.Status>("status");
            if (status == Player.Status.Ready || status == Player.Status.Playing)
            {
                if (!hasDisabledMovieWindow)
                {
                    GameObject.Find("/UIRoot/Camera/MovieWindow/Widget/Quad").SetActive(false);
                    hasDisabledMovieWindow = true;
                }

                VRRig.Instance.CutsceneHandler.SetupCutscene(__0.material);
            }
        }

        public static bool SetMainCamFOV(float __0)
        {
            if (VRRig.Instance.CutsceneHandler.IsActive)
            {
                VRRig.Instance.CutsceneHandler.CurrentCamera.fieldOfView = __0;
                return false;
            }
            else
                return true;
        }

        public static bool OnSetFOV(Camera __instance, float value)
        {
            if (__instance == DawnMainCamera.main)
            {
                if (VRRig.Instance.CutsceneHandler.IsActive)
                    VRRig.Instance.CutsceneHandler.CurrentCamera.fieldOfView = value;
            }

            if (__instance == VRRig.Instance.CutsceneHandler.CurrentCamera)
                return true;

            return false;
        }
    }
}
