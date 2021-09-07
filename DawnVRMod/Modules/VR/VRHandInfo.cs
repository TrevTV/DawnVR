using System;
using UnityEngine;

namespace DawnVR.Modules.VR
{
    internal class VRHandInfo : MonoBehaviour
    {
        public static Texture2D ChloeTexture;
        public static Texture2D MaxTexture;
        public static Mesh[] ChloeHandMeshes = new Mesh[2];
        public static Mesh[] MaxHandMeshes = new Mesh[2];

        private int hand;
        private MeshFilter meshFilter;
        private MeshRenderer meshRenderer;

        private void Start()
        {
            if (name.Contains("left")) hand = 0;
            else if (name.Contains("right")) hand = 1;

            meshFilter = transform.Find("CustomModel").GetComponent<MeshFilter>();
            meshRenderer = transform.Find("CustomModel").GetComponent<MeshRenderer>();
            ChangeModel(HandModel.Chloe);
            ChangeMaterial(HandMaterial.Dithered);
        }

        public void ChangeModel(HandModel model)
        {
            switch (model)
            {
                case HandModel.Chloe:
                    meshFilter.mesh = ChloeHandMeshes[hand];
                    meshRenderer.sharedMaterial.mainTexture = ChloeTexture;                   
                    break;
                case HandModel.Max:
                    meshFilter.mesh = MaxHandMeshes[hand];
                    meshRenderer.sharedMaterial.mainTexture = MaxTexture;
                    break;
            }
        }

        public void ChangeMaterial(HandMaterial material)
        {
            switch (material)
            {
                case HandMaterial.Dithered:
                    meshRenderer.sharedMaterial.shader = Resources.DitheredHandMaterial.shader;
                    break;
                case HandMaterial.Standard:
                    meshRenderer.sharedMaterial.shader = VRRig.Instance.ChloeMaterial.shader;
                    break;
            }
        }

        public enum HandModel
        {
            Chloe,
            Max
        }

        public enum HandMaterial
        {
            Dithered,
            Standard
        }
    }
}
