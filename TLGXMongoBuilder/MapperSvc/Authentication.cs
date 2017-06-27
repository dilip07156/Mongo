using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.Web;
using System.IO;
using System.Collections.Specialized;

namespace MapperSvc
{
    public static class Authentication
    {
        public static object NameValueCollections { get; private set; }

        public static bool Authenticate(IncomingWebRequestContext context)
        {
            bool Authenticated = false;
            string normalizedUrl;
            string normalizedRequestParameters;

            //context headers
            NameValueCollection pa = context.UriTemplateMatch.QueryParameters;

            if (pa != null && pa["oauth_consumer_key"] != null)
            {
                string uri = context.UriTemplateMatch.RequestUri.OriginalString.Replace(context.UriTemplateMatch.RequestUri.Query, "");
                string consumerSecret = "MapSvc";
                OAuthBase oauth = new OAuthBase();
                string hash = oauth.GenerateSignature(
                                new Uri(uri),
                                pa["oauth_consumer_key"],
                                consumerSecret,
                                null, //token
                                null, //token secret
                                "GET",
                                pa["oauth_timestamp"], 
                                pa["oauth_nonce"],
                                out normalizedUrl,
                                out normalizedRequestParameters
                                );

                Authenticated = pa["oauth_signature"] == hash;
            }

            return Authenticated;
        }
    }
}