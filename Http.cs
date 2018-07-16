using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public sealed class Http : MonoBehaviour
	{
		public static Http Instance
		{
			get
			{
				if (instance != null) return instance;
				instance = new GameObject(typeof(Http).Name).AddComponent<Http>();
				instance.gameObject.hideFlags = HideFlags.HideInHierarchy;
				instance.superHeaders = new Dictionary<string, string>();
				instance.httpRequests = new Dictionary<HttpRequest, Coroutine>();
				DontDestroyOnLoad(instance.gameObject);
				return instance;
			}
		}

		private static Http instance;

		private Dictionary<string, string> superHeaders;
		private Dictionary<HttpRequest, Coroutine> httpRequests;

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

		/// <summary>
		/// Creates a HttpRequest configured for HTTP GET.
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>A HttpRequest object configured to retrieve data from uri.</returns>
		public static HttpRequest Get(string uri)
		{
			return new HttpRequest(UnityWebRequest.Get(uri));
		}

		/// <summary>
		/// Creates a HttpRequest configured for HTTP GET.
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>A HttpRequest object configured to retrieve data from uri.</returns>
		public static HttpRequest GetTexture(string uri)
		{
			return new HttpRequest(UnityWebRequestTexture.GetTexture(uri));
		}

		/// <summary>
		/// Create a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="postData">Form body data. Will be URLEncoded via WWWTranscoder.URLEncode prior to transmission.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		public static HttpRequest Post(string uri, string postData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, postData));
		}

		/// <summary>
		/// Create a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
		/// <returns> A HttpRequest configured to send form data to uri via POST. </returns>
		public static HttpRequest Post(string uri, WWWForm formData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, formData));
		}

		/// <summary>
		/// Create a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields in the form of a Key Value Pair, for formatting and transmission to the remote server.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		public static HttpRequest Post(string uri, Dictionary<string, string> formData)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, formData));
		}

		/// <summary>
		/// Create a HttpRequest configured to send form multipart form to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="multipartForm">MultipartForm data for formatting and transmission to the remote server.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		public static HttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, multipartForm));
		}

		/// <summary>
		/// Create a HttpRequest configured to send raw bytes to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which bytes will be transmitted.</param>
		/// <param name="bytes">Byte array data.</param>
		/// <param name="contentType">String representing the MIME type of the data (e.g. image/jpeg).</param>
		/// <returns>A HttpRequest configured to send raw bytes to a server via POST.</returns>
		public static HttpRequest Post(string uri, byte[] bytes, string contentType)
		{
			var unityWebRequest = new UnityWebRequest(uri, UnityWebRequest.kHttpVerbPOST)
			{
				uploadHandler = new UploadHandlerRaw(bytes)
				{
					contentType = contentType
				},
				downloadHandler = new DownloadHandlerBuffer()
			};
			return new HttpRequest(unityWebRequest);
		}

		/// <summary>
		/// Create a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="json">Json body data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
		public static HttpRequest PostJson(string uri, string json)
		{
			return Post(uri, Encoding.UTF8.GetBytes(json), "application/json");
		}

		/// <summary>
		/// Create a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="payload">The object to be parsed to json data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
		public static HttpRequest PostJson<T>(string uri, T payload) where T : class
		{
			return PostJson(uri, JsonUtility.ToJson(payload));
		}

		/// <summary>
		/// Create a HttpRequest configured to upload raw data to a remote server via HTTP PUT.
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.</param>
		/// <returns>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</returns>
		public static HttpRequest Put(string uri, byte[] bodyData)
		{
			return new HttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		/// <summary>
		/// Create a HttpRequest configured to upload raw data to a remote server via HTTP PUT.
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.
		/// The string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
		/// <returns>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</returns>
		public static HttpRequest Put(string uri, string bodyData)
		{
			return new HttpRequest(UnityWebRequest.Put(uri, bodyData));
		}

		/// <summary>
		/// Creates a HttpRequest configured for HTTP DELETE.
		/// </summary>
		/// <param name="uri">The URI to which a DELETE request should be sent.</param>
		/// <returns>A HttpRequest configured to send an HTTP DELETE request.</returns>
		public static HttpRequest Delete(string uri)
		{
			return new HttpRequest(UnityWebRequest.Delete(uri));
		}

		/// <summary>
		/// Creates a HttpRequest configured to send a HTTP HEAD request.
		/// </summary>
		/// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
		/// <returns>A HttpRequest configured to transmit a HTTP HEAD request.</returns>
		public static HttpRequest Head(string uri)
		{
			return new HttpRequest(UnityWebRequest.Head(uri));
		}

		internal void Send(HttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			var coroutine = StartCoroutine(SendCoroutine(request, onSuccess, onError));
			httpRequests.Add(request, coroutine);
		}

		internal void Abort(HttpRequest request)
		{
			if (request.UnityWebRequest != null && !request.UnityWebRequest.isDone)
			{
				request.UnityWebRequest.Abort();
			}

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
				httpRequest.UpdateProgress();
			}
		}

		private static IEnumerator SendCoroutine(HttpRequest request, Action<HttpResponse> onSuccess,
			Action<HttpResponse> onError)
		{
			var unityWebRequest = request.UnityWebRequest;
			yield return unityWebRequest.SendWebRequest();

			var response = new HttpResponse(unityWebRequest);

			if (unityWebRequest.isNetworkError || unityWebRequest.isHttpError)
			{
				if (onError != null)
				{
					onError.Invoke(response);
				}
			}
			else if (onSuccess != null)
			{
				onSuccess.Invoke(response);
			}

			Instance.httpRequests.Remove(request);
		}
	}
}
