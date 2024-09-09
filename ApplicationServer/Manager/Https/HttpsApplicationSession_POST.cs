using Azure;
using NetCoreServer;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    /// <summary>
    /// POST 请求
    /// </summary>
    public partial class HttpsApplicationSession
    {
        /// <summary>
        /// 验证Token
        /// </summary>
        /// <param name="request"></param>
        public void HandleVerifyTokenRequest(HttpRequest request)
        {
            try
            {
                var body = request.Body;
                var parsedForm = System.Web.HttpUtility.ParseQueryString(body);

                string? token = parsedForm["Token"];

                if (TokenManager.GetInstance().ValidateJwtToken(token))
                {
                    TokenResult tokenResult = new TokenResult();
                    tokenResult.ResultCode = 200;
                    tokenResult.Message = "验证成功";
                    tokenResult.Token = token;
                    string result = JsonConvert.SerializeObject(tokenResult);

                    SetSendResponseAsync(result);
                }
                else
                {
                    Log.Error("验证失败");
                    SendResponseAsync(Response.MakeErrorResponse("验证失败"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"处理HandleVerifyTokenRequest时发生异常: {ex.Message}");
                SetErrorResponseAsync("服务器内部错误，请稍后再试。");
            }
        }

        /// <summary>
        /// 刷新Token
        /// </summary>
        /// <param name="request"></param>
        public void HandleRefreshTokenRequest(HttpRequest request)
        {
            try
            {
                var body = request.Body;
                var parsedForm = System.Web.HttpUtility.ParseQueryString(body);

                string? token = parsedForm["Token"];

                string? newToken = TokenManager.GetInstance().RefreshToken(token);

                if (newToken != "Error")
                {
                    TokenResult tokenResult = new TokenResult();
                    tokenResult.ResultCode = 200;
                    tokenResult.Message = "验证成功";
                    tokenResult.Token = token;
                    string result = JsonConvert.SerializeObject(tokenResult);

                    SetSendResponseAsync(result);
                }
                else
                {
                    Log.Error("刷新Token失败");
                    SendResponseAsync(Response.MakeErrorResponse("刷新Token失败"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"处理HandleVerifyTokenRequest时发生异常: {ex.Message}");
                SetErrorResponseAsync("服务器内部错误，请稍后再试。");
            }
        }

        /// <summary>
        /// 处理接收到的请求
        /// </summary>
        /// <param name="request"></param>
        public void HandleLoginRequest(HttpRequest request)
        {
            try
            {
                var body = request.Body;
                var parsedForm = System.Web.HttpUtility.ParseQueryString(body);

                string? username = parsedForm["Username"];
                string? password = parsedForm["Password"];

                if (UserManager.GetInstance().ValidateUser(username, password))
                {
                    string token = TokenManager.GetInstance().GenerateToken(username);

                    LoginResult loginResult = new LoginResult();
                    //loginResult.User = UserManager.GetInstance().GetUser(username);
                    loginResult.Token = token;
                    string result = JsonConvert.SerializeObject(loginResult);

                    SetSendResponseAsync(result);
                }
                else
                {
                    Log.Error("用户不存在,或账户密码错误。");
                    SendResponseAsync(Response.MakeErrorResponse("用户不存在,或账户密码错误。"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"处理HandleLoginRequest时发生异常: {ex.Message}");
                SetErrorResponseAsync("服务器内部错误，请稍后再试。");
            }
        }

        /// <summary>
        /// 处理接收到的请求
        /// </summary>
        /// <param name="request"></param>
        public async void HandleRegisterRequest(HttpRequest request)
        {
            try
            {
                var body = request.Body;
                var parsedForm = System.Web.HttpUtility.ParseQueryString(body);

                string? username = parsedForm["Username"];
                string? password = parsedForm["Password"];
                string? email = parsedForm["Email"];
                int permissions = int.Parse(parsedForm["Permissions"]);

                User user = UserManager.GetInstance().GetUser(username);
                if (user == null)
                {
                    bool insertResult = await DatabaseManager.GetInstance().InsertUserAsync(username,
                                            password,
                                            email,
                                            permissions);

                    if (insertResult)
                    {
                        string token = TokenManager.GetInstance().GenerateToken(username);

                        RegisterResult registerResult = new RegisterResult();
                        //registerResult.User = UserManager.GetInstance().GetUser(username);
                        registerResult.Token = token;
                        string result = JsonConvert.SerializeObject(registerResult);

                        SetSendResponseAsync(result);
                    }
                    else
                    {
                        Log.Error("注册失败，数据无法写入，请联系管理员。");
                        SetErrorResponseAsync("注册失败，数据无法写入，请联系管理员。");
                    }
                }
                else
                {
                    Log.Error("用户已存在");
                    SendResponseAsync(Response.MakeErrorResponse("用户已存在"));
                }
            }
            catch (Exception ex)
            {
                Log.Error($"处理HandleRegisterRequest时发生异常: {ex.Message}");
                SetErrorResponseAsync("服务器内部错误，请稍后再试。");
            }
        }

    }
}
