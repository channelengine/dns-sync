using System.IO;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;
using DnsSync.ConsoleApp.Configuration;
using DnsSync.ConsoleApp.TransIp.Auth;
using DnsSync.ConsoleApp.TransIp.Models;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Moq;
using NUnit.Framework;

namespace DnsSync.Tests
{
    [TestFixture]
    public class TransIpApiTests
    {
        /// <summary>
        /// Verify signature against existing test data from Go library
        /// See: https://github.com/transip/gotransip/blob/master/authenticator/sign_test.go
        /// </summary>
        [Test]
        public async Task SignRequest()
        {
            var content = new AuthRequest()
            {
                Login = "test-user",
                Nonce = "98475920834"
            };

            // Match GO implementation for test
            var jsonSerializerOptions = new JsonSerializerOptions()
            {
                IgnoreNullValues = true
            };

            var httpClient = new Mock<HttpClient>();
            var cache = new Mock<IMemoryCache>();
            var config = new Mock<IOptionsMonitor<TransIpApiConfiguration>>();

            var client = new TransIpAuthClient(httpClient.Object, cache.Object, config.Object);

            await using var ms = new MemoryStream();
            await JsonSerializer.SerializeAsync(ms, content, jsonSerializerOptions);
            ms.Position = 0;

            var signature = TransIpAuthClient.GetSignature(PrivateKey, ms);

            Assert.AreEqual(Signature, signature);
        }
        
        private const string PrivateKey = @"-----BEGIN PRIVATE KEY-----
            MIIEvAIBADANBgkqhkiG9w0BAQEFAASCBKYwggSiAgEAAoIBAQCZT5Eh9PmQ3flx
            uFJyVG55A/RuxbYb5qv/1SBcPSZajBogtgEKvw7lcKLhkXLDSCN0pQGABRl6vTgP
            aSi/s3wrKA3n9tpVa0VAQi9QGP7oVQeq3UxJ0L+yEX5HsuqYRw+mFEqxkXcdYxeV
            3xGF8eB01cBOles2H5JUOMeKTyT4VQxNe+gqyG09Ia8aPDocvERBmCAdrZBSLEdH
            fxGxTVzhwFh81qpR9CD8q7Q8nX7Tk0a8s42WozXu8pHQhTMHPdRkxLFxGab0p/AT
            jNzG2nLi5LXS3rCSkQeHJjbPZP0T3m2OehNk40uXthH9BHgOMfyGXbX8BA8EgOeG
            BCp6TNHTAgMBAAECggEANQ/4AJPEiSJ7AqQ0TQPyFIqM4IYnyLJnF64RfDth+fcB
            2A6Gf8yvADSi+4WW/gYK14WA5mldb0DslVDlXKxnrpw3a/Dhkq0FE/+UVpnAKHO9
            qqLbk7TflGc/mNtRHRGDVg0x6RGa853nfOTvMLgN4wJUhB6ZgWsd/26DidhoyFZG
            P3Poz1u4VCsupfvxr2wo/u1vAQzAG6fVIFDTVWYtAp6nRCSg5kORIALpeNEoSMj0
            JWaUoA0LiUjV6JnagMQmQtkb5ScoFMpQOoNmRdJHsJTF62lWwKSarMWHq6eEpdXG
            O+Zl9CcE1/wmnY0+rVMeTJxbWn7gVXoxKG75C5jSQQKBgQDGlYvCiy6Ap/WjtrEg
            Jr0aVpSSf0LkKYnvWvW7/kZJBz5KC6mEG/jW16CnA9ZnMAOTs+HKOQCYd+vPMhvz
            XBNoyNxF8MrPn/lTD+3EH4sV6ZXxnLM4kpJiAEgoRSB49W4nRI6F72nUT+KakUVq
            LQurujv69mxowF+W8QpnluncYQKBgQDFow6mZ6DKILqQ4425NpxZqCDXUnb71UyS
            iq0dg+Fd0BsstqGqUIVeiC3I0guqMwmQ8l/o6xvXr4UVA0m26MqF5OPix6KDRfu4
            Dqy1qWRFNQcj0wrFen0gAmbIEX6C6EW5wmQQ46YIym1yGOXAU7owV4pF5cZL32h3
            OrhahTH6swKBgGW8YZB2S4mgAqkvxEirb//ZUV5IElXfrgnQ+Mmp+Aobyt6WYO8M
            gYxXhbdqsOHGaF64LjmywEpcTZOloUoo5sys8qRmOxDpbQsPwwjR/ChqteXFGNAn
            zxSj/lObLoqpehhl9/pH8FjT4Ey9lelSUINW8rmcm2eC/rXOoTz2xLKhAoGARYLG
            AkzsRmsgcxk1nXDRqM7zTggZBRXOKrRPktPxjddF14IcdhR/8/GdeMY3iBMPSEWW
            6grW7hMzkWJoqMZThKguZnKke9s/X0r5/6KmO5kc+8KcRTyBiaKOl8tfXZdn/p+a
            Jj6LBQh9WeXb2LsZ/yqq3U6lYcYfrd+fO2chXvUCgYA0RxpJpa8TxwCPjS7u8mZm
            enKsThRtC63+IWfaoGv3JkNiNbchpWcMDi7k0aAwT5aS1kbHMoGiCmnVTPmFYshS
            fXxzeTpm7N5Oa2kABzA6KZot1ckfA9x5Cv/6HMToNiuKmwScx4x7ASwiLSNwuxA5
            AeN9hjadhpK2ql+X9qnmkw==
            -----END PRIVATE KEY-----";

        private const string Signature = "MjUq83SdzBrT4yqoEv5qQj5GOjC6xjnvj8wm6/Q5LWxNeQSc9yl8n44vE4mw0XkiL/RyKj1fJxoMu2lgw+7Wn5J7aTcHGiURbJksW+R1GyVU5czy9D9L3ZehfZbKkED4pCwMhsjLTUlbaLRN/KjgJjdTH76C6uJFJBNYCGH0FDQe0TTy8JTaIFX6OLU/OFywasffrmnn/7kR1ue0hjb5ghfoMcQg55klYsbUihprdWPerjMsMY+2QUClTVpfG/kBFDwLn1A6ViWCF9O9yWM8nOxFmBIjQnNrFkwwBU5jMbVL6eUaizd/emOBsCe1XWN+0Unx5Ph9vyQzh86PpxvV7Q==";
        
    }
}