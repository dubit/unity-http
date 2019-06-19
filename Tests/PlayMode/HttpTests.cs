using System;
using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using Object = UnityEngine.Object;

namespace Duck.Http.Tests.PlayMode
{
	[TestFixture]
	public class HttpTests
	{
		private Http http;
		private WaitForEndOfFrame waitForEndOfFrame;

		[OneTimeSetUp]
		public void Setup()
		{
			http = Http.Instance;
			waitForEndOfFrame = new WaitForEndOfFrame();
		}

		[OneTimeTearDown]
		public void TearDown()
		{
			Object.DestroyImmediate(http.gameObject);
		}

		[Test]
		public void Expect_Singleton_Is_Created()
		{
			Assert.IsNotNull(http);
		}

		[Test]
		public void Expect_Set_Super_Headers()
		{
			const string HEADER_KEY = "HeaderKey";
			const string HEADER_VALUE = "HeaderValue";

			Http.SetSuperHeader(HEADER_KEY, HEADER_VALUE);
			var headers = Http.GetSuperHeaders();

			Assert.IsTrue(headers.ContainsKey(HEADER_KEY));
			Assert.AreEqual(HEADER_VALUE, headers[HEADER_KEY]);
			Assert.AreEqual(1, headers.Count);
		}

		[UnityTest]
		public IEnumerator Expect_OnNetworkError_Event()
		{
			// Domain does not exist.
			const string URI = "http://127.0.0.1:1235";

			var eventTriggered = false;

			Action<HttpResponse> onNetworkError = response => eventTriggered = true;

			var request = Http.Get(URI);
			request.OnNetworkError(onNetworkError);
			request.Send();

			yield return new WaitForRequest(request);
			yield return waitForEndOfFrame;

			Assert.IsTrue(eventTriggered);
		}

		[UnityTest]
		public IEnumerator Expect_OnError_Event()
		{
			// Existing domain but missing endpoint
			const string URI = "http://www.google.com/test/http";

			var eventTriggered = false;

			Action<HttpResponse> onError = response => eventTriggered = true;

			var request = Http.Get(URI);
			request.OnError(onError);
			request.Send();

			yield return new WaitForRequest(request);
			yield return waitForEndOfFrame;

			Assert.IsTrue(eventTriggered);
		}

		[UnityTest]
		public IEnumerator Expect_OnSuccess_Event()
		{
			// Existing domain
			const string URI = "http://www.google.com";

			var eventTriggered = false;

			Action<HttpResponse> onSuccess = response => eventTriggered = true;

			var request = Http.Get(URI);
			request.OnSuccess(onSuccess);
			request.Send();

			yield return new WaitForRequest(request);
			yield return waitForEndOfFrame;

			Assert.IsTrue(eventTriggered);
		}

		[UnityTest]
		public IEnumerator Expect_OnDownloadProgress_Event()
		{
			const string URI = "http://www.google.com";

			var eventTriggered = false;

			Action<float> onDownloadProgress = progress => eventTriggered = true;

			var request = Http.GetTexture(URI);
			request.OnDownloadProgress(onDownloadProgress);
			request.Send();

			yield return new WaitForRequest(request);
			yield return waitForEndOfFrame;

			Assert.IsTrue(eventTriggered);
		}
		
		[UnityTest]
		public IEnumerator Expect_OnUploadProgress_Event()
		{
			const string URI = "http://www.google.com";

			var eventTriggered = false;

			Action<float> onUploadProgress = progress => eventTriggered = true;

			var request = Http.Post(URI, string.Empty);
			request.OnUploadProgress(onUploadProgress);
			request.Send();

			yield return new WaitForRequest(request);
			yield return waitForEndOfFrame;

			Assert.IsTrue(eventTriggered);
		}
	}
}
