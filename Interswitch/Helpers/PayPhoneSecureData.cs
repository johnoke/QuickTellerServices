using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Payment.Helpers
{
    public class PayPhoneSecureData
    {

        
        /// <summary>
        /// The _pay phone secure data header value
        /// </summary>
        private string _payPhoneSecureDataHeaderValue = "4D";

        /// <summary>
        /// The _pay phone secure data version
        /// </summary>
        private string _payPhoneSecureDataVersion = "07";

        /// <summary>
        /// The _rsa public key exponent
        /// </summary>
        private string _rsaPublicKeyExponent = "010001";

        /// <summary>
        /// The _rsa public key modulus
        /// </summary>
        private string _rsaPublicKeyModulus = "009C7B3BA621A26C4B02F48CFC07EF6EE0AED8E12B4BD11C5CC0ABF80D5206BE69E1891E60FC88E2D565E2FABE4D0CF630E318A6C721C3DED718D0C530CDF050387AD0A30A336899BBDA877D0EC7C7C3FFE693988BFAE0FFBAB71B25468C7814924F022CB5FDA36E0D2C30A7161FA1C6FB5FBD7D05ADBEF7E68D48F8B6C5F511827C4B1C5ED15B6F20555AFFC4D0857EF7AB2B5C18BA22BEA5D3A79BD1834BADB5878D8C7A4B19DA20C1F62340B1F7FBF01D2F2E97C9714A9DF376AC0EA58072B2B77AEB7872B54A89667519DE44D0FC73540BEEAEC4CB778A45EEBFBEFE2D817A8A8319B2BC6D9FA714F5289EC7C0DBC43496D71CF2A642CB679B0FC4072FD2CF";

        /// <summary>
        /// The _footer bytes
        /// </summary>
        private string _footerBytes = "5A";
        public string EsbClientId { get; set; }
        public string EsbSharedKey { get; set; }
        /// <summary>
        /// Gets or sets the primary mobile number.
        /// </summary>
        /// <value>The primary mobile number.</value>
        [JsonIgnore]
        public string PrimaryMobileNumber { get; set; }

        /// <summary>
        /// Gets or sets the expiry date.
        /// </summary>
        /// <value>The expiry date.</value>
        [JsonIgnore]
        public string Amount { get; set; }


        private string _pinDesKey;

        /// <summary>
        /// Gets or sets the account number.
        /// </summary>
        /// <value>The account number.</value>
        [JsonIgnore]
        public string PinDesKey
        {
            get
            {
                if (string.IsNullOrWhiteSpace(_pinDesKey))
                    return _pinDesKey;

                if (_pinDesKey.Length == 32)
                {
                    return _pinDesKey;
                }

                return string.Format("{0}{1}", _pinDesKey.Length.ToString(CultureInfo.InvariantCulture).PadLeft(2, '0'), _pinDesKey.PadRight(30, '0'));
            }
            set
            {
                _pinDesKey = value;
            }
        }

        /// <summary>
        /// Gets or sets the pan.
        /// </summary>
        /// <value>The pan.</value>
        [JsonIgnore]
        public string Pan { get; set; }

        private string _identifier = "";
        /// <summary>
        /// Gets or sets the identifier.
        /// </summary>
        /// <value>The identifier.</value>
        [JsonIgnore]
        public string Identifier
        {
            get { return _identifier; }
            set { _identifier = value; }
        }

        //[JsonIgnore]
        //public int TimeOut { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="PayPhoneSecureData"/> class.
        /// </summary>
        public PayPhoneSecureData()
        {
            Status = "active";
            PinDesKey = "";
            Pan = "";
            Amount = "";
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        /// <value>The type.</value>
        [JsonProperty(PropertyName = "paymentMethodTypeCode")]
        public string Type { get; set; }
        /// <summary>
        /// Gets the status.
        /// </summary>
        /// <value>The status.</value>
        [JsonProperty(PropertyName = "status")]
        public string Status { get; private set; }

        /// <summary>
        /// Gets or sets the secure data.
        /// </summary>
        /// <value>The secure data.</value>
        [JsonProperty(PropertyName = "secure")]
        public string SecureData { get; set; }

        /// <summary>
        /// Gets or sets the pay phone secure data header value.
        /// </summary>
        /// <value>The pay phone secure data header value.</value>
        [JsonIgnore]
        public string PayPhoneSecureDataHeaderValue
        {
            get { return _payPhoneSecureDataHeaderValue; }
            //set { _payPhoneSecureDataHeaderValue = value; }
        }

        /// <summary>
        /// Gets or sets the pay phone secure data version.
        /// </summary>
        /// <value>The pay phone secure data version.</value>
        [JsonIgnore]
        public string PayPhoneSecureDataVersion
        {
            get { return _payPhoneSecureDataVersion; }
            set { _payPhoneSecureDataVersion = value; }
        }


        private string _payPhoneSecureMacVersion = "05";

        /// <summary>
        /// Gets or sets the pay phone secure data version.
        /// </summary>
        /// <value>The pay phone secure data version.</value>
        [JsonIgnore]
        public string PayPhoneSecureMacVersion
        {
            get { return _payPhoneSecureMacVersion; }
            set { _payPhoneSecureMacVersion = value; }
        }

        /// <summary>
        /// Gets or sets the RSA public key exponent.
        /// </summary>
        /// <value>The RSA public key exponent.</value>
        [JsonIgnore]
        public string RsaPublicKeyExponent
        {
            get { return _rsaPublicKeyExponent; }
            set { _rsaPublicKeyExponent = value; }
        }

        public string CustomerId { get; set; }
        /// <summary>
        /// Gets or sets the RSA public key modulus.
        /// </summary>
        /// <value>The RSA public key modulus.</value>
        [JsonIgnore]
        public string RsaPublicKeyModulus
        {
            get { return _rsaPublicKeyModulus; }
            set { _rsaPublicKeyModulus = value; }
        }

        /// <summary>
        /// Gets or sets the footer bytes.
        /// </summary>
        /// <value>The footer bytes.</value>
        [JsonIgnore]
        public string FooterBytes
        {
            get { return _footerBytes; }
            //set { _footerBytes = value; }
        }

        //[JsonIgnore]
        //public string Url { get; set; }
    }
}