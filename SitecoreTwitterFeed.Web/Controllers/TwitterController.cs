using Newtonsoft.Json.Linq;
using SitecoreTwitterFeed.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace SitecoreTwitterFeed.Web.Controllers
{
    public class TwitterController : Controller
    {
        // GET: Twitter Feeds
        public ActionResult TwitterFeed()
        {
            var twitterSettings = Sitecore.Context.Database.GetItem("/sitecore/content/Home/Twitter Settings");

            var twitterViewModel = new TwitterViewModel()
            {
                Tweets = GetTweets(twitterSettings)
            };

            return View(twitterViewModel);
        }

        private List<Tweet> GetTweets(Sitecore.Data.Items.Item twitterSettings)
        {
            var tweets = new List<Tweet>();

            var oauth_token = twitterSettings.Fields["OauthToken"].Value;
            var oauth_token_secret = twitterSettings.Fields["OauthTokenSecret"].Value;
            var oauth_consumer_key = twitterSettings.Fields["OauthConsumerKey"].Value;
            var oauth_consumer_secret = twitterSettings.Fields["OauthConsumerSecret"].Value;
            var query = twitterSettings.Fields["TwitterAccountName"].Value;

            // oauth implementation details
            var oauth_version = twitterSettings.Fields["OauthVersion"].Value;
            var oauth_signature_method = twitterSettings.Fields["OauthSignatureMethod"].Value;

            // unique request details
            var oauth_nonce = Convert.ToBase64String(new ASCIIEncoding().GetBytes(DateTime.Now.Ticks.ToString()));
            var timeSpan = DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc);
            var oauth_timestamp = Convert.ToInt64(timeSpan.TotalSeconds).ToString();

            // create oauth signature
            var baseFormat = "oauth_consumer_key={0}&oauth_nonce={1}&oauth_signature_method={2}" +
                            "&oauth_timestamp={3}&oauth_token={4}&oauth_version={5}&q={6}";

            var baseString = string.Format(baseFormat,
                                        oauth_consumer_key,
                                        oauth_nonce,
                                        oauth_signature_method,
                                        oauth_timestamp,
                                        oauth_token,
                                        oauth_version,
                                        Uri.EscapeDataString(query)
                                        );

            string resource_url = twitterSettings.Fields["TwitterApiUrl"].Value;
            baseString = string.Concat("GET&", Uri.EscapeDataString(resource_url), "&", Uri.EscapeDataString(baseString));

            var compositeKey = string.Concat(Uri.EscapeDataString(oauth_consumer_secret),
                                    "&", Uri.EscapeDataString(oauth_token_secret));

            string oauth_signature;
            using (HMACSHA1 hasher = new HMACSHA1(ASCIIEncoding.ASCII.GetBytes(compositeKey)))
            {
                oauth_signature = Convert.ToBase64String(
                    hasher.ComputeHash(ASCIIEncoding.ASCII.GetBytes(baseString)));
            }

            // create the request header
            var headerFormat = "OAuth oauth_nonce=\"{0}\", oauth_signature_method=\"{1}\", " +
                               "oauth_timestamp=\"{2}\", oauth_consumer_key=\"{3}\", " +
                               "oauth_token=\"{4}\", oauth_signature=\"{5}\", " +
                               "oauth_version=\"{6}\"";

            var authHeader = string.Format(headerFormat,
                                    Uri.EscapeDataString(oauth_nonce),
                                    Uri.EscapeDataString(oauth_signature_method),
                                    Uri.EscapeDataString(oauth_timestamp),
                                    Uri.EscapeDataString(oauth_consumer_key),
                                    Uri.EscapeDataString(oauth_token),
                                    Uri.EscapeDataString(oauth_signature),
                                    Uri.EscapeDataString(oauth_version)
                            );

            ServicePointManager.Expect100Continue = false;

            // make the request
            var postBody = "q=" + Uri.EscapeDataString(query);
            resource_url += "?" + postBody;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(resource_url);

            //Check if proxy authentication is applicable or not
            if (twitterSettings.Fields["AuthenticateWithProxy"].Value == "1")
            {
                Uri newUri = new Uri(twitterSettings.Fields["ProxyUrl"].Value);

                //Web Proxy 
                WebProxy proxy = new WebProxy
                {
                    // Associate the newUri object to 'myProxy' object so that new myProxy settings can be set.
                    Address = newUri,
                    // Create a NetworkCredential object and associate it with the Proxy property of request object.
                    Credentials = new NetworkCredential(twitterSettings.Fields["ProxyUsername"].Value, twitterSettings.Fields["ProxyPassword"].Value)
                };

                request.Proxy = proxy;
            }
            //Proxy Ends here

            request.Headers.Add("Authorization", authHeader);
            request.Method = "GET";
            request.ContentType = "application/x-www-form-urlencoded";
            try
            {
                var response = (HttpWebResponse)request.GetResponse();
                var reader = new StreamReader(response.GetResponseStream());
                var objText = reader.ReadToEnd();

                JArray jsonTweet = JArray.Parse(objText);
                Tweet tweetItem = null;
                for (int x = 0; x < jsonTweet.Count(); x++)
                {
                    tweetItem = new Tweet();
                    tweetItem.TweetMessage = jsonTweet[x]["text"].ToString();
                    DateTime createdAt = DateTime.ParseExact((string)jsonTweet[x]["created_at"], "ddd MMM dd HH:mm:ss +ffff yyyy", new System.Globalization.CultureInfo("en-US"));
                    tweetItem.TweetDateTime = createdAt;
                    tweets.Add(tweetItem);

                    if (!string.IsNullOrEmpty(twitterSettings.Fields["TweetCount"].Value) && x >= Convert.ToInt32(twitterSettings.Fields["TweetCount"].Value) - 1)
                        break;
                }

                return tweets;
            }
            catch (Exception ex)
            {

            }

            return tweets;
        }
    }
}