﻿//
// SslHandshakeExceptionTests.cs
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
using System.Net.Sockets;
using System.Net.Security;
using System.Security.Authentication;
using System.Security.Cryptography.X509Certificates;
using System.Runtime.Serialization.Formatters.Binary;

using MailKit;
using MailKit.Security;

namespace UnitTests.Security {
	[TestFixture]
	public class SslHandshakeExceptionTests
	{
		const string HelpLink = "https://github.com/jstedfast/MailKit/blob/master/FAQ.md#ssl-handshake-exception";

#if NET6_0

		[Test]
		public void TestSerialization ()
		{
			var expected = new SslHandshakeException ("Bad boys, bad boys. Whatcha gonna do?", new IOException ("I/O Error."));
			SslCertificateValidationInfo info = null;

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = new SslHandshakeException ("Bad boys, bad boys. Whatcha gonna do?");

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = new SslHandshakeException ();

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = SslHandshakeException.Create (ref info, new AggregateException ("Aggregate errors.", new IOException (), new IOException ()), false, "IMAP", "localhost", 993, 993, 143);

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = SslHandshakeException.Create (ref info, new AggregateException ("Aggregate errors.", new IOException (), new IOException ()), true, "IMAP", "localhost", 143, 993, 143);

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = SslHandshakeException.Create (ref info, new AggregateException ("Aggregate errors.", new IOException ()), false, "IMAP", "localhost", 993, 993, 143);

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}

			expected = SslHandshakeException.Create (ref info, new AggregateException ("Aggregate errors.", new IOException ()), true, "IMAP", "localhost", 143, 993, 143);

			Assert.That (expected.HelpLink, Is.EqualTo (HelpLink), "Unexpected HelpLink.");

			using (var stream = new MemoryStream ()) {
				var formatter = new BinaryFormatter ();
				formatter.Serialize (stream, expected);
				stream.Position = 0;

				var ex = (SslHandshakeException) formatter.Deserialize (stream);
				Assert.That (ex.Message, Is.EqualTo (expected.Message), "Unexpected Message.");
				Assert.That (ex.HelpLink, Is.EqualTo (expected.HelpLink), "Unexpected HelpLink.");
			}
		}

#endif // NET6_0

		class FakeClient : MailService
		{
			SslCertificateValidationInfo sslValidationInfo;
			int timeout = 2 * 60 * 1000;
			string hostName;

			public FakeClient (IProtocolLogger logger) : base (logger)
			{
			}

			public override object SyncRoot => throw new NotImplementedException ();

			public override HashSet<string> AuthenticationMechanisms => throw new NotImplementedException ();

			public override bool IsConnected => throw new NotImplementedException ();

			public override bool IsSecure => throw new NotImplementedException ();

			public override bool IsEncrypted => throw new NotImplementedException ();

			public override bool IsSigned => throw new NotImplementedException ();

			public override SslProtocols SslProtocol => throw new NotImplementedException ();

			public override CipherAlgorithmType? SslCipherAlgorithm => throw new NotImplementedException ();

			public override int? SslCipherStrength => throw new NotImplementedException ();

#if NET5_0_OR_GREATER
			public override TlsCipherSuite? SslCipherSuite => throw new NotImplementedException ();
#endif

			public override HashAlgorithmType? SslHashAlgorithm => throw new NotImplementedException ();

			public override int? SslHashStrength => throw new NotImplementedException ();

			public override ExchangeAlgorithmType? SslKeyExchangeAlgorithm => throw new NotImplementedException ();

			public override int? SslKeyExchangeStrength => throw new NotImplementedException ();

			public override bool IsAuthenticated => throw new NotImplementedException ();

			public override int Timeout {
				get { return timeout; }
				set { timeout = value; }
			}

			protected override string Protocol => throw new NotImplementedException ();

			public override void Authenticate (Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override void Authenticate (SaslMechanism mechanism, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task AuthenticateAsync (Encoding encoding, ICredentials credentials, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task AuthenticateAsync (SaslMechanism mechanism, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			bool ValidateRemoteCertificate (object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
			{
				bool valid;

				sslValidationInfo?.Dispose ();
				sslValidationInfo = null;

				if (ServerCertificateValidationCallback != null) {
					valid = ServerCertificateValidationCallback (hostName, certificate, chain, sslPolicyErrors);
#if !NETSTANDARD1_3 && !NETSTANDARD1_6
				} else if (ServicePointManager.ServerCertificateValidationCallback != null) {
					valid = ServicePointManager.ServerCertificateValidationCallback (hostName, certificate, chain, sslPolicyErrors);
#endif
				} else {
					valid = DefaultServerCertificateValidationCallback (hostName, certificate, chain, sslPolicyErrors);
				}

				if (!valid) {
					// Note: The SslHandshakeException.Create() method will nullify this once it's done using it.
					sslValidationInfo = new SslCertificateValidationInfo (sender, certificate, chain, sslPolicyErrors);
				}

				return valid;
			}

			async Task ConnectAsync (string host, int port, SecureSocketOptions options, bool doAsync, CancellationToken cancellationToken)
			{
				using (var stream = await ConnectNetwork (host, port, doAsync, cancellationToken).ConfigureAwait (false)) {
					hostName = host;

					var ssl = new SslStream (stream, false, ValidateRemoteCertificate);

					try {
						if (doAsync) {
							await ssl.AuthenticateAsClientAsync (host, ClientCertificates, SslProtocols, CheckCertificateRevocation).ConfigureAwait (false);
						} else {
							ssl.AuthenticateAsClient (host, ClientCertificates, SslProtocols, CheckCertificateRevocation);
						}
					} catch (Exception ex) {
						ssl.Dispose ();

						throw SslHandshakeException.Create (ref sslValidationInfo, ex, false, "HTTP", host, port, 443, 80);
					}
				}
			}

			public override void Connect (string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				ConnectAsync (host, port, options, false, cancellationToken).GetAwaiter ().GetResult ();
			}

			public override void Connect (Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override void Connect (Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task ConnectAsync (string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				return ConnectAsync (host, port, options, false, cancellationToken);
			}

			public override Task ConnectAsync (Socket socket, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task ConnectAsync (Stream stream, string host, int port = 0, SecureSocketOptions options = SecureSocketOptions.Auto, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override void Disconnect (bool quit, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task DisconnectAsync (bool quit, CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override void NoOp (CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}

			public override Task NoOpAsync (CancellationToken cancellationToken = default (CancellationToken))
			{
				throw new NotImplementedException ();
			}
		}

		public static void AssertServerCertificate (X509Certificate2 certificate)
		{
			Assert.That (certificate.GetNameInfo (X509NameType.SimpleName, false), Is.EqualTo ("*.badssl.com"), "CommonName");
			Assert.That (certificate.Issuer, Is.EqualTo ("CN=BadSSL Untrusted Root Certificate Authority, O=BadSSL, L=San Francisco, S=California, C=US"), "Issuer");
			//Assert.That (certificate.SerialNumber, Is.EqualTo ("008040A36688A3B1F2"), "SerialNumber");
			//Assert.That (certificate.Thumbprint, Is.EqualTo ("209BADBBC9E63BBFFC301B3E30C5B51216FCE81D"), "Thumbprint");
		}

		public static void AssertRootCertificate (X509Certificate2 certificate)
		{
			Assert.That (certificate.GetNameInfo (X509NameType.SimpleName, false), Is.EqualTo ("BadSSL Untrusted Root Certificate Authority"), "CommonName");
			Assert.That (certificate.Issuer, Is.EqualTo ("CN=BadSSL Untrusted Root Certificate Authority, O=BadSSL, L=San Francisco, S=California, C=US"), "Issuer");
			Assert.That (certificate.SerialNumber, Is.EqualTo ("0097A0FCFAD7E528FD"), "SerialNumber");
			Assert.That (certificate.Thumbprint, Is.EqualTo ("7890C8934D5869B25D2F8D0D646F9A5D7385BA85"), "Thumbprint");
		}

		[Test]
		public void TestSslCertificateValidationFailure ()
		{
			Assert.Throws<ArgumentNullException> (() => new FakeClient (null));

			using (var client = new FakeClient (new NullProtocolLogger ())) {
				try {
					client.Connect ("untrusted-root.badssl.com", 443, SecureSocketOptions.SslOnConnect);
					Assert.Fail ("SSL handshake should have failed with untrusted-root.badssl.com.");
				} catch (SslHandshakeException ex) {
					Assert.That (ex.ServerCertificate, Is.Not.Null, "ServerCertificate");
					AssertServerCertificate ((X509Certificate2) ex.ServerCertificate);

					// Note: This is null on Mono because Mono provides an empty chain.
					if (ex.RootCertificateAuthority is X509Certificate2 root)
						AssertRootCertificate (root);
				} catch (Exception ex) {
					Assert.Ignore ($"SSL handshake failure inconclusive: {ex}");
				}
			}
		}

		[Test]
		public async Task TestSslCertificateValidationFailureAsync ()
		{
			Assert.Throws<ArgumentNullException> (() => new FakeClient (null));

			using (var client = new FakeClient (new NullProtocolLogger ())) {
				try {
					await client.ConnectAsync ("untrusted-root.badssl.com", 443, SecureSocketOptions.SslOnConnect);
					Assert.Fail ("SSL handshake should have failed with untrusted-root.badssl.com.");
				} catch (SslHandshakeException ex) {
					Assert.That (ex.ServerCertificate, Is.Not.Null, "ServerCertificate");
					AssertServerCertificate ((X509Certificate2) ex.ServerCertificate);

					// Note: This is null on Mono because Mono provides an empty chain.
					if (ex.RootCertificateAuthority is X509Certificate2 root)
						AssertRootCertificate (root);
				} catch (Exception ex) {
					Assert.Ignore ($"SSL handshake failure inconclusive: {ex}");
				}
			}
		}

		[Test]
		public void TestSslConnectOnPlainTextPortFailure ()
		{
			Assert.Throws<ArgumentNullException> (() => new FakeClient (null));

			using (var client = new FakeClient (new NullProtocolLogger ())) {
				try {
					client.Connect ("www.google.com", 80, SecureSocketOptions.SslOnConnect);
					Assert.Fail ("SSL handshake should have failed with www.google.com:80.");
				} catch (SslHandshakeException ex) {
					Assert.That (ex.ServerCertificate, Is.Null, "ServerCertificate");
					Assert.That (ex.RootCertificateAuthority, Is.Null, "RootCertificateAuthority");
				} catch (Exception ex) {
					Assert.Ignore ($"SSL handshake failure inconclusive: {ex}");
				}
			}
		}

		[Test]
		public async Task TestSslConnectOnPlainTextPortFailureAsync ()
		{
			Assert.Throws<ArgumentNullException> (() => new FakeClient (null));

			using (var client = new FakeClient (new NullProtocolLogger ())) {
				try {
					await client.ConnectAsync ("www.google.com", 80, SecureSocketOptions.SslOnConnect);
					Assert.Fail ("SSL handshake should have failed with www.google.com:80.");
				} catch (SslHandshakeException ex) {
					Assert.That (ex.ServerCertificate, Is.Null, "ServerCertificate");
					Assert.That (ex.RootCertificateAuthority, Is.Null, "RootCertificateAuthority");
				} catch (Exception ex) {
					Assert.Ignore ($"SSL handshake failure inconclusive: {ex}");
				}
			}
		}
	}
}
