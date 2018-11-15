using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace DUCK.Http
{
	public class HttpResponse
	{
		public string Url { get; private set; }
		public bool IsSuccessful { get; private set; }
		public bool IsHttpError { get; private set; }
		public bool IsNetworkError { get; private set; }
		public long StatusCode { get; private set; }
		public ResponseType ResponseType { get; private set; }
		public byte[] Bytes { get; private set; }
		public string Text { get; private set; }
		public string Error { get; private set; }
		public Texture Texture { get; private set; }
		public Dictionary<string, string> ResponseHeaders { get; private set; }

		public HttpResponse(UnityWebRequest unityWebRequest)
		{
			Url = unityWebRequest.url;
			Bytes = unityWebRequest.downloadHandler.data;
			Text = unityWebRequest.downloadHandler.text;
			IsSuccessful = !unityWebRequest.isHttpError && !unityWebRequest.isNetworkError;
			IsHttpError = unityWebRequest.isHttpError;
			IsNetworkError = unityWebRequest.isNetworkError;
			Error = unityWebRequest.error;
			StatusCode = unityWebRequest.responseCode;
			ResponseType = HttpUtils.GetResponseType(StatusCode);
			ResponseHeaders = unityWebRequest.GetResponseHeaders();

			var downloadHandlerTexture = unityWebRequest.downloadHandler as DownloadHandlerTexture;
			if (downloadHandlerTexture != null)
			{
				Texture = downloadHandlerTexture.texture;
			}
		}
	}
}
