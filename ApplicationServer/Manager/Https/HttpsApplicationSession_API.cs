using NetCoreServer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Manager
{
    public partial class HttpsApplicationSession
    {
        private Dictionary<string, Action<string>> apiGetRoutes;
        private Dictionary<string, Action<HttpRequest>> apiPostRoutes;
        private Dictionary<string, Action<string>> apiPutRoutes;
        private Dictionary<string, Action<string>> apiDeleteRoutes;
        private Dictionary<string, Action<string>> apiOptionsRoutes;
        private Dictionary<string, Action<string>> apiTraceRoutes;

        public void InitApiRoutes()
        {
            apiGetRoutes = new Dictionary<string, Action<string>>(StringComparer.InvariantCultureIgnoreCase)
            {

            };

            apiPostRoutes = new Dictionary<string, Action<HttpRequest>>(StringComparer.InvariantCultureIgnoreCase)
            {
                { "/api/login", HandleLoginRequest },
                { "/api/register", HandleRegisterRequest },

                { "/api/verify/token", HandleVerifyTokenRequest },
                { "/api/refresh/token", HandleRefreshTokenRequest },
            };
        }
    }
}
