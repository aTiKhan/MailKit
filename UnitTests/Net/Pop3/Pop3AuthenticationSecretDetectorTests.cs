﻿//
// Pop3AuthenticationSecretDetectorTests.cs
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

using System.Text;

using MailKit;
using MailKit.Net.Pop3;

namespace UnitTests.Net.Pop3 {
	[TestFixture]
	public class Pop3AuthenticationSecretDetectorTests
	{
		[Test]
		public void TestEmptyCommand ()
		{
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Array.Empty<byte> ();

			detector.IsAuthenticating = true;

			var secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (0), "# of secrets");
		}

		[Test]
		public void TestNonAuthCommand ()
		{
			const string command = "UIDL 1\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);

			detector.IsAuthenticating = true;

			var secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (0), "# of secrets");
		}

		[Test]
		public void TestNotIsAuthenticating ()
		{
			const string command = "AUTH PLAIN AHVzZXJuYW1lAHBhc3N3b3Jk\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);

			var secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (0), "# of secrets");
		}

		[Test]
		public void TestApopCommand ()
		{
			const string command = "APOP username AHVzZXJuYW1lAHBhc3N3b3Jk\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);

			detector.IsAuthenticating = true;

			var secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (2), "# of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (5), "StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (8), "Length");
			Assert.That (secrets[1].StartIndex, Is.EqualTo (14), "StartIndex");
			Assert.That (secrets[1].Length, Is.EqualTo (24), "Length");
		}

		[Test]
		public void TestApopCommandBitByBit ()
		{
			const string command = "APOP username AHVzZXJuYW1lAHBhc3N3b3Jk\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);
			IList<AuthenticationSecret> secrets;
			int index = 0;

			detector.IsAuthenticating = true;

			while (index < command.Length) {
				secrets = detector.DetectSecrets (buffer, index, 1);
				if (index >= 5 && index != 13 && index < command.Length - 2) {
					Assert.That (secrets.Count, Is.EqualTo (1), $"# of secrets @ index {index}");
					Assert.That (secrets[0].StartIndex, Is.EqualTo (index), "StartIndex");
					Assert.That (secrets[0].Length, Is.EqualTo (1), "Length");
				} else {
					Assert.That (secrets.Count, Is.EqualTo (0), $"# of secrets @ index {index}");
				}
				index++;
			}
		}

		[Test]
		public void TestUserPassCommand ()
		{
			var detector = new Pop3AuthenticationSecretDetector ();
			IList<AuthenticationSecret> secrets;
			byte[] buffer;

			detector.IsAuthenticating = true;

			buffer = Encoding.ASCII.GetBytes ("USER user\r\n");
			secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (1), "USER # of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (5), "USER StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (4), "USER Length");

			buffer = Encoding.ASCII.GetBytes ("PASS p@$$w0rd\r\n");
			secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (1), "PASS # of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (5), "PASS StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (8), "PASS Length");
		}

		[Test]
		public void TestUserPassCommandBitByBit ()
		{
			const string command = "USER user\r\nPASS p@$$w0rd\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);
			IList<AuthenticationSecret> secrets;
			int index = 0;

			detector.IsAuthenticating = true;

			while (index < command.Length) {
				secrets = detector.DetectSecrets (buffer, index, 1);
				if ((index >= 5 && index < 9) || (index >= 16 && index < 24)) {
					Assert.That (secrets.Count, Is.EqualTo (1), $"# of secrets @ index {index}");
					Assert.That (secrets[0].StartIndex, Is.EqualTo (index), "StartIndex");
					Assert.That (secrets[0].Length, Is.EqualTo (1), "Length");
				} else {
					Assert.That (secrets.Count, Is.EqualTo (0), $"# of secrets @ index {index}");
				}
				index++;
			}
		}

		[Test]
		public void TestSaslIRAuthCommand ()
		{
			const string command = "AUTH PLAIN AHVzZXJuYW1lAHBhc3N3b3Jk\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);

			detector.IsAuthenticating = true;

			var secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (1), "# of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (11), "StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (24), "Length");
		}

		[Test]
		public void TestSaslIRAuthCommandBitByBit ()
		{
			const string command = "AUTH PLAIN AHVzZXJuYW1lAHBhc3N3b3Jk\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);
			int secretIndex = "AUTH PLAIN ".Length;
			IList<AuthenticationSecret> secrets;
			int index = 0;

			detector.IsAuthenticating = true;

			while (index < command.Length) {
				secrets = detector.DetectSecrets (buffer, index, 1);
				if (index >= secretIndex && command[index] != '\r' && command[index] != '\n') {
					Assert.That (secrets.Count, Is.EqualTo (1), $"# of secrets @ index {index}");
					Assert.That (secrets[0].StartIndex, Is.EqualTo (index), "StartIndex");
					Assert.That (secrets[0].Length, Is.EqualTo (1), "Length");
				} else {
					Assert.That (secrets.Count, Is.EqualTo (0), $"# of secrets @ index {index}");
				}
				index++;
			}
		}

		[Test]
		public void TestMultiLineSaslAuthCommand ()
		{
			var detector = new Pop3AuthenticationSecretDetector ();
			IList<AuthenticationSecret> secrets;
			byte[] buffer;

			detector.IsAuthenticating = true;

			buffer = Encoding.ASCII.GetBytes ("AUTH LOGIN\r\n");
			secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (0), "initial # of secrets");

			buffer = Encoding.ASCII.GetBytes ("dXNlcm5hbWU=\r\n");
			secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (1), "# of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (0), "StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (12), "Length");

			buffer = Encoding.ASCII.GetBytes ("cGFzc3dvcmQ=\r\n");
			secrets = detector.DetectSecrets (buffer, 0, buffer.Length);
			Assert.That (secrets.Count, Is.EqualTo (1), "# of secrets");
			Assert.That (secrets[0].StartIndex, Is.EqualTo (0), "StartIndex");
			Assert.That (secrets[0].Length, Is.EqualTo (12), "Length");
		}

		[Test]
		public void TestMultiLineSaslAuthCommandBitByBit ()
		{
			const string command = "AUTH LOGIN\r\ndXNlcm5hbWU=\r\ncGFzc3dvcmQ=\r\n";
			var detector = new Pop3AuthenticationSecretDetector ();
			var buffer = Encoding.ASCII.GetBytes (command);
			int secretIndex = "AUTH LOGIN\r\n".Length;
			IList<AuthenticationSecret> secrets;
			int index = 0;

			detector.IsAuthenticating = true;

			while (index < command.Length) {
				secrets = detector.DetectSecrets (buffer, index, 1);
				if (index >= secretIndex && command[index] != '\r' && command[index] != '\n') {
					Assert.That (secrets.Count, Is.EqualTo (1), $"# of secrets @ index {index}");
					Assert.That (secrets[0].StartIndex, Is.EqualTo (index), "StartIndex");
					Assert.That (secrets[0].Length, Is.EqualTo (1), "Length");
				} else {
					Assert.That (secrets.Count, Is.EqualTo (0), $"# of secrets @ index {index}");
				}
				index++;
			}
		}
	}
}
