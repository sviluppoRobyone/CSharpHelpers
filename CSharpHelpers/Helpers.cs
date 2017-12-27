using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Web.Http.Results;
using System.Web.Http.Routing;

namespace CSharpHelpers
{
    public static class Helpers
    {
        public static T ToEnum<T>(this string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        public static T WaitAndGetData<T>(this Task<T> task)
        {
            task.Wait();
            return task.Result;
        }

        public static Version GetAssemblyVersion(this Type t)
        {
            return t.Assembly.GetName().Version;
        }

        public static DateTime GetLastTimeEdit(this Type t)
        {
            return System.IO.File.GetLastWriteTime(t.Assembly.Location);
        }
        //http://stackoverflow.com/questions/249087/how-do-i-remove-diacritics-accents-from-a-string-in-net
        public static string RemoveAccents(string text)
        {
            var normalizedString = text.Normalize(NormalizationForm.FormD);
            var stringBuilder = new StringBuilder();

            foreach (var c in normalizedString)
            {
                var unicodeCategory = CharUnicodeInfo.GetUnicodeCategory(c);
                if (unicodeCategory != UnicodeCategory.NonSpacingMark)
                {
                    stringBuilder.Append(c);
                }
            }

            return stringBuilder.ToString().Normalize(NormalizationForm.FormC);
        }

        public const string Md5Regex = "^[0-9a-fA-F]{32}$";
        public const string Base64Regex = "^(?:[A-Za-z0-9+/]{4})*(?:[A-Za-z0-9+/]{2}==|[A-Za-z0-9+/]{3}=)?$";

        public static bool IsBase64String(this string s)
        {//http://stackoverflow.com/questions/475074/regex-to-parse-or-validate-base64-data
            return !string.IsNullOrEmpty(s) &&
                   Regex.IsMatch(s, Base64Regex);
        }
        public static string HashPassword(string password)
        {
            return Crypto.HashPassword(password);
        }

        public static bool VerifyPassword(string password, string hashedPassword)
        {
            return Crypto.VerifyHashedPassword(hashedPassword, password);

        }

        public static bool IsMd5(this string s)
        {
            return !string.IsNullOrEmpty(s) && Regex.IsMatch(s, "^[0-9a-fA-F]{32}$");
        }
        public static string RandomMd5(params object[] seeds)
        {
            if (seeds == null)
                seeds = new object[] { };
            var myseed = seeds.Select(x => x.ToString()).ToList();
            myseed.Add(Guid.NewGuid().ToString());
            return CalculateMd5(string.Concat(myseed));

        }

        public static string CalculateMd5(byte[] content)
        {
            using (var md5 = MD5.Create())
            {
                byte[] retVal = md5.ComputeHash(content);
                StringBuilder sb = new StringBuilder();
                for (int i = 0; i < retVal.Length; i++)
                {
                    sb.Append(retVal[i].ToString("x2"));
                }
                return sb.ToString();
            }
        }
        public static string CalculateMd5(string input)
        {
            return CalculateMd5(Encoding.Unicode.GetBytes(input));

        }

        public static string ToMd5(this string input)
        {
            return CalculateMd5(input);
        }

        public static string ToMd5<T>(this IEnumerable<T> input)
        {
            return string.Join("",
                input.Where(x => x != null).Select(x => Newtonsoft.Json.JsonConvert.SerializeObject(x))).ToMd5();
        }

        public static bool IsSuccessfulResponse(this IHttpActionResult result)
        {

            var t = result.GetType();
            var genTypes = new[] { typeof(OkNegotiatedContentResult<>), typeof(CreatedAtRouteNegotiatedContentResult<>), typeof(CreatedNegotiatedContentResult<>) };

            return (result is OkResult)
                   ||
                   (result is StatusCodeResult && ((int)((StatusCodeResult)(result)).StatusCode).ToString().StartsWith("20"))
                   ||
                   (t.IsGenericType && genTypes.Any(x => t.GetGenericTypeDefinition().IsAssignableFrom(x)));

        }

        public static T As<T>(this IHttpActionResult result)
        {
            var contentResult = result as OkNegotiatedContentResult<T>;
            if (contentResult != null)
                return contentResult.Content;
            throw new ArgumentException(nameof(result));
        }

        public static async Task<T> As<T>(this Task<IHttpActionResult> result)
        {
            return (await result).As<T>();
        }

        public static byte[] AsByteArray(this HttpPostedFile file)
        {
            using (var reader = new BinaryReader(file.InputStream))
            {
                return reader.ReadBytes((int)file.InputStream.Length);
            }
        }

        public static string AsBase64String(this HttpPostedFile file)
        {
            return Convert.ToBase64String(file.AsByteArray());
        }
        public static bool HasParam(this IHttpRouteData routeData, string paramName)
        {
            return routeData.Values.ContainsKey(paramName);
        }
        public static int? AsIntParam(this IHttpRouteData routeData, string paramName)
        {
            var val = default(int?);


            if (routeData.HasParam(paramName))
            {
                var rawData = routeData.Values[paramName].ToString();

                try
                {
                    val = int.Parse(rawData);

                    if (val < 0)
                        val = null;
                }
                catch (Exception e)
                {

                }

            }
            return val;

        }

        public static int? GetIntParam(this HttpActionContext a, string paramName)
        {
            return a.Request.GetRouteData().AsIntParam(paramName);
        }
        public static bool HasParam(this HttpActionContext a, string paramName)
        {
            return a.Request.GetRouteData().HasParam(paramName);
        }

        public static List<HttpPostedFile> ListUploadedFiles(this HttpRequest req)
        {
            var list = new List<HttpPostedFile>();
            foreach (string x in req.Files)
            {
                list.Add(req.Files[x]);
            }
            return list;
        }
    }
}

