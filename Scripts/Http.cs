using System;
using System.Collections;
using System.Collections.Generic;
using Duck.Http.Service;
using Duck.Http.Service.Unity;
using UnityEngine;
using UnityEngine.Networking;

namespace Duck.Http
{
	public sealed class Http : MonoBehaviour
	{
		public static Http Instance
		{
			get
			{
				if (instance != null) return instance;
				Init(new UnityHttpService());
				return instance;
			}
		}

		private static Http instance;

		private IHttpService service;
		private Dictionary<string, string> superHeaders;
		private Dictionary<IHttpRequest, Coroutine> httpRequests;

		public static void Init(IHttpService service)
		{
			if (instance) return;

			instance = new GameObject(typeof(Http).Name).AddComponent<Http>();
			instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
			instance.superHeaders = new Dictionary<string, string>();
			instance.httpRequests = new Dictionary<IHttpRequest, Coroutine>();
			instance.service = service;
			DontDestroyOnLoad(instance.gameObject);
		}

		#region Super Headers

		/// <summary>
		/// Super headers are key value pairs that will be added to every subsequent HttpRequest.
		/// </summary>
		/// <returns>A dictionary of super-headers.</returns>
		public static Dictionary<string, string> GetSuperHeaders()
		{
			return new Dictionary<string, string>(Instance.superHeaders);
		}

		/// <summary>
		/// Sets a header to the SuperHeaders key value pair, if the header key already exists, the value will be replaced.
		/// </summary>
		/// <param name="key">The header key to be set.</param>
		/// <param name="value">The header value to be assigned.</param>
		public static void SetSuperHeader(string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key cannot be null or empty.");
			}

			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Value cannot be null or empty, if you are intending to remove the value, use the RemoveSuperHeader() method.");
			}

			Instance.superHeaders[key] = value;
		}

		/// <summary>
		/// Removes a header from the SuperHeaders list.
		/// </summary>
		/// <param name="key">The header key to be removed.</param>
		/// <returns>If the removal of the element was successful</returns>
		public static bool RemoveSuperHeader(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key cannot be null or empty.");
			}

			return Instance.superHeaders.Remove(key);
		}

		#endregion

		#region Static Requests

		/// <see cref="Duck.Http.Service.IHttpService.Get"/>
		public static IHttpRequest Get(string uri)
		{
			return Instance.service.Get(uri);
		}

		/// <see cref="Duck.Http.Service.IHttpService.GetTexture"/>
		public static IHttpRequest GetTexture(string uri)
		{
			return Instance.service.GetTexture(uri);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Post(string, string)"/>
		public static IHttpRequest Post(string uri, string postData)
		{
			return Instance.service.Post(uri, postData);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Post(string, WWWForm)"/>
		public static IHttpRequest Post(string uri, WWWForm formData)
		{
			return Instance.service.Post(uri, formData);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Post(string, Dictionary&lt;string, string&gt;)"/>
		public static IHttpRequest Post(string uri, Dictionary<string, string> formData)
		{
			return Instance.service.Post(uri, formData);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Post(string, List&lt;IMultipartFormSection&gt;)"/>
		public static IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return Instance.service.Post(uri, multipartForm);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Post(string, byte[], string)"/>
		public static IHttpRequest Post(string uri, byte[] bytes, string contentType)
		{
			return Instance.service.Post(uri, bytes, contentType);
		}

		/// <see cref="Duck.Http.Service.IHttpService.PostJson"/>
		public static IHttpRequest PostJson(string uri, string json)
		{
			return Instance.service.PostJson(uri, json);
		}

		/// <see cref="Duck.Http.Service.IHttpService.PostJson{T}(string, T)"/>
		public static IHttpRequest PostJson<T>(string uri, T payload) where T : class
		{
			return Instance.service.PostJson(uri, payload);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Put(string, byte[])"/>
		public static IHttpRequest Put(string uri, byte[] bodyData)
		{
			return Instance.service.Put(uri, bodyData);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Put(string, string)"/>
		public static IHttpRequest Put(string uri, string bodyData)
		{
			return Instance.service.Put(uri, bodyData);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Delete"/>
		public static IHttpRequest Delete(string uri)
		{
			return Instance.service.Delete(uri);
		}

		/// <see cref="Duck.Http.Service.IHttpService.Head"/>
		public static IHttpRequest Head(string uri)
		{
			return Instance.service.Head(uri);
		}

		#endregion

		internal void Send(IHttpRequest request, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
		{
			var enumerator = SendCoroutine(request, onSuccess, onError, onNetworkError);
			var coroutine = StartCoroutine(enumerator);
			httpRequests.Add(request, coroutine);
		}

		private IEnumerator SendCoroutine(IHttpRequest request, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null)
		{
			yield return service.Send(request, onSuccess, onError, onNetworkError);
			Instance.httpRequests.Remove(request);
		}

		internal void Abort(IHttpRequest request)
		{
			Instance.service.Abort(request);

			if (httpRequests.ContainsKey(request))
			{
				StopCoroutine(httpRequests[request]);
			}

			Instance.httpRequests.Remove(request);
		}

		private void Update()
		{
			foreach (var httpRequest in httpRequests.Keys)
			{
				(httpRequest as IUpdateProgress)?.UpdateProgress();
			}
		}
	}
}
