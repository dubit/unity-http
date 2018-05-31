using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	// How to use
	// First use one of Http static helper methods to create a HttpRequest. for example we will do a get request.
	// Http.Get("uri");
	// This will return a HttpRequest setup for Http Verb GET.
	// The next part is to actually send the HttpRequest, we do this by simply calling Send() on the HttpRequest.
	// Http.Get("uri").Send();
	// The Send method signature has the options for onSuccess and onError callbacks.
	// HttpRequest.Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null);
	// Both callbacks include the HttpResponse object.
	// You can get the Body of the response in 4 different forms: Text, Bytes, Texture or as an object parsed from Json.
	// Heres what it looks like parsed to an object:
	// Http.Get("uri").Send(response => { response.ParseBodyAs<User>(); });
	//
	// Alternatively you can use Http for sending UnityWebRequest with callbacks. For example:
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), (UnityWebRequest request) => { });
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onError: () => { });
	// Http.Instance.Send(UnityWebRequest.Post("uri", "payload"), onNetworkError: () => { });
	public sealed class Http : MonoBehaviour
	{
		private static Http instance;

		public static Http Instance
		{
			get
			{
				if (instance == null)
				{
					instance = new GameObject("Http").AddComponent<Http>();
				}

				return instance;
			}
		}

		private Dictionary<string, string> superHeaders;

		private void Awake()
		{
			instance = this;
			instance.hideFlags = HideFlags.HideInHierarchy;

			superHeaders = new Dictionary<string, string>();
		}

		/// <summary>
		/// Super headers are key value pairs that will be added to every subsequent HttpRequest.
		/// </summary>
		/// <returns>A dictionary of super-headers.</returns>
		public Dictionary<string, string> GetSuperHeaders()
		{
			return new Dictionary<string, string>(superHeaders);
		}

		/// <summary>
		/// Sets a header to the SuperHeaders key value pair, if the header key already exists, the value will be replaced.
		/// </summary>
		/// <param name="key">The header key to be set.</param>
		/// <param name="value">The header value to be assigned.</param>
		public void SetSuperHeader(string key, string value)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key cannot be null or empty.");
			}
			if (string.IsNullOrEmpty(value))
			{
				throw new ArgumentException("Value cannot be null or empty, if you are intending to remove the value, use the RemoveSuperHeader() method.");
			}
			superHeaders[key] = value;
		}

		/// <summary>
		/// Removes a header from the SuperHeaders list.
		/// </summary>
		/// <param name="key">The header key to be removed.</param>
		/// <returns>If the removal of the element was successful</returns>
		public bool RemoveSuperHeader(string key)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentException("Key cannot be null or empty.");
			}
			return superHeaders.Remove(key);
		}

		#region Creation Helpers

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

		public static HttpRequest Post(string uri, List<IMultipartFormSection> multipartForm)
		{
			return new HttpRequest(UnityWebRequest.Post(uri, multipartForm));
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

		#endregion

		#region None Static Send Methods

		/// <summary>
		/// Transmit a HTTP request to the remote server at the target URL and process the server’s response.
		/// </summary>
		/// <param name="request">The request to transmit</param>
		/// <param name="onSuccess">The callback for on success response from the server</param>
		/// <param name="onError">The callback for on error with the request or response.</param>
		public void Send(HttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(request, onSuccess, onError));
		}

		/// <summary>
		/// Transmit a HTTP request to the remote server at the target URL and process the server’s response.
		/// </summary>
		/// <param name="unityWebRequest">The request to transmit</param>
		/// <param name="onSuccess">The callback for on success response from the server</param>
		/// <param name="onError">THe callback for on error with the request or response.</param>
		/// <param name="onNetworkError">The callback for on network error with the request.</param>
		public void Send(UnityWebRequest unityWebRequest, Action<UnityWebRequest> onSuccess = null,
			Action<UnityWebRequest> onError = null, Action<UnityWebRequest> onNetworkError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		/// <summary>
		/// Transmit a HTTP request to the remote server at the target URL and process the server’s response.
		/// </summary>
		/// <param name="unityWebRequest">The request to transmit</param>
		/// <param name="onSuccess">The callback for on success response from the server</param>
		/// <param name="onError">The callback for on error with the request or response.</param>
		/// <param name="onNetworkError">The callback for on network error with the request.</param>
		public void Send(UnityWebRequest unityWebRequest, Action onSuccess = null,
			Action onError = null, Action onNetworkError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError, onNetworkError));
		}

		/// <summary>
		/// Transmit a HTTP request to the remote server at the target URL and process the server’s response.
		/// </summary>
		/// <param name="unityWebRequest">The request to transmit</param>
		/// <param name="onSuccess">The callback for on success response from the server</param>
		/// <param name="onError">The callback for on error with the request or response.</param>
		public void Send(UnityWebRequest unityWebRequest, Action<HttpResponse> onSuccess = null,
			Action<HttpResponse> onError = null)
		{
			StartCoroutine(SendCoroutine(unityWebRequest, onSuccess, onError));
		}

		#endregion

		#region Send HttpRequest methods

		private static IEnumerator SendCoroutine(HttpRequest request, Action<HttpResponse> onSuccess,
			Action<HttpResponse> onError)
		{
			var unityWebRequest = request.UnityWebRequest;
#if UNITY_2017_2_OR_NEWER
			yield return unityWebRequest.SendWebRequest();
#else
			yield return unityWebRequest.Send();
#endif
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
		}

		#endregion

		#region Send UnityWebRequest methods

		private static IEnumerator SendCoroutine(UnityWebRequest unityWebRequest,
			Action onSuccess = null, Action onError = null, Action onNetworkError = null)
		{
#if UNITY_2017_2_OR_NEWER
			yield return unityWebRequest.SendWebRequest();
#else
			yield return unityWebRequest.Send();
#endif
			if (unityWebRequest.isNetworkError)
			{
				if (onNetworkError != null)
				{
					onNetworkError.Invoke();
				}
			}
			else if (unityWebRequest.isHttpError)
			{
				if (onError != null)
				{
					onError.Invoke();
				}
			}
			else
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke();
				}
			}
		}

		private static IEnumerator SendCoroutine(UnityWebRequest unityWebRequest,
			Action<UnityWebRequest> onSuccess = null,
			Action<UnityWebRequest> onError = null, Action<UnityWebRequest> onNetworkError = null)
		{
#if UNITY_2017_2_OR_NEWER
			yield return unityWebRequest.SendWebRequest();
#else
			yield return unityWebRequest.Send();
#endif
			if (unityWebRequest.isNetworkError)
			{
				if (onNetworkError != null)
				{
					onNetworkError.Invoke(unityWebRequest);
				}
			}
			else if (unityWebRequest.isHttpError)
			{
				if (onError != null)
				{
					onError.Invoke(unityWebRequest);
				}
			}
			else
			{
				if (onSuccess != null)
				{
					onSuccess.Invoke(unityWebRequest);
				}
			}
		}

		private static IEnumerator SendCoroutine(UnityWebRequest request,
			Action<HttpResponse> onSuccess,
			Action<HttpResponse> onError)
		{
#if UNITY_2017_2_OR_NEWER
			yield return request.SendWebRequest();
#else
			yield return request.Send();
#endif
			var response = new HttpResponse(request);

			if (request.isNetworkError || request.isHttpError)
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
		}

		#endregion
	}
}