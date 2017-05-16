using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SitecoreTwitterFeed.Web.Models
{
    public class TwitterViewModel
    {
        #region Properties
        /// <summary>
        /// Twitter Tweets 
        /// </summary>
        public List<Tweet> Tweets { get; set; }

        /// <summary>
        /// OauthToken
        /// </summary>
        public string OauthToken { get; set; }

        /// <summary>
        /// Oauth Token Secret
        /// </summary>
        public string OauthTokenSecret { get; set; }

        /// <summary>
        /// Oauth Consumer Key
        /// </summary>
        public string OauthConsumerKey { get; set; }

        /// <summary>
        /// Oauth Consumer Secret
        /// </summary>
        public string OauthConsumerSecret { get; set; }

        /// <summary>
        /// Oauth Version
        /// </summary>
        public string OauthVersion { get; set; }

        /// <summary>
        /// Oauth Signature Method
        /// </summary>
        public string OauthSignatureMethod { get; set; }

        /// <summary>
        /// Twitter Account Name
        /// </summary>
        public string TwitterAccountName { get; set; }

        /// <summary>
        /// Tweet Count
        /// </summary>
        public string TweetCount { get; set; }

        /// <summary>
        /// Twitter Api Url
        /// </summary>
        public string TwitterApiUrl { get; set; }

        /// <summary>
        /// Authenticate with proxy
        /// </summary>
        public bool AuthenticateWithProxy { get; set; }

        /// <summary>
        /// Proxy Url
        /// </summary>
        public string ProxyUrl { get; set; }

        /// <summary>
        /// Proxy User name
        /// </summary>
        public string ProxyUsername { get; set; }

        /// <summary>
        /// Proxy Password
        /// </summary>
        public string ProxyPassword { get; set; }
        #endregion
    }

    public class Tweet
    {
        #region Properties
        /// <summary>
        /// Tweet Message
        /// </summary>
        public string TweetMessage { get; set; }

        /// <summary>
        /// Tweet Date Time
        /// </summary>
        public DateTime TweetDateTime { get; set; }
        #endregion
    }
}