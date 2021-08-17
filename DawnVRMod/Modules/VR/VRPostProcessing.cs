using System;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine._1F1547F66;
using System.Collections.Generic;

namespace DawnVRMod.Modules.VR
{
	[RequireComponent(typeof(Camera))]
	internal class VRPostProcessing : MonoBehaviour
    {
		public T_EEA04AB5 profile;
		public Func<Vector2, Matrix4x4> jitteredMatrixFunc;
		private Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>> m_CommandBuffers;
		private List<T_2AEBE7B4> m_Components;
		private Dictionary<T_2AEBE7B4, bool> m_ComponentStates;
		private Vector3[] m_frustumCorners = new Vector3[4];
		private T_FBB0748 m_MaterialFactory;
		private T_1E4B6FBC m_RenderTextureFactory;
		private T_45EF8C6F m_Context;
		private Camera m_Camera;
		private T_EEA04AB5 m_PreviousProfile;
		private bool m_RenderingInSceneView;
		private T_9A1222A0 m_DebugViews;
		private T_38A0EF37 m_AmbientOcclusion;
		private T_500C5913 m_ScreenSpaceReflection;
		private T_DA9AE9BD m_MotionBlur;
		private T_EE3DF9EA m_Taa;
		private T_DA260798 m_EyeAdaptation;
		private T_94D1D537 m_Bloom;
		private T_67FB24C9 m_ChromaticAberration;
		private T_9029920B m_ColorGrading;
		private T_BE10C434 m_UserLut;
		private T_5D13383C m_Vignette;
		private T_6246D46A m_colorStrokes;
		private T_24E2ACA2 m_Fxaa;
		private T_96C502AA m_DepthOfField;
		private T_C0F7FD02 m_dofPass;
		private List<T_2AEBE7B4> m_ComponentsToEnable = new List<T_2AEBE7B4>();
		private List<T_2AEBE7B4> m_ComponentsToDisable = new List<T_2AEBE7B4>();

		private void OnEnable()
		{
			// todo: set public variables?
			m_CommandBuffers = new Dictionary<Type, KeyValuePair<CameraEvent, CommandBuffer>>();
			m_MaterialFactory = new T_FBB0748();
			m_RenderTextureFactory = new T_1E4B6FBC();
			m_Context = new T_45EF8C6F();
			m_Camera = GetComponent<Camera>();
			m_Components = new List<T_2AEBE7B4>();
			m_DebugViews = AddComponent(new T_9A1222A0());
			m_AmbientOcclusion = AddComponent(new T_38A0EF37());
			m_ScreenSpaceReflection = AddComponent(new T_500C5913());
			m_MotionBlur = AddComponent(new T_DA9AE9BD());
			m_Taa = AddComponent(new T_EE3DF9EA());
			m_EyeAdaptation = AddComponent(new T_DA260798());
			m_Bloom = AddComponent(new T_94D1D537());
			m_ChromaticAberration = AddComponent(new T_67FB24C9());
			m_colorStrokes = AddComponent(new T_6246D46A());
			m_ColorGrading = AddComponent(new T_9029920B());
			m_UserLut = AddComponent(new T_BE10C434());
			m_Vignette = AddComponent(new T_5D13383C());
			m_Fxaa = AddComponent(new T_24E2ACA2());
			m_DepthOfField = AddComponent(new T_96C502AA());
			m_ComponentStates = new Dictionary<T_2AEBE7B4, bool>();
			foreach (T_2AEBE7B4 key in m_Components)
				m_ComponentStates.Add(key, false);
			useGUILayout = false;
		}

		private void OnPreCull()
		{
			if (profile == null || m_Camera == null) return;

			T_24192640.Settings settings = profile.antialiasing.settings;
			T_24192640.FxaaPreset preset = T_24192640.FxaaPreset.Performance;
			if (m_Fxaa != null && m_Fxaa.model != null) m_Fxaa.model.enabled = true;

			switch (T_8E34015F.s_imGraphicsSettings.aaQuality)
			{
				case T_8E34015F.OnOffQualitySetting.kLow:
					settings.method = T_24192640.Method.Fxaa;
					preset = T_24192640.FxaaPreset.Performance;
					break;
				case T_8E34015F.OnOffQualitySetting.kMed:
					settings.method = T_24192640.Method.Fxaa;
					preset = T_24192640.FxaaPreset.Default;
					break;
				case T_8E34015F.OnOffQualitySetting.kHigh:
					settings.method = T_24192640.Method.Fxaa;
					preset = T_24192640.FxaaPreset.ExtremeQuality;
					break;
				case T_8E34015F.OnOffQualitySetting.kHighest:
					settings.method = T_24192640.Method.Fxaa;
					preset = T_24192640.FxaaPreset.ExtremeQuality;
					break;
				case T_8E34015F.OnOffQualitySetting.kOff:
					settings.method = T_24192640.Method.Fxaa;
					break;
			}

			T_24192640.TaaSettings taaSettings = settings.taaSettings;
			taaSettings.jitterSpread = 0.75f;
			taaSettings.stationaryBlending = 0.95f;
			taaSettings.motionBlending = 0.85f;
			taaSettings.sharpen = 0.3f;
			settings.taaSettings = taaSettings;
			profile.antialiasing.settings = settings;

			T_45EF8C6F postProcessingContext = m_Context.Reset();
			postProcessingContext.profile = profile;
			postProcessingContext.renderTextureFactory = m_RenderTextureFactory;
			postProcessingContext.materialFactory = m_MaterialFactory;
			postProcessingContext.camera = m_Camera;

			m_DebugViews.Init(postProcessingContext, profile.debugViews);
			m_AmbientOcclusion.Init(postProcessingContext, profile.ambientOcclusion);
			m_ScreenSpaceReflection.Init(postProcessingContext, profile.screenSpaceReflection);
			m_MotionBlur.Init(postProcessingContext, profile.motionBlur);
			m_Taa.Init(postProcessingContext, profile.antialiasing);
			m_EyeAdaptation.Init(postProcessingContext, profile.eyeAdaptation);
			m_Bloom.Init(postProcessingContext, profile.bloom);
			m_colorStrokes.Init(postProcessingContext, profile.colorStrokes);
			m_ChromaticAberration.Init(postProcessingContext, profile.chromaticAberration);
			m_ColorGrading.Init(postProcessingContext, profile.colorGrading);
			m_UserLut.Init(postProcessingContext, profile.userLut);
			m_Vignette.Init(postProcessingContext, profile.vignette);
			m_Fxaa.Init(postProcessingContext, profile.antialiasing);
			m_DepthOfField.Init(postProcessingContext, profile.depthOfField);

			if (m_PreviousProfile != profile)
			{
				DisableComponents();
				m_PreviousProfile = profile;
			}

			if (settings.method == T_24192640.Method.Fxaa)
			{
				T_24192640.Settings settings2 = m_Fxaa.model.settings;
				T_24192640.FxaaSettings fxaaSettings = settings2.fxaaSettings;
				fxaaSettings.preset = preset;
				settings2.fxaaSettings = fxaaSettings;
				m_Fxaa.model.settings = settings2;
				if (T_8E34015F.s_imGraphicsSettings.aaQuality == T_8E34015F.OnOffQualitySetting.kOff)
					m_Fxaa.model.enabled = false;
			}

			CheckObservers();

			DepthTextureMode depthTextureMode = DepthTextureMode.None;
			foreach (T_2AEBE7B4 postProcessingComponentBase in m_Components)
				if (postProcessingComponentBase.active)
					depthTextureMode |= postProcessingComponentBase.GetCameraFlags();

			postProcessingContext.camera.depthTextureMode = depthTextureMode;
			if (!m_RenderingInSceneView && m_Taa.active && !profile.debugViews.willInterrupt)
				m_Taa.SetProjectionMatrix(jitteredMatrixFunc);
		}

		private void OnPreRender()
		{
			if (profile == null) return;

			TryExecuteCommandBuffer(m_DebugViews);
			TryExecuteCommandBuffer(m_AmbientOcclusion);
			TryExecuteCommandBuffer(m_ScreenSpaceReflection);

			if (!m_RenderingInSceneView)
				TryExecuteCommandBuffer(m_MotionBlur);
		}

		[ImageEffectTransformsToLDR]
		private void OnRenderImage(RenderTexture source, RenderTexture destination)
		{
			if (profile == null || m_Camera == null)
			{
				Graphics.Blit(source, destination);
				return;
			}

			if (m_DepthOfField != null && m_DepthOfField.active)
			{
				if (m_dofPass == null)
					m_dofPass = gameObject.GetComponent<T_C0F7FD02>();
				if (m_dofPass == null)
					m_dofPass = gameObject.AddComponent<T_C0F7FD02>();

				m_dofPass.enabled = true;
			}
			else if (m_dofPass)
				m_dofPass.enabled = false;

			bool someFlagIDontKnowWhat = false;

			Material material = m_MaterialFactory.Get("Hidden/Post FX/Uber Shader");
			material.shaderKeywords = null;

			RenderTexture renderTexture = source;
			if (m_Taa.active && !m_RenderingInSceneView)
			{
				RenderTexture renderTexture2 = m_RenderTextureFactory.Get(renderTexture);
				m_Taa.Render(renderTexture, renderTexture2);
				renderTexture = renderTexture2;
			}

			Texture texture = T_67A00CDD.whiteTexture;
			if (m_EyeAdaptation.active)
			{
				someFlagIDontKnowWhat = true;
				texture = m_EyeAdaptation.Prepare(renderTexture, material);
			}

			material.SetTexture("_AutoExposure", texture);
			if (m_Bloom.active)
			{
				someFlagIDontKnowWhat = true;
				m_Bloom.Prepare(renderTexture, material, texture);
			}

			someFlagIDontKnowWhat |= TryPrepareUberImageEffect(m_colorStrokes, material);
			someFlagIDontKnowWhat |= TryPrepareUberImageEffect(m_ChromaticAberration, material);
			someFlagIDontKnowWhat |= TryPrepareUberImageEffect(m_ColorGrading, material);
			someFlagIDontKnowWhat |= TryPrepareUberImageEffect(m_UserLut, material);

			if (!m_DepthOfField.active)
				someFlagIDontKnowWhat |= TryPrepareUberImageEffect(m_Vignette, material);

			SetupCorners(material);
			Material material2 = (!m_Fxaa.active) ? null : m_MaterialFactory.Get("Hidden/Post FX/FXAA");
			if (m_Fxaa.active)
			{
				material2.shaderKeywords = null;
				if (someFlagIDontKnowWhat)
				{
					RenderTexture renderTexture3 = m_RenderTextureFactory.Get(renderTexture);
					Graphics.Blit(renderTexture, renderTexture3, material, 0);
					renderTexture = renderTexture3;
				}
				m_Fxaa.Render(renderTexture, destination);
			}
			else if (someFlagIDontKnowWhat)
			{
				if (!T_67A00CDD.isLinearColorSpace)
					material.EnableKeyword("UNITY_COLORSPACE_GAMMA");

				Graphics.Blit(renderTexture, destination, material, 0);
			}

			if (!someFlagIDontKnowWhat && !m_Fxaa.active)
				Graphics.Blit(renderTexture, destination);

			m_RenderTextureFactory.ReleaseAll();
		}

		private void OnPostRender()
		{
			if (profile == null || m_Camera == null) return;
			if (!m_RenderingInSceneView && m_Taa.active && !profile.debugViews.willInterrupt)
				m_Context.camera.ResetProjectionMatrix();
		}

		private void OnDisable()
		{
			foreach (KeyValuePair<CameraEvent, CommandBuffer> keyValuePair in m_CommandBuffers.Values)
			{
				m_Camera.RemoveCommandBuffer(keyValuePair.Key, keyValuePair.Value);
				keyValuePair.Value.Dispose();
			}

			m_CommandBuffers.Clear();

			if (profile != null)
				DisableComponents();

			m_Components.Clear();

			if (m_Camera != null)
				m_Camera.depthTextureMode = DepthTextureMode.None;

			m_MaterialFactory.Dispose();
			m_RenderTextureFactory.Dispose();
			T_67A00CDD.Dispose();
		}

		private void SetupCorners(Material uberMaterial)
		{
			Camera camera = m_Camera;
			Transform transform = camera.transform;
			camera.CalculateFrustumCorners(new Rect(0f, 0f, 1f, 1f), camera.farClipPlane, camera.stereoActiveEye, m_frustumCorners);
			Vector3 v = transform.TransformVector(m_frustumCorners[0]);
			Vector3 v2 = transform.TransformVector(m_frustumCorners[1]);
			Vector3 v3 = transform.TransformVector(m_frustumCorners[2]);
			Vector3 v4 = transform.TransformVector(m_frustumCorners[3]);
			Matrix4x4 identity = Matrix4x4.identity;
			identity.SetRow(0, v);
			identity.SetRow(1, v4);
			identity.SetRow(2, v2);
			identity.SetRow(3, v3);
			Vector3 position = transform.position;
			uberMaterial.SetMatrix("_FrustumCornersWS", identity);
			uberMaterial.SetVector("_CameraWS", position);
		}

		private void CheckObservers()
		{
			foreach (var keyValuePair in m_ComponentStates)
			{
				T_2AEBE7B4 key = keyValuePair.Key;
				bool enabled = key.GetModel().enabled;
				if (enabled != keyValuePair.Value)
				{
					if (enabled) m_ComponentsToEnable.Add(key);
					else m_ComponentsToDisable.Add(key);
				}
			}

			for (int i = 0; i < m_ComponentsToDisable.Count; i++)
			{
				T_2AEBE7B4 postProcessingComponentBase = m_ComponentsToDisable[i];
				m_ComponentStates[postProcessingComponentBase] = false;
				postProcessingComponentBase.OnDisable();
			}

			for (int j = 0; j < m_ComponentsToEnable.Count; j++)
			{
				T_2AEBE7B4 postProcessingComponentBase2 = m_ComponentsToEnable[j];
				m_ComponentStates[postProcessingComponentBase2] = true;
				postProcessingComponentBase2.OnEnable();
			}

			m_ComponentsToDisable.Clear();
			m_ComponentsToEnable.Clear();
		}

		private void DisableComponents()
		{
			foreach (T_2AEBE7B4 postProcessingComponentBase in m_Components)
			{
				T_46CC3035 model = postProcessingComponentBase.GetModel();
				if (model != null && model.enabled)
					postProcessingComponentBase.OnDisable();
			}
		}

		private CommandBuffer GetCommandBuffer<T>(CameraEvent evt, string name) where T : T_46CC3035
		{
			KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;
			CommandBuffer result;

			if (!m_CommandBuffers.TryGetValue(typeof(T), out keyValuePair))
				result = AddCommandBuffer<T>(evt, name);
			else if (keyValuePair.Key != evt)
			{
				RemoveCommandBuffer<T>();
				result = AddCommandBuffer<T>(evt, name);
			}
			else
				result = keyValuePair.Value;

			return result;
		}

		private CommandBuffer AddCommandBuffer<T>(CameraEvent evt, string name) where T : T_46CC3035
		{
			CommandBuffer value = new CommandBuffer();
			value.name = name;

			KeyValuePair<CameraEvent, CommandBuffer> value2 = new KeyValuePair<CameraEvent, CommandBuffer>(evt, value);
			m_CommandBuffers.Add(typeof(T), value2);
			m_Camera.AddCommandBuffer(evt, value2.Value);

			return value2.Value;
		}

		private void RemoveCommandBuffer<T>() where T : T_46CC3035
		{
			Type typeFromHandle = typeof(T);
			KeyValuePair<CameraEvent, CommandBuffer> keyValuePair;

			if (!m_CommandBuffers.TryGetValue(typeFromHandle, out keyValuePair))
				return;

			m_Camera.RemoveCommandBuffer(keyValuePair.Key, keyValuePair.Value);
			m_CommandBuffers.Remove(typeFromHandle);
			keyValuePair.Value.Dispose();
		}

		private void TryExecuteCommandBuffer<T>(T_1B23EBD8<T> component) where T : T_46CC3035
		{
			if (component.active)
			{
				CommandBuffer commandBuffer = GetCommandBuffer<T>(component.GetCameraEvent(), component.GetName());
				commandBuffer.Clear();
				component.PopulateCommandBuffer(commandBuffer);
			}
			else RemoveCommandBuffer<T>();
		}

		public bool TryPrepareUberImageEffect<T>(T_32FC32A8<T> component, Material material) where T : T_46CC3035
		{
			if (!component.active)
				return false;

			component.Prepare(material);
			return true;
		}

		private T AddComponent<T>(T component) where T : T_2AEBE7B4
		{
			m_Components.Add(component);
			return component;
		}
	}
}