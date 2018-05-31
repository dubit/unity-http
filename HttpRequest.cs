using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest
	{
		public UnityWebRequest UnityWebRequest { get; private set; }

		private readonly Dictionary<string, string> headers;

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

			Http.Instance.Send(this, onSuccess, onError);
		}
	}
}