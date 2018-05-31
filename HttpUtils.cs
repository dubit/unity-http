using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

namespace DUCK.Http
{
	/// <summary>
	/// A utility class for http, deals with response codes and error messages.
	/// <see cref="https://developer.mozilla.org/en-US/docs/Web/HTTP/Status"/>
	/// </summary>
	public static class HttpUtils
	{
		private static readonly Dictionary<ResponseType, string> responseTypeMessages = new Dictionary<ResponseType, string>
		{
			{ResponseType.Unknown, "Cannot find server - {1}"},
			{ResponseType.Success, "Request successful with code: {0} - {1}"},
			{ResponseType.Error, "The client encounted an error with code: {0} - {1}"},
			{ResponseType.ServerError, "The server encounted an error and responded with code: {0} - {1}"}
		};

		private static readonly Dictionary<long, string> customResponseTypeMessages = new Dictionary<long, string>();

		/// <summary>
		/// Format and append parameters to a uri
		/// </summary>
		/// <param name="uri">The uri to append the properties to.</param>
		/// <param name="parameters">A dictionary of parameters to append to the uri.</param>
		/// <returns>The uri with the appended parameters.</returns>
		public static string FormatUrlParameters(string uri, Dictionary<string, string> parameters)
		{
			if (parameters == null || parameters.Count == 0)
			{
				return uri;
			}

			var stringBuilder = new StringBuilder(uri);

			for (var i = 0; i < parameters.Count; i++)
			{
				var element = parameters.ElementAt(i);
				stringBuilder.Append(i == 0 ? "?" : "&");
				stringBuilder.Append(element.Key);
				stringBuilder.Append("=");
				stringBuilder.Append(element.Value);
			}

			return stringBuilder.ToString();
		}

		/// <summary>
		/// Sets a custom response message
		/// Setting a responseCode that is within the utilities defailt range will override the default message.
		/// </summary>
		/// <param name="responseCode">The response code</param>
		/// <param name="responseMessage">The message regarding the response code</param>
		public static void SetCustomResponseMessage(long responseCode, string responseMessage)
		{
			customResponseTypeMessages[responseCode] = responseMessage;
		}

		/// <summary>
		/// Removed a custom response code message.
		/// </summary>
		/// <param name="responseCode">The response code to remove</param>
		public static bool RemoveCustomResponseMessage(long responseCode)
		{
			return customResponseTypeMessages.Remove(responseCode);
		}

		public static string GetResponseTypeMessage(HttpResponse response)
		{
			return GetResponseTypeMessage(response.ResponseCode, response.Url);
		}

		public static string GetResponseTypeMessage(long responseCode, string url)
		{
			var responseType = GetResponseType(responseCode);
			if (customResponseTypeMessages.ContainsKey(responseCode))
			{
				return customResponseTypeMessages[responseCode];
			}
			if (!responseTypeMessages.ContainsKey(responseType))
			{
				return string.Format(responseTypeMessages[ResponseType.Unknown], responseCode, url);
			}
			return string.Format(responseTypeMessages[responseType], responseCode, url);
		}

		public static ResponseType GetResponseType(long responseCode)
		{
			if (CheckIsCustomResponseCode(responseCode)) return ResponseType.Custom;
			if (CheckIsSuccessful(responseCode)) return ResponseType.Success;
			if (CheckIsError(responseCode)) return ResponseType.Error;
			if (CheckIsServerError(responseCode)) return ResponseType.ServerError;

			return ResponseType.Unknown;
		}

		private static bool CheckIsCustomResponseCode(long responseCode)
		{
			return customResponseTypeMessages.Any(customResponseTypeMessage => responseCode == customResponseTypeMessage.Key);
		}

		private static bool CheckIsSuccessful(long responseCode)
		{
			return responseCode >= 200 && responseCode <= 206;
		}

		private static bool CheckIsError(long responseCode)
		{
			return responseCode >= 400 && responseCode <= 431;
		}

		private static bool CheckIsServerError(long responseCode)
		{
			return responseCode >= 500 && responseCode <= 511;
		}
	}
}