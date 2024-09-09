using Azure;
using NetCoreServer;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    public partial class HttpsApplicationSession : HttpsSession
    {
        // 最后活动时间
        public DateTime LastActivity { get; private set; }

        public HttpsApplicationSession(HttpsServer server) : base(server)
        {
            InitApiRoutes();
            LastActivity = DateTime.UtcNow; // 更新最后活动时间
        }

        protected override void OnReceived(byte[] buffer, long offset, long size)
        {
            base.OnReceived(buffer, offset, size);
            LastActivity = DateTime.UtcNow; // 更新最后活动时间
        }

        protected override void OnSent(long sent, long pending)
        {
            base.OnSent(sent, pending);
            LastActivity = DateTime.UtcNow; // 更新最后活动时间
        }

        protected override void OnReceivedRequestError(HttpRequest request, string error)
        {
            base.OnReceivedRequestError(request, error);
            Log.Error($"请求错误: {error}");
            LastActivity = DateTime.UtcNow; // 更新最后活动时间
        }

        protected override void OnError(SocketError error)
        {
            base.OnError(error);
            Log.Error($"HTTPS会话捕获错误: {error}");
            LastActivity = DateTime.UtcNow; // 更新最后活动时间
        }

        //-----------------------------------------------------------------------------------------------------

        /// <summary>
        /// 处理接收到的请求
        /// </summary>
        /// <param name="request"></param>
        protected override void OnReceivedRequest(HttpRequest request)
        {
            // 显示HTTP请求内容
            Console.WriteLine(request);

            switch (request.Method)
            {
                case "HEAD":
                    HandleHeadRequest(request);
                    break;
                case "GET":
                    HandleGetRequest(request);
                    break;
                case "POST":
                    HandlePostRequest(request);
                    break;
                case "PUT":
                    HandlePutRequest(request);
                    break;
                case "DELETE":
                    HandleDeleteRequest(request);
                    break;
                case "OPTIONS":
                    HandleOptionsRequest(request);
                    break;
                case "TRACE":
                    HandleTraceRequest(request);
                    break;
                default:
                    SendResponseAsync(Response.MakeErrorResponse("Unsupported HTTP method: " + request.Method));
                    break;
            }
        }

        #region HADE
        private void HandleHeadRequest(HttpRequest request)
        {
            SendResponseAsync(Response.MakeHeadResponse());
        }
        #endregion

        #region Get
        private void HandleGetRequest(HttpRequest request)
        {
            string key = request.Url;
            key = Uri.UnescapeDataString(key);

            foreach (var route in apiGetRoutes)
            {
                if (key.StartsWith(route.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    key = key.Replace(route.Key, "", StringComparison.InvariantCultureIgnoreCase);
                    route.Value(key);
                    return;
                }
            }
        }
        #endregion

        #region POST
        private void HandlePostRequest(HttpRequest request)
        {
            string key = request.Url;
            key = Uri.UnescapeDataString(key);

            foreach (var route in apiPostRoutes)
            {
                if (key.StartsWith(route.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    route.Value(request);
                    return;
                }
            }
        }
        #endregion

        #region PUT
        private void HandlePutRequest(HttpRequest request)
        {
            string key = request.Url;
            key = Uri.UnescapeDataString(key);
            Log.Debug($"PUT请求: {key}");

            foreach (var route in apiPutRoutes)
            {
                if (key.StartsWith(route.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    key = key.Replace(route.Key, "", StringComparison.InvariantCultureIgnoreCase);
                    route.Value(key);
                    return;
                }
            }
        }
        #endregion

        #region DELETE
        private void HandleDeleteRequest(HttpRequest request)
        {
            string key = request.Url;
            key = Uri.UnescapeDataString(key);
            Log.Debug($"DELETE请求: {key}");

            foreach (var route in apiDeleteRoutes)
            {
                if (key.StartsWith(route.Key, StringComparison.InvariantCultureIgnoreCase))
                {
                    key = key.Replace(route.Key, "", StringComparison.InvariantCultureIgnoreCase);
                    route.Value(key);
                    return;
                }
            }
        }
        #endregion

        #region OPTIONS
        private void HandleOptionsRequest(HttpRequest request)
        {
            SendResponseAsync(Response.MakeOptionsResponse());
        }
        #endregion

        #region TRACE
        private void HandleTraceRequest(HttpRequest request)
        {
            SendResponseAsync(Response.MakeTraceResponse(request.Cache.Data));
        }
        #endregion

        /// <summary>
        /// 设置响应并发送响应
        /// </summary>
        /// <param name="result"></param>
        private void SetSendResponseAsync(string result)
        {
            var r = Response;
            r.Clear();
            r.SetBegin(200);
            r.SetHeader("Content-Type", "text/plain; charset=UTF-8");
            r.SetHeader("Access-Control-Allow-Origin", "*");
            r.SetHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            r.SetBody(result);

            Log.Debug("SetSendResponseAsync:" + r.ToString());

            SendResponseAsync(r);
        }

        /// <summary>
        /// 设置响应和错误响应
        /// </summary>
        /// <param name="result"></param>
        private void SetErrorResponseAsync(string result)
        {
            var r = Response;
            r.Clear();
            r.SetBegin(500);
            r.SetHeader("Content-Type", "application/octet-stream");
            r.SetHeader("Access-Control-Allow-Origin", "*");
            r.SetHeader("Access-Control-Allow-Methods", "GET, POST, PUT, DELETE, OPTIONS");
            r.SetBody(result);

            Log.Debug("SetErrorResponseAsync:" + r.ToString());

            SendResponseAsync(r);
        }
    }
}
