﻿//
// SaslMechanismAnonymousTests.cs
//
// Author: Jeffrey Stedfast <jestedfa@microsoft.com>
//
// Copyright (c) 2013-2023 .NET Foundation and Contributors
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
//

using System.Net;
using System.Text;

using MailKit.Security;

namespace UnitTests.Security {
	[TestFixture]
	public class SaslMechanismAnonymousTests
	{
		[Test]
		public void TestArgumentExceptions ()
		{
			var credentials = new NetworkCredential ("username", "password");
			SaslMechanism sasl;

			sasl = new SaslMechanismAnonymous (credentials);
			Assert.DoesNotThrow (() => sasl.Challenge (null));

			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous (null, credentials));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous (Encoding.UTF8, (NetworkCredential) null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous ((NetworkCredential) null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous (null, "username"));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous (Encoding.UTF8, (string) null));
			Assert.Throws<ArgumentNullException> (() => new SaslMechanismAnonymous ((string) null));
		}

		static void AssertAnonymous (SaslMechanismAnonymous sasl, string prefix)
		{
			const string expected = "dXNlcm5hbWU=";
			string challenge;

			Assert.That (sasl.SupportsChannelBinding, Is.False, $"{prefix}: SupportsChannelBinding");
			Assert.That (sasl.SupportsInitialResponse, Is.True, $"{prefix}: SupportsInitialResponse");

			challenge = sasl.Challenge (string.Empty);

			Assert.That (challenge, Is.EqualTo (expected), $"{prefix}: challenge response does not match the expected string.");
			Assert.That (sasl.IsAuthenticated, Is.True, $"{prefix}: should be authenticated.");
			Assert.That (sasl.NegotiatedChannelBinding, Is.False, $"{prefix}: NegotiatedChannelBinding");
			Assert.That (sasl.NegotiatedSecurityLayer, Is.False, $"{prefix}: NegotiatedSecurityLayer");

			Assert.That (sasl.Challenge (string.Empty), Is.EqualTo (string.Empty), $"{prefix}: challenge while authenticated.");
		}

		[Test]
		public void TestAnonymousAuth ()
		{
			var credentials = new NetworkCredential ("username", "password");
			var sasl = new SaslMechanismAnonymous (credentials);

			AssertAnonymous (sasl, "NetworkCredential");

			sasl = new SaslMechanismAnonymous ("username");

			AssertAnonymous (sasl, "user");
		}
	}
}
