using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpRequest
	{
		internal UnityWebRequest UnityWebRequest { get { return unityWebRequest; } }

		private readonly UnityWebRequest unityWebRequest;
		private Dictionary<string, string> headers;

		private event Action<int> onProgress;
		private event Action<HttpResponse> onSuccess;
		private event Action<HttpResponse> onError;
		private event Action<HttpResponse> onNetworkError;

		public HttpRequest(UnityWebRequest unityWebRequest)
		{
			this.unityWebRequest = unityWebRequest;
			headers = new Dictionary<string, string>();
		}

		public HttpRequest IncludeSuperHeaders()
		{
			headers = headers.Concat(Http.Instance.GetSuperHeaders()).ToDictionary(x => x.Key, x => x.Value);
			return this;
		}

		public HttpRequest SetHeader(string key, string value)
		{
			headers[key] = value;
			return this;
		}

		public HttpRequest SetHeaders(IEnumerable<KeyValuePair<string, string>> headers)
		{
			foreach (var kvp in headers)
			{
				SetHeader(kvp.Key, kvp.Value);
			}

			return this;
		}

		public HttpRequest OnProgress(Action<int> onProgress)
		{
			this.onProgress += onProgress;
			return this;
		}

		public HttpRequest OnSuccess(Action<HttpResponse> onSuccess)
		{
			this.onSuccess += onSuccess;
			return this;
		}

		public HttpRequest OnError(Action<HttpResponse> onError)
		{
			this.onError += onError;
			return this;
		}

		public HttpRequest OnNetworkError(Action<HttpResponse> onNetworkError)
		{
			this.onNetworkError += onNetworkError;
			return this;
		}

		public bool RemoveHeader(string key)
		{
			return headers.Remove(key);
		}

		public HttpRequest Send()
		{
			foreach (var header in headers)
			{
				unityWebRequest.SetRequestHeader(header.Key, header.Value);
			}

			Http.Instance.Send(this, onSuccess, onError);
			return this;
		}

		internal void UpdateProgress(int progress)
		{
			if (onProgress != null)
			{
				onProgress.Invoke(progress);
			}
		}
	}
}
