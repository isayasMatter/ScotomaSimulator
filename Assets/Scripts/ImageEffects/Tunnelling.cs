﻿using UnityEngine;
using System.Collections;

namespace Sigtrap.ImageEffects {
	[RequireComponent(typeof(Camera))]
	public class Tunnelling : MonoBehaviour {
		#region Public Fields
		[Tooltip("Remove for plain black effect.")]
		public Cubemap skybox;

		[Header("Effect Settings")]
		/// <summary>
		/// Screen coverage at max angular velocity.
		/// </summary>
		[Range(0f,1f)][Tooltip("Screen coverage at max angular velocity.\n(1-this) is radius of visible area at max effect (screen space).")]
		public float maxEffect = 0.75f;

		/// <summary>
		/// Feather around cut-off as fraction of screen.
		/// </summary>
		[Range(0f, 0.5f)][Tooltip("Feather around cut-off as fraction of screen.")]
		public float feather = 0.1f;

		/// <summary>
		/// Smooth out radius over time. 0 for no smoothing.
		/// </summary>
		[Tooltip("Smooth out radius over time. 0 for no smoothing.")]
		public float smoothTime = 0.15f;
		#endregion

		#region Smoothing
		private float _avSlew;
		private float _av;
		#endregion

		#region Shader property IDs
		private int _propAV;
		private int _propFeather;
		#endregion

		#region Eye matrices
		Matrix4x4[] _eyeToWorld = new Matrix4x4[2];
		Matrix4x4[] _eyeProjection = new Matrix4x4[2];
		#endregion

		#region Misc Fields
		private Material _m;
		private Camera _cam;
		#endregion

		#region Messages
		void Awake () {
			_m = new Material(Shader.Find("Hidden/Tunnelling"));

			_propAV = Shader.PropertyToID("_AV");
			_propFeather = Shader.PropertyToID("_Feather");

			_cam = GetComponent<Camera>();
		}

		void Update(){
			
			float av;
			av = 1-maxEffect;
			_av = Mathf.SmoothDamp(_av, av, ref _avSlew, smoothTime);

			_m.SetFloat(_propAV, _av);
			_m.SetFloat(_propFeather, feather);
		}

		void OnPreRender(){
			// Update eye matrices
			Matrix4x4 local;
			#if UNITY_2017_2_OR_NEWER
			if (UnityEngine.XR.XRSettings.enabled) {
			#else
			if (UnityEngine.VR.VRSettings.enabled) {
			#endif
				local = _cam.transform.parent.worldToLocalMatrix;
			} else {
				local = Matrix4x4.identity;
			}

			_eyeProjection[0] = _cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Left);
			_eyeProjection[1] = _cam.GetStereoProjectionMatrix(Camera.StereoscopicEye.Right);
			_eyeProjection[0] = GL.GetGPUProjectionMatrix(_eyeProjection[0], true).inverse;
			_eyeProjection[1] = GL.GetGPUProjectionMatrix(_eyeProjection[1], true).inverse;
			
			_eyeProjection[0][1, 1] *= -1f;
			_eyeProjection[1][1, 1] *= -1f;

			// Hard-code far clip
			_eyeProjection[0][3, 3] = 0.001f;
			_eyeProjection[1][3, 3] = 0.001f;

			_eyeToWorld[0] = _cam.GetStereoViewMatrix(Camera.StereoscopicEye.Left);
			_eyeToWorld[1] = _cam.GetStereoViewMatrix(Camera.StereoscopicEye.Right);

			_eyeToWorld[0] = local * _eyeToWorld[0].inverse;
			_eyeToWorld[1] = local * _eyeToWorld[1].inverse;

			_m.SetMatrixArray("_EyeProjection", _eyeProjection);
			_m.SetMatrixArray("_EyeToWorld", _eyeToWorld);

			// Update skybox
			if (skybox){
				_m.SetTexture("_Skybox", skybox);
				_m.EnableKeyword("TUNNEL_SKYBOX");
			} else {
				_m.DisableKeyword("TUNNEL_SKYBOX");
			}
		}

		void OnRenderImage(RenderTexture src, RenderTexture dest){
			Graphics.Blit(src, dest, _m);
		}

		void OnDestroy(){
			Destroy(_m);
		}
		#endregion
	}
}