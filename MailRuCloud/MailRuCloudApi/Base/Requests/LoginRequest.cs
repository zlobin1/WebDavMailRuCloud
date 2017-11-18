﻿using System;
using System.Linq;
using System.Net;
using System.Text;
using YaR.MailRuCloud.Api.Base.Requests.Types;

namespace YaR.MailRuCloud.Api.Base.Requests
{
    class LoginRequest : BaseRequest<LoginResult>
    {
        private readonly IBasicCredentials _credentials;

        public LoginRequest(CloudApi cloudApi, IBasicCredentials credentials) : base(cloudApi)
        {
            _credentials = credentials;
        }

        protected override HttpWebRequest CreateRequest(string baseDomain = null)
        {
            var request = base.CreateRequest(ConstSettings.AuthDomain);
            request.Accept = ConstSettings.DefaultAcceptType;
            return request;
        }

        protected override string RelationalUri => "/cgi-bin/auth?lang=ru_RU&from=authpopup";

        protected override byte[] CreateHttpContent()
        {
            string data = $"Login={Uri.EscapeUriString(_credentials.Login)}&Password={Uri.EscapeUriString(_credentials.Password)}";
            data += $"&page={Uri.EscapeUriString("https://cloud.mail.ru/?from=promo")}&FailPage=&Domain=mail.ru&new_auth_form=1&saveauth=1";
            return Encoding.UTF8.GetBytes(data);
        }

        protected override RequestResponse<LoginResult> DeserializeMessage(string responseText)
        {
            var csrf = responseText.Contains("csrf")
                ? new string(responseText.Split(new[] {"csrf"}, StringSplitOptions.None)[1].Split(',')[0].Where(char.IsLetterOrDigit).ToArray())
                : string.Empty;

            var msg = new RequestResponse<LoginResult>
            {
                Ok = true,
                Result = new LoginResult
                {
                    Csrf = csrf
                }
            };
            return msg;
        }
    }
}
