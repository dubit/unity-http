using System;
using System.Collections.Generic;
using UnityEngine.Networking;

namespace Duck.Http
{
	public class HttpRequest
	{
		internal UnityWebRequest UnityWebRequest { get { return unityWebRequest; } }

		private readonly UnityWebRequest unityWebRequest;
		private Dictionary<string, string> headers;

		private event Action<float> onUploadProgress;
		private event Action<float> onDownloadProgress;
		private event Action<HttpResponse> onSuccess;
		private event Action<HttpResponse> onError;
		private event Action<HttpResponse> onNetworkError;

		private float downloadProgress;
		private float uploadProgress;

		public HttpRequest(UnityWebRequest unityWebRequest)
		{
			this.unityWebRequest = unityWebRequest;
			headers = new Dictionary<string, string>(Http.GetSuperHeaders());
		}

		public HttpRequest RemoveSuperHeaders()
		{
			foreach (var kvp in Http.GetSuperHeaders())
			{
				headers.Remove(kvp.Key);
			}

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

		public HttpRequest OnUploadProgress(Action<float> onProgress)
		{
			onUploadProgress += onProgress;
			return this;
		}

		public HttpRequest OnDownloadProgress(Action<float> onProgress)
		{
			onDownloadProgress += onProgress;
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

		public HttpRequest SetTimeout(int duration)
		{
			unityWebRequest.timeout = duration;
			return this;
		}

		public HttpRequest Send()
		{
			foreach (var header in headers)
			{
				unityWebRequest.SetRequestHeader(header.Key, header.Value);
			}

			Http.Instance.Send(this, onSuccess, onError, onNetworkError);
			return this;
		}

		public HttpRequest SetRedirectLimit(int redirectLimit)
		{
			UnityWebRequest.redirectLimit = redirectLimit;
			return this;
		}

		public void Abort()
		{
			Http.Instance.Abort(this);
		}

		internal void UpdateProgress()
		{
			UpdateProgress(ref downloadProgress, unityWebRequest.downloadProgress, onDownloadProgress);
			UpdateProgress(ref uploadProgress, unityWebRequest.uploadProgress, onUploadProgress);
		}

		private void UpdateProgress(ref float currentProgress, float progress, Action<float> onProgress)
		{
			if (currentProgress < progress)
			{
				currentProgress = progress;
				if (onProgress != null)
				{
					onProgress.Invoke(downloadProgress);
				}
			}
		}
	}
}
