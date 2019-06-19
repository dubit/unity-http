using System.Collections.Generic;
using NUnit.Framework;

namespace Duck.Http.Tests.EditMode
{
	[TestFixture]
	public class UtilsTests
	{
		[Test]
		public void Expect_Construct_Uri_From_Parameters()
		{
			const string URI = "https://subdomain.domain.extension/index.html";
			const string EXPECTED_CONSTRUCTED_URI = URI + "?page=2&start=25&count=5";
			var parameters = new Dictionary<string, string>
			{
				{"page", "2"},
				{"start", "25"},
				{"count", "5"}
			};
			var constructUriWithParameters = HttpUtils.ConstructUriWithParameters(URI, parameters);
			Assert.AreEqual(EXPECTED_CONSTRUCTED_URI, constructUriWithParameters);
		}
	}
}
