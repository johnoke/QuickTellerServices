using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using RestSharp.Portable;
using RestSharp.Portable.Deserializers;
using System.Net;
using System.Net.Http;
using Payment.Models;
using Payment.Utils;
using System.Collections;
using Org.BouncyCastle.Crypto.Engines;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.Utilities.Encoders;
using System.Security.Cryptography;
using System.Globalization;
using Payment.Helpers;
using System.Linq;
using Org.BouncyCastle.Crypto.Macs;
using Org.BouncyCastle.Math;

namespace Payment
{

    public class Interswitch
    {
        public static string ClientId;
        public static string ClientSecret;
        public static string MyAccessToken;
        public static string Environment;
        public static string AuthData;
        public static string FrontEndPartnerId;

        public static String GetAuthData(string pan, string pin, string expiryDate, string cvv)
        {
            AuthData = Crypto.GetAuthData(pan, pin, expiryDate, cvv);
            return AuthData;
        }
        public static string GetAuthData(string mod, string pubExpo, string pan, string pin, string expiryDate, string cvv)
        {
            AuthData = Crypto.GetAuthData(mod, pubExpo, pan, pin, expiryDate, cvv);
            return AuthData;
        }
        public static string GetAuthData(string certificatePath, string pan, string pin, string expiryDate, string cvv2)
        {
            AuthData = Crypto.GetAuthData(certificatePath, pan, pin, expiryDate, cvv2);
            return AuthData;
        }

        public static Dictionary<string, string> GetSecureData(string subscriberId, string ttid, string pan, string pin, string cvv2, string expiryDate, string paymentMethodTypeCode)
        {
            var secure = new InterswitchSecure.InterswitchSecure(ClientId, ClientSecret, Environment);
            Byte[] pinKeyBytes = InterswitchSecure.DESUtils.generateKey();
            String pinKeyHex = InterswitchSecure.HexConverter.ByteArrayToString(pinKeyBytes);
            Dictionary<string, string> result = secure.GetSecureData(pan, expiryDate, cvv2, pin, null, "07036748693", "98323232");
            //var result = secure.GetSecureData(pan: pan, expDate: expiryDate, cvv: cvv2, pin: pin, msisdn: subscriberId, ttid: ttid);
            return result;
        }
        

        public static async Task<string> GetToken(string environment)
        {
            var accessToken = await GetClientAccessToken(ClientId, ClientSecret, environment);
            return accessToken.access_token;
        }

        public static string CalculateSecurePinVersion9(string pin, byte[] pinKey, string cvv2 = "000", string expiryDate = "0000")
        {
            if (pinKey == null)
            {
                throw new ArgumentNullException("pinKey");
            }
            var pinBlockString = pin + cvv2 + expiryDate;
            var clearPinBlock = "2" + pinBlockString.Length + pinBlockString;
            const byte randomBytes = 0x0;
            var randomDigit = (int)((randomBytes * 10) / 128);
            randomDigit = Math.Abs(randomDigit);
            var pinPadLength = 16 - clearPinBlock.Length;

            for (var i = 0; i < pinPadLength; i++)
            {
                clearPinBlock += randomDigit;
            }

            var desEdeEngine = new DesEdeEngine();
            var keyParameters = new DesEdeParameters(pinKey);
            desEdeEngine.Init(true, keyParameters);
            var clearPinBlockBytes = Hex.Decode(clearPinBlock);
            var encryptedPinBlockBytes = new byte[8];
            desEdeEngine.ProcessBlock(clearPinBlockBytes, 0, encryptedPinBlockBytes, 0);
            var encodedEncryptedPinBlockBytes = Hex.Encode(encryptedPinBlockBytes);
            var securePin = Encoding.UTF8.GetString(encodedEncryptedPinBlockBytes, 0, encodedEncryptedPinBlockBytes.Length);
            return securePin;
        }

        public static object CalculateSecurePinVersion9(string v, object p1, object p2, object p3)
        {
            throw new NotImplementedException();
        }

        public static string CalculateSecureData12(PayPhoneSecureData secureDataInfo, byte[] macDesBytes, string macDataVaue = null)
        {
            var secureBytes = new byte[64];
            var headerBytes = Hex.Decode(secureDataInfo.PayPhoneSecureDataHeaderValue);

            var formatVersionBytes = Hex.Decode(secureDataInfo.PayPhoneSecureDataVersion);

            var macVersionBytes = Hex.Decode(secureDataInfo.PayPhoneSecureMacVersion);

            var footerBytes = Hex.Decode(secureDataInfo.FooterBytes);


            byte[] pinDesKey;
            try
            {
                if (secureDataInfo.PinDesKey.Length == 32)
                {
                    pinDesKey = Hex.Decode(secureDataInfo.PinDesKey);
                }
                else
                {
                    pinDesKey = Hex.Decode(string.Format("{0}{1}", secureDataInfo.PinDesKey.Length.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), secureDataInfo.PinDesKey.PadRight(30, '0')));
                }
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while Hex.Decoding account number: {0}", secureDataInfo.PinDesKey), e);
            }

            byte[] customerIdBytes;
            try
            {
                var customerId = string.IsNullOrWhiteSpace(secureDataInfo.CustomerId) ? secureDataInfo.Pan : secureDataInfo.CustomerId;

                customerIdBytes = Hex.Decode(string.Format("{0}{1}{2}", (customerId.Length).ToString().Length, customerId.Length, customerId).PadRight(40, '0'));
            }
            catch (Exception e)
            {
                throw new Exception(string.Format("Error while Hex.Decoding pan: {0}", secureDataInfo.Pan), e);
            }

            var macData = "";
            if (string.IsNullOrWhiteSpace(macDataVaue))
            {
                macData = string.Format("{0}default{1}", secureDataInfo.PrimaryMobileNumber, secureDataInfo.Identifier);
            }
            else
            {
                macData = macDataVaue;
            }

            var macBytes = BouncyCastleGetCbcMac(macData, macDesBytes);

            //   macBytes = Hex.Encode(macBytes);

            Array.Copy(headerBytes, 0, secureBytes, 0, 1);

            Array.Copy(formatVersionBytes, 0, secureBytes, 1, 1);

            Array.Copy(macVersionBytes, 0, secureBytes, 2, 1);

            Array.Copy(pinDesKey, 0, secureBytes, 3, 16);

            Array.Copy(macDesBytes, 0, secureBytes, 19, 16);
            Array.Copy(customerIdBytes, 0, secureBytes, 35, 20);
            Array.Copy(macBytes, 0, secureBytes, 55, 4);

            //use this string to pard the data
            var paddingStringBytes = Hex.Decode(SetFixedLength("", 28, true, '0'));
            Array.Copy(paddingStringBytes, 0, secureBytes, 59, 4);

            Array.Copy(footerBytes, 0, secureBytes, 63, 1);


            var encryptedSecureData = Hex.Encode(BouncyCastleRsaEncrypt(secureBytes, secureDataInfo.RsaPublicKeyExponent, secureDataInfo.RsaPublicKeyModulus));

            return Encoding.UTF8.GetString(encryptedSecureData, 0, encryptedSecureData.Length);
        }
        public static string SetFixedLength(string value, int length, bool paddRight = true, char paddingCharacter = ' ', bool removeExcessFromRight = true)
        {
            if (value == null)
            {
                value = "";
            }
            var valueLength = value.Length;
            if (valueLength < length)
            {
                return paddRight ? value.PadRight(length, paddingCharacter) : value.PadLeft(length, paddingCharacter);
            }
            if (valueLength > length)
            {
                return removeExcessFromRight ? value.Substring(0, length) : value.Remove(0, valueLength - length);
            }
            return value;
        }
        public static byte[] BouncyCastleGetCbcMac(string data, byte[] macKey)
        {
            var macBytes = new byte[4];
            var macCipher = new CbcBlockCipherMac(new DesEdeEngine());
            var keyParameters = new DesEdeParameters(macKey);
            //var engine = new DesEdeEngine();
            //engine.Init(true, keyParameters);
            macCipher.Init(keyParameters);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            macCipher.BlockUpdate(dataBytes, 0, data.Length);
            macCipher.DoFinal(macBytes, 0);
            return macBytes;
        }
        public static byte[] BouncyCastleRsaEncrypt(byte[] data, string exponent, string modulus)
        {
            var engine = new RsaEngine();
            var keyParameters = new RsaKeyParameters(false, new BigInteger(HexDecode(modulus)), new BigInteger(HexDecode(exponent)));
            engine.Init(true, keyParameters);
            return engine.ProcessBlock(data, 0, data.Length);
        }
        public static byte[] GenerateRandomBytes(int length)
        {
            var bytes = new byte[length];
            new RNGCryptoServiceProvider().GetBytes(bytes);
            return bytes;
        }
        public static byte[] HexDecode(string hex)
        {
            hex = hex.Replace("-", "");
            byte[] raw = new byte[hex.Length / 2];
            for (int i = 0; i < raw.Length; i++)
            {
                raw[i] = Convert.ToByte(hex.Substring(i * 2, 2), 16);
            }
            return raw;
        }
        //================
        public static string ComputeHMACSHA256(string macData, byte[] macDesBytes)
        {
            var macBytes = Encoding.UTF8.GetBytes(macData);


            using (var hmac = new HMACSHA256(macDesBytes))
            {
                var mac = hmac.ComputeHash(macBytes);

                var hexString = ByteArrayToString(mac);

                return hexString;
            }
        }
        public static string ByteArrayToString(byte[] ba)
        {
            string hex = BitConverter.ToString(ba);
            return hex.Replace("-", "");
        }
        public static PayPhoneSecureData GetPayphoneSecureDataInfo(string msisdn, string paymentMethodTypeCode, string ttid, long amount = -1, string version = "10")
        {
            var payPhoneSecureDataInfo = new PayPhoneSecureData()
            {
                CustomerId = msisdn,
                PrimaryMobileNumber = msisdn,
                PinDesKey = GenerateRandomString(32),
                Identifier = ttid,
                PayPhoneSecureDataVersion = version,
                PayPhoneSecureMacVersion = version,
                Type = paymentMethodTypeCode
            };
            if (amount != -1)
            {
                payPhoneSecureDataInfo.Amount = amount.ToString();
            }
            return payPhoneSecureDataInfo;
        }
        public static string GenerateRandomString(int length,
                                                  string allowedChars =
                                                      "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ123456789")
        {
            if (length < 0)
            {
                return "";
            }
            if (string.IsNullOrEmpty(allowedChars))
            {
                allowedChars = "abcdefghijklmnpqrstuvwxyzABCDEFGHIJKLMNPQRSTUVWXYZ123456789";
            }

            const int byteSize = 256;
            var allowedCharSet = new HashSet<char>(allowedChars).ToArray();
            if (byteSize < allowedCharSet.Length)
            {
                allowedCharSet = new HashSet<char>(allowedChars.Substring(0, byteSize)).ToArray();
            }

            // Guid.NewGuid and System.Random are not particularly random. By using a
            // cryptographically-secure random number generator, the caller is always
            // protected, regardless of use.
            var result = new StringBuilder();
            var buf = new byte[128];
            while (result.Length < length)
            {
                new RNGCryptoServiceProvider().GetBytes(buf);
                for (var i = 0; i < buf.Length && result.Length < length; ++i)
                {
                    // Divide the byte into allowedCharSet-sized groups. If the
                    // random value falls into the last group and the last group is
                    // too small to choose from the entire allowedCharSet, ignore
                    // the value in order to avoid biasing the result.
                    var outOfRangeStart = byteSize - (byteSize % allowedCharSet.Length);
                    if (outOfRangeStart <= buf[i]) continue;
                    result.Append(allowedCharSet[buf[i] % allowedCharSet.Length]);
                }
            }
            return result.ToString();
        }
        public static async Task<Token> GetClientAccessToken(string clientId, string clientSecret, string environment)
        {
            var url = string.Concat(GetTokenEnvironment(environment), "/passport/oauth/token");
            var client = new RestClient(url);
            client.IgnoreResponseStatusCode = true;

            var request = new RestRequest(url, HttpMethod.Post);
            request.AddHeader(Constants.ContentType, "application/x-www-form-urlencoded");
            request.AddHeader(Constants.Authorization, SetAuthorization(clientId, clientSecret));
            request.AddParameter("grant_type", "client_credentials", ParameterType.GetOrPost);
            request.AddParameter("Scope", "profile", ParameterType.GetOrPost);

            JsonDeserializer deserial = new JsonDeserializer();
            ServicePointManager.Expect100Continue = true;
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
            IRestResponse response;
            try
            {
                response = await client.Execute(request);
            }
            catch (Exception exception)
            {
                return new Token { ErrorMessage = exception.Message, ErrorCode = HttpStatusCode.BadRequest.ToString() };
            }
            var httpStatusCode = response.StatusCode;
            var numericStatusCode = (int)httpStatusCode;
            var passportResponse = new Token();
            if (numericStatusCode == (int)HttpStatusCode.OK)
            {
                passportResponse = deserial.Deserialize<Token>(response);

                passportResponse.setAccessToken(passportResponse.access_token);
            }
            else if (response.ContentType == "text/html" || (numericStatusCode == 401 || numericStatusCode == 404 || numericStatusCode == 502 || numericStatusCode == 504))
            {
                passportResponse.ErrorCode = numericStatusCode.ToString();
                passportResponse.ErrorMessage = response.StatusDescription;
            }
            else
            {
                var errorResponse = deserial.Deserialize<ErrorResponse>(response);
                passportResponse.ErrorCode = errorResponse.error.code;
                passportResponse.ErrorMessage = errorResponse.error.message;
            }
            return passportResponse;
        }
        public static string GetEnvironment(string env)
        {
            switch (env)
            {
                case "production":
                    return Uris.liveenvironment;
                case "uat":
                    return Uris.uatenvironment;
                default:
                    return Uris.environment;
            }
        }
        public static string GetTokenEnvironment(string env)
        {
            switch (env)
            {
                case "production":
                    return Uris.passportenvironment;
                case "uat":
                    return Uris.uatenvironment;
                default:
                    return Uris.environment;
            }
        }
        /// <summary>
        /// Send Payment
        /// </summary>
        /// <param name="uri"></param>
        /// <param name="httpMethod"></param>
        /// <param name="data"></param>
        /// <param name="token"></param>
        /// <param name="hashMap"></param>
        /// <param name="signedParameters"></param>
        /// <returns></returns>
        public static async Task<Dictionary<string, string>> Send(string uri, string httpMethod, object data, string environment = "staging", string token = null, List<KeyValuePair<string, string>> hashMap = null, string signedParameters = null, bool isPaycode = false, bool isPaycodeStatus = false, string paycode = "")
        {
            Dictionary<string, string> requestResponse;
            try
            {
                var environmentUrl = GetEnvironment(environment);
                var url = string.Concat(environmentUrl, uri);
                var client = new RestClient(url);
                client.IgnoreResponseStatusCode = true;
                var authConfig = string.IsNullOrEmpty(token)
                    ? new Config(httpMethod, url, ClientId, ClientSecret, MyAccessToken, "", "")
                    : new Config(httpMethod, url, ClientId, ClientSecret, token, "", "");
                RestRequest paymentRequests =null;
                if (httpMethod.Equals("POST"))
                {
                     paymentRequests = new RestRequest(url, HttpMethod.Post);
                }
                if (httpMethod.Equals("GET"))
                {
                    paymentRequests = new RestRequest(url, HttpMethod.Get);
                }
                if (httpMethod.Equals("PUT"))
                {
                    paymentRequests = new RestRequest(url, HttpMethod.Put);
                }
                if (httpMethod.Equals("DELETE"))
                {
                    paymentRequests = new RestRequest(url, HttpMethod.Delete);
                }
                #region -- Add Headers --
                paymentRequests.AddHeader("Signature", authConfig.Signature);
                paymentRequests.AddHeader("Timestamp", authConfig.TimeStamp);
                paymentRequests.AddHeader("Nonce", authConfig.Nonce);
                if (isPaycode)
                {
                    paymentRequests.AddHeader("frontEndPartnerId", FrontEndPartnerId);
                    paymentRequests.AddHeader("Authorization", $"InterswitchAuth {Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId))}");
                    paymentRequests.AddHeader("access_token", authConfig.Authorization.Replace("Bearer ", ""));
                }
                else
                {
                    paymentRequests.AddHeader("Authorization", authConfig.Authorization);
                }
                if (isPaycodeStatus)
                {
                    paymentRequests.AddHeader("paycode", paycode);
                    paymentRequests.AddHeader("access_token", authConfig.Authorization.Replace("Bearer ", ""));
                    paymentRequests.AddHeader("Authorization", $"InterswitchAuth {Convert.ToBase64String(Encoding.UTF8.GetBytes(ClientId))}");
                }
                if (hashMap != null)
                {
                    foreach (var keyValue in hashMap)
                    {
                        paymentRequests.AddHeader(keyValue.Key, keyValue.Value);
                    }
                }
                paymentRequests.AddHeader(Constants.ContentType, "application/json");
                paymentRequests.AddHeader("SignatureMethod", "SHA1");
                #endregion
                if (data != null && (httpMethod.Equals("POST") || httpMethod.Equals("DELETE")))
                    paymentRequests.AddJsonBody(data);

                ServicePointManager.Expect100Continue = true;
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

                var response = await client.Execute(paymentRequests);
                var httpStatusCode = response.StatusCode;
                var numericStatusCode = (int)httpStatusCode;
                if (response.StatusCode == HttpStatusCode.OK || response.StatusCode == HttpStatusCode.Created)
                {
                    requestResponse = new Dictionary<string, string>();
                    requestResponse.Add(Constants.Status, Constants.Successful);
                    requestResponse.Add(Constants.Response, Encoding.UTF8.GetString(response.RawBytes));
                    requestResponse.Add(Constants.Url, uri);
                }
                else
                {
                    requestResponse = new Dictionary<string, string>();
                    requestResponse.Add(Constants.Status, Constants.Failed);
                    requestResponse.Add(Constants.Response, Encoding.UTF8.GetString(response.RawBytes));
                    requestResponse.Add(Constants.Url, uri);
                }
            }
            catch (Exception ex)
            {
                requestResponse = new Dictionary<string, string>();
                requestResponse.Add(Constants.Status, Constants.Error);
                requestResponse.Add(Constants.Response, ex.ToString());
                requestResponse.Add(Constants.Url, uri);
            }
            return requestResponse;
        }
        public static string SetAuthorization(string clientId, string clientSecret)
        {
            try
            {
                String Auth;
                byte[] bytes;
                bytes = Encoding.UTF8.GetBytes(String.Format("{0}:{1}", clientId, clientSecret));
                Auth = Convert.ToBase64String(bytes);
                return String.Concat("Basic ", Auth);
            }
            catch (Exception e)
            {
                throw new Exception("Unable to encode parameters, Please contact connect@interswitchng.com. for assistance.", e);
            }
        }

        public static void Init(string clientId, string clientSecret, string frontEndPartnerId, string environment = "https://sandbox.interswitchng.com")
        {
            ClientId = clientId;
            ClientSecret = clientSecret;
            Environment = environment;
            FrontEndPartnerId = frontEndPartnerId;
            //MyAccessToken = GetToken().Result;
        }
    }

    public class Error1
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class Error2
    {
        public string code { get; set; }
        public string message { get; set; }
    }

    public class ErrorResponse
    {
        public List<Error1> errors { get; set; }
        public Error2 error { get; set; }
        public string transactionRef { get; set; }
    }
}
