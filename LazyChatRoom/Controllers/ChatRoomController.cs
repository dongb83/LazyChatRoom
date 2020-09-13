using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Threading.Tasks;
using LazyWeChat.Abstract.OfficialAccount;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace LazyChatRoom.Controllers
{
    public class ChatRoomController : Controller
    {
        private readonly ILogger<ChatRoomController> _logger;
        private readonly ILazyWeChatBasic _lazyWeChatBasic;

        public ChatRoomController(
            ILogger<ChatRoomController> logger,
            ILazyWeChatBasic lazyWeChatBasic)
        {
            _logger = logger;
            _lazyWeChatBasic = lazyWeChatBasic;
        }

        public IActionResult GetAuthorizationCode()
        {
            var isWeixinBrowser = false;
            string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            if (userAgent.ToLower().Contains("micromessenger"))
                isWeixinBrowser = true;

            var code = "";
            if (isWeixinBrowser)
                code = _lazyWeChatBasic.GetAuthorizationCode(this.HttpContext, SCOPE.snsapi_userinfo);
            else
                code = _lazyWeChatBasic.GetQRConnectCode(this.HttpContext);

            if (!string.IsNullOrEmpty(code))
                return Redirect($"Index?code={code}");
            else
                return View();
        }

        public async Task<IActionResult> Index()
        {
#if DEBUG
            ViewBag.userInfo = "{\"openid\":\"oNDiC0d-r7Su5mYCU-mXFSXuhmtQ\",\"nickname\":\"dongbo\",\"sex\":\"1\",\"language\":\"zh_CN\",\"city\":\"Pudong New District\",\"province\":\"Shanghai\",\"country\":\"China\",\"headimgurl\":\"http://thirdwx.qlogo.cn/mmopen/vi_32/euD61xrF7LY4Juuj6TqMNzgeMVI1bIDKcEobRmiaAmNwnyJCjUXWZ79VwCv4RJZyUzkLfVFQBL6LFqcrKiacDfdw/132\",\"unionid\":\"ohksXwX2Et71NuQpc0BM0ZiQDs5A\",\"privilege\":[]}";
#else

            var code = "";
            if (!HttpContext.Request.Query.Keys.Contains("code"))
                return Redirect("ChatRoom/GetAuthorizationCode");
            else
                code = HttpContext.Request.Query["code"].ToString();


            var isWeixinBrowser = false;
            string userAgent = HttpContext.Request.Headers["User-Agent"].ToString();
            if (userAgent.ToLower().Contains("micromessenger"))
                isWeixinBrowser = true;

            dynamic webAccessToken = new ExpandoObject();

            if (isWeixinBrowser)
                webAccessToken = await _lazyWeChatBasic.GetWebAccessTokenAsync(code);
            else
                webAccessToken = await _lazyWeChatBasic.GetOpenWebAccessTokenAsync(code);

            var access_token = webAccessToken.access_token;
            var openid = webAccessToken.openid;
            var res = await _lazyWeChatBasic.GetUserInfoAsync(access_token, openid, "zh-CN");
            var userInfo = JsonConvert.SerializeObject(res);
            ViewBag.userInfo = userInfo;

            _logger.LogInformation($"UserInfo :{userInfo}");
#endif
            return View();
        }
    }
}
