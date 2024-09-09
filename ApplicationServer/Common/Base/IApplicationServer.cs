using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationServer.Common.Base
{
    /// <summary>
    /// 应用服务器-接口
    /// </summary>
    public interface IApplicationServer
    {
        void Start();
        void Stop();
        void Restart();
    }
}