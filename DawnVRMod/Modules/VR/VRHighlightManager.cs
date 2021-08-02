using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityStandardAssets._1CC59503E;

namespace DawnVR.Modules.VR
{
    public class VRHighlightManager : MonoBehaviour
    {
        public static VRHighlightManager Instance;

        public Texture2D m_hatchTexture;
        public Texture2D m_noiseTexture;
        public Texture2D m_outlineNoiseTexture;
        public float m_objectJitter = 1.8f;
        public float m_characterJitter = 0.6f;

        private List<T_A8FCC1F5> m_selectedObject = new List<T_A8FCC1F5>();
        private T_EE22C7E6 m_selectedObjectHlo;
        private Material m_highlightMaterial;
        private bool m_isCharacter;
        private List<Renderer> m_selectedRenderers;
        private T_FA85E78 m_edgeDetection;
        private CommandBuffer m_commandBuffer;
        private bool m_hadSemiTrans;

        private void Start()
        {
            m_edgeDetection = gameObject.AddComponent<T_FA85E78>();
            CopyComponent(T_4679B25C.s_highlightManager.GetComponent<T_FA85E78>(), m_edgeDetection);
            if (m_edgeDetection != null)
            {
                m_edgeDetection.SetTextures(m_hatchTexture, m_noiseTexture, m_outlineNoiseTexture);
                m_edgeDetection.enabled = false;
            }
            m_highlightMaterial = new Material(Shader.Find("Hidden/WriteHighlightIntoOcclusion"));
        }

        public void SelectObject(T_A8FCC1F5 hightlightableObject)
        {
            if (m_selectedObject.Contains(hightlightableObject))
                return;

            CreateCommandBuffer();
            m_selectedObject.Add(hightlightableObject);
            m_selectedObjectHlo = (hightlightableObject as T_EE22C7E6);
            m_isCharacter = (hightlightableObject as T_58F8A8D8 != null);

            if (m_edgeDetection != null)
            {
                if (m_isCharacter)
                    m_edgeDetection.m_outlineJitter = m_characterJitter;
                else
                    m_edgeDetection.m_outlineJitter = m_objectJitter;
            }

            if (m_selectedObjectHlo != null)
            {
                bool flag = false;
                switch (m_selectedObjectHlo.m_highlightMode)
                {
                    case T_4679B25C.HighlightMode.kHatch:
                        flag = true;
                        break;
                    case T_4679B25C.HighlightMode.kOutlineHatch:
                        flag = true;
                        break;
                }

                if (flag && m_edgeDetection != null)
                    m_edgeDetection.m_hatchScale = m_selectedObjectHlo.m_hatchScale;

                m_hadSemiTrans = false;
                if (m_isCharacter)
                    AddRenderersRecursive(m_selectedObject[m_selectedObject.Count - 1].transform.parent);
                else
                    AddRenderersRecursive(m_selectedObject[m_selectedObject.Count - 1].transform);

                if (m_edgeDetection != null)
                {
                    if (m_hadSemiTrans)
                        m_edgeDetection.SetActivePass(5);
                    else
                        m_edgeDetection.SetActivePass((int)m_selectedObjectHlo.m_highlightMode);

                    m_edgeDetection.enabled = true;
                }
            }
            else if (m_edgeDetection != null)
            {
                m_edgeDetection.enabled = false;
            }
        }

        public void DeselectObject(T_A8FCC1F5 hlo)
        {
            if (!m_selectedObject.Contains(hlo))
                return;

            m_selectedObject.Clear();
            m_selectedObjectHlo = null;
            m_isCharacter = false;

            hlo.m_isSelected = false;

            if (m_edgeDetection != null)
                m_edgeDetection.enabled = false;
            if (m_commandBuffer != null)
                m_commandBuffer.Clear();

            for (int i = 0; i < m_selectedRenderers.Count; i++)
            {
                Renderer renderer = m_selectedRenderers[i];
                for (int j = 0; j < renderer.materials.Length; j++)
                    renderer.materials[j].SetFloat("_HighlightMask", 0f);
            }

            m_selectedRenderers.Clear();
        }

        public void DrawMe(T_A8FCC1F5 hlo)
        {
        }

        private void Awake()
        {
            m_selectedRenderers = new List<Renderer>();
        }

        private void AddMaterial(Renderer r, int materialIndex)
        {
            r.materials[materialIndex].SetFloat("_HighlightMask", 2f);
            SkinnedMeshRenderer x = r as SkinnedMeshRenderer;
            if (x == null && r.materials[materialIndex].HasProperty("_Mode"))
            {
                float @float = r.materials[materialIndex].GetFloat("_Mode");
                if (@float > 1f)
                {
                    if (!m_hadSemiTrans)
                    {
                        m_hadSemiTrans = true;
                    }
                    m_commandBuffer.DrawRenderer(r, m_highlightMaterial, materialIndex);
                }
            }
        }

        // Token: 0x06003231 RID: 12849 RVA: 0x00124780 File Offset: 0x00122B80
        private void AddRenderersRecursive(Transform t)
        {
            if (!t.gameObject.activeInHierarchy)
            {
                return;
            }
            Renderer component = t.GetComponent<Renderer>();
            if (component != null)
            {
                m_selectedRenderers.Add(component);
                if (m_selectedObjectHlo == null || m_selectedObjectHlo.meshIndex == -1)
                {
                    for (int i = 0; i < component.materials.Length; i++)
                    {
                        AddMaterial(component, i);
                    }
                }
                else
                {
                    int num = m_selectedObjectHlo.meshIndex;
                    if (num < 0 || num > component.materials.Length - 1)
                    {
                        num = 0;
                    }
                    AddMaterial(component, num);
                }
            }
            for (int j = 0; j < t.childCount; j++)
            {
                AddRenderersRecursive(t.GetChild(j));
            }
        }

        public void OnDisable()
        {
            if (m_commandBuffer != null && VRRig.Instance?.Camera.Camera != null)
            {
                VRRig.Instance?.Camera.Camera.RemoveCommandBuffer(CameraEvent.AfterGBuffer, m_commandBuffer);
                m_commandBuffer = null;
            }
        }

        public void DeselectAll()
        {
            if (m_selectedObject != null && m_selectedObject.Count > 0)
                DeselectObject(m_selectedObject[0]);
        }

        private void CreateCommandBuffer()
        {
            if (m_commandBuffer != null)
                m_commandBuffer.Clear();
            else
            {
                m_commandBuffer = new CommandBuffer();
                m_commandBuffer.name = "OutlineTransparent";
                VRRig.Instance?.Camera.Camera.AddCommandBuffer(CameraEvent.AfterGBuffer, m_commandBuffer);
            }
        }

        public static Component CopyComponent(Component original, Component newcopy)
        {
            foreach (System.Reflection.FieldInfo fieldInfo in original.GetType().GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance))
                fieldInfo.SetValue(newcopy, fieldInfo.GetValue(original));
            return newcopy;
        }
    }
}