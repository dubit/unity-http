using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

namespace Duck.Http.Service
{
	public interface IHttpService
	{
		/// <summary>
		/// Creates a HttpRequest configured for HTTP GET.
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>A HttpRequest object configured to retrieve data from uri.</returns>
		IHttpRequest Get(string uri);

		/// <summary>
		/// Creates a HttpRequest configured for HTTP GET.
		/// </summary>
		/// <param name="uri">The URI of the resource to retrieve via HTTP GET.</param>
		/// <returns>A HttpRequest object configured to retrieve data from uri.</returns>
		IHttpRequest GetTexture(string uri);

		/// <summary>
		/// Creates a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="postData">Form body data. Will be URLEncoded via WWWTranscoder.URLEncode prior to transmission.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		IHttpRequest Post(string uri, string postData);

		/// <summary>
		/// Creates a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields or files encapsulated in a WWWForm object, for formatting and transmission to the remote server.</param>
		/// <returns> A HttpRequest configured to send form data to uri via POST. </returns>
		IHttpRequest Post(string uri, WWWForm formData);

		/// <summary>
		/// Creates a HttpRequest configured to send form data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="formData">Form fields in the form of a Key Value Pair, for formatting and transmission to the remote server.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		IHttpRequest Post(string uri, Dictionary<string, string> formData);

		/// <summary>
		/// Creates a HttpRequest configured to send form multipart form to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which form data will be transmitted.</param>
		/// <param name="multipartForm">MultipartForm data for formatting and transmission to the remote server.</param>
		/// <returns>A HttpRequest configured to send form data to uri via POST.</returns>
		IHttpRequest Post(string uri, List<IMultipartFormSection> multipartForm);

		/// <summary>
		/// Creates a HttpRequest configured to send raw bytes to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which bytes will be transmitted.</param>
		/// <param name="bytes">Byte array data.</param>
		/// <param name="contentType">String representing the MIME type of the data (e.g. image/jpeg).</param>
		/// <returns>A HttpRequest configured to send raw bytes to a server via POST.</returns>
		IHttpRequest Post(string uri, byte[] bytes, string contentType);

		/// <summary>
		/// Creates a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="json">Json body data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
		IHttpRequest PostJson(string uri, string json);

		/// <summary>
		/// Creates a HttpRequest configured to send json data to a server via HTTP POST.
		/// </summary>
		/// <param name="uri">The target URI to which json data will be transmitted.</param>
		/// <param name="payload">The object to be parsed to json data.</param>
		/// <returns>A HttpRequest configured to send json data to uri via POST.</returns>
		IHttpRequest PostJson<T>(string uri, T payload) where T : class;

		/// <summary>
		/// Creates a HttpRequest configured to upload raw data to a remote server via HTTP PUT.
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.</param>
		/// <returns>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</returns>
		IHttpRequest Put(string uri, byte[] bodyData);

		/// <summary>
		/// Creates a HttpRequest configured to upload raw data to a remote server via HTTP PUT.
		/// </summary>
		/// <param name="uri">The URI to which the data will be sent.</param>
		/// <param name="bodyData">The data to transmit to the remote server.
		/// The string will be converted to raw bytes via &lt;a href="http:msdn.microsoft.comen-uslibrarysystem.text.encoding.utf8"&gt;System.Text.Encoding.UTF8&lt;a&gt;.</param>
		/// <returns>A HttpRequest configured to transmit bodyData to uri via HTTP PUT.</returns>
		IHttpRequest Put(string uri, string bodyData);

		/// <summary>
		/// Creates a HttpRequest configured for HTTP DELETE.
		/// </summary>
		/// <param name="uri">The URI to which a DELETE request should be sent.</param>
		/// <returns>A HttpRequest configured to send an HTTP DELETE request.</returns>
		IHttpRequest Delete(string uri);

		/// <summary>
		/// Creates a HttpRequest configured to send a HTTP HEAD request.
		/// </summary>
		/// <param name="uri">The URI to which to send a HTTP HEAD request.</param>
		/// <returns>A HttpRequest configured to transmit a HTTP HEAD request.</returns>
		IHttpRequest Head(string uri);

		IEnumerator Send(IHttpRequest request, Action<HttpResponse> onSuccess = null, Action<HttpResponse> onError = null, Action<HttpResponse> onNetworkError = null);

		void Abort(IHttpRequest request);
	}
}
