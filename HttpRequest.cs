using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest
	{
		public UnityWebRequest UnityWebRequest { get; private set; }

		private readonly Dictionary<string, string> headers;
		private Coroutine coroutine;

		public HttpRequest(UnityWebRequest unityWebRequest)
		{
			UnityWebRequest = unityWebRequest;

			headers = Http.Instance.GetSuperHeaders();
		}

		public void SetHeader(string key, string value)
		{
			headers[key] = value;
		}

		public void SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
		{
			if (headers == null) return;

			foreach (var kvp in headers)
			{
				SetHeader(kvp.Key, kvp.Value);
			}
		}

		public bool RemoveHeader(string key)
		{
			return headers.Remove(key);
		}

		public void Send(Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null)
		{
			foreach (var header in headers)
			{
				UnityWebRequest.SetRequestHeader(header.Key, header.Value);
			}

			coroutine = Http.Instance.Send(this, onSuccess, onError);
		}

		public void Abort()
		{
			if (UnityWebRequest != null && !UnityWebRequest.isDone)
			{
				UnityWebRequest.Abort();
			}

			if (coroutine != null)
			{
				Http.Instance.Abort(coroutine);
			}
		}
	}
}
