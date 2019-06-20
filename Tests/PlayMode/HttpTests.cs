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

		[OneTimeSetUp]
		public void Setup()
		{
			http = Http.Instance;
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
	}
}
