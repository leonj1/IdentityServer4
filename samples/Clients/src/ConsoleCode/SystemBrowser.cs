using IdentityModel.OidcClient.Browser;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace ConsoleClientWithBrowser
{
    public class SystemBrowser : IBrowser
    {
        public int Port { get; }
        private readonly string _path;

        public SystemBrowser(int? port = null, string path = null)
        {
            _path = path;

            if (!port.HasValue)
            {
                Port = GetRandomUnusedPort();
            }
            else
            {
                Port = port.Value;
            }
        }

        private int GetRandomUnusedPort()
        {
            var listener = new TcpListener(IPAddress.Loopback, 0);
            listener.Start();
            var port = ((IPEndPoint)listener.LocalEndpoint).Port;
            listener.Stop();
            return port;
        }

        public async Task<BrowserResult> InvokeAsync(BrowserOptions options, CancellationToken cancellationToken = default)
        {
            using (var loopbackHttpListener = new LoopbackHttpListener(Port, _path))
            {
                var url = loopbackHttpListener.Url;

                try
                {
                    await Process.Start(new ProcessStartInfo
                    {
                        FileName = "cmd.exe",
                        Arguments = $"/c start {url}",
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    });

                    var result = await loopbackHttpListener.WaitForCallbackAsync();
                    return new BrowserResult { Result = result };
                }
                catch (Exception ex)
                {
                    return new BrowserResult { Error = ex.Message };
                }
            }
        }

        public void Dispose()
        {
            // No-op for now
        }
    }

    public class LoopbackHttpListener : IDisposable
    {
        IWebHost _host;
        TaskCompletionSource<string> _source = new TaskCompletionSource<string>();
        string _url;

        public string Url => _url;

        public LoopbackHttpListener(int port, string path = null)
        {
            path = path ?? String.Empty;
            if (path.StartsWith("/")) path = path.Substring(1);

            _url = $"http://127.0.0.1:{port}/{path}";

            _host = new WebHostBuilder()
                .UseKestrel()
                .UseUrls(_url)
                .Configure(Configure)
                .Build();
            _host.Start();
        }

        public void Dispose()
        {
            Task.Run(async () =>
            {
                await Task.Delay(500);
                _host.Dispose();
            });
        }

        void Configure(IApplicationBuilder app)
        {
            app.Run(async ctx =>
            {
                if (ctx.Request.Method == "GET")
                {
                    SetResult(ctx.Request.QueryString.Value, ctx);
                }
                else if (ctx.Request.Method == "POST")
                {
                    if (!ctx.Request.ContentType.Equals("application/x-www-form-urlencoded", StringComparison.OrdinalIgnoreCase))
                    {
                        ctx.Response.StatusCode = 415;
                    }
                    else
                    {
                        using (var sr = new StreamReader(ctx.Request.Body, Encoding.UTF8))
                        {
                            var body = await sr.ReadToEndAsync();
                            SetResult(body, ctx);
                        }
                    }
                }
                else
                {
                    ctx.Response.StatusCode = 405;
                }
            });
        }

        private void SetResult(string value, HttpContext ctx)
        {
            try
            {
                ctx.Response.StatusCode = 200;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>You can now return to the application.</h1>");
                ctx.Response.Body.Flush();

                _source.TrySetResult(value);
            }
            catch
            {
                ctx.Response.StatusCode = 400;
                ctx.Response.ContentType = "text/html";
                ctx.Response.WriteAsync("<h1>Invalid request.</h1>");
                ctx.Response.Body.Flush();
            }
        }

        public Task<string> WaitForCallbackAsync(int timeoutInSeconds = 300)
        {
            Task.Run(async () =>
            {
                await Task.Delay(timeoutInSeconds * 1000);
                _source.TrySetCanceled();
            });

            return _source.Task;
        }
    }
}
