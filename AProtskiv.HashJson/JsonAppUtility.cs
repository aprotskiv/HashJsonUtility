using AProtskiv.HashJson.Computing;
using AProtskiv.HashJson.Extensions;
using AProtskiv.HashJson.Reserved;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace AProtskiv.HashJson
{
    public class JsonAppUtility
    {
        private readonly string _salt;
        private readonly int _hashChainLength;
        private readonly string _algorithm;
        private readonly JsonHashUtility _utility;

        public JsonAppUtility(string algorithm, string salt, int hashChainLength)
        {
            _salt = salt;
            _hashChainLength = hashChainLength;
            _algorithm = algorithm;
            _utility = new JsonHashUtility();
        }

        public NamespaceSettings GetReservedNamespaces()
        {
            return _utility.GetReservedNamespaces();
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="inputJson"></param>
        /// <param name="formatting"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException" />
        /// <exception cref="JsonAppException"></exception>
        public HashChainJsonResult Hash_WithHashOfSalt(string inputJson, Formatting formatting, HashFormattingMode mode)
        {
            var algorithm = KeyedHashAlgorithmExtensions.CreateWithSalt(_algorithm, _salt);
            var formatter = new StringModeHashFormatter(mode);
            var provider = new ChainComputeHashProvider(algorithm, _hashChainLength, formatter);

            var outputRoot = _utility.ParseAndHashToken(inputJson, provider);

            var hashChainOfSalt = provider.ComputeHash(_salt ?? String.Empty);

            /// append HashOfSalt
            if (outputRoot.Type == JTokenType.Object)
            {
                (outputRoot as JObject)[APP_ReservedNamespace_Consts.HashChainOfSalt_PROPERTY] = formatter.Format(hashChainOfSalt);
            }

            return new HashChainJsonResult
            {
                HashedDocument = outputRoot.ToString(formatting),
                HashChainOfSalt = hashChainOfSalt
            };
        }

        /// <param name="jsonAppDocument"></param>        
        /// <exception cref="JsonAppException"></exception>
        public static IEnumerable<byte[]> GetHashOfSalt(string jsonAppDocument)
        {
            JToken docRoot;

            try
            {
                docRoot = JRaw.Parse(jsonAppDocument);
            }
            catch
            {
                throw new JsonAppException($"Document must be a valid JSON-APP document");
            }

            JToken hashOfSaltPropertyValue;
            if (docRoot is JObject rootObject)
            {
                hashOfSaltPropertyValue = rootObject[APP_ReservedNamespace_Consts.HashChainOfSalt_PROPERTY];
            }
            else
            {
                throw new JsonAppException($"Document must be a valid JSON-APP document");
            }

            var hashAsString = hashOfSaltPropertyValue?.Value<string>();

            var result = new List<byte[]>();

            if (!string.IsNullOrEmpty(hashAsString))
            {
                try
                {
                    result.Add(ByteArrayEntensions.StringToByteArray(hashAsString));
                }
                catch
                {
                }

                try
                {
                    result.Add(Convert.FromBase64String(hashAsString));
                }
                catch
                {
                }
            }

            if (!result.Any())
            {
                throw new JsonAppException($"Property at '{APP_ReservedNamespace_Consts.HashChainOfSalt_PROPERTY}' must contain a HashChain of Salt in Hexadecimal or Base64 format.");
            }

            return result;
        }
    }
}