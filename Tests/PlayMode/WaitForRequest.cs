using UnityEngine;

namespace Duck.Http.Tests.PlayMode
{
	public class WaitForRequest : CustomYieldInstruction
	{
		public override bool keepWaiting
		{
			get
			{
				return !request.IsDone;
			}
		}

		private readonly HttpRequest request;
		
		public WaitForRequest(HttpRequest request)
		{
			this.request = request;
		}
	}
}
