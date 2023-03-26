﻿using AProtskiv.HashJson.Reserved;
using AProtskiv.JsonUtilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace AProtskiv.HashJson
{
    public class JsonHashUtility
    {
        private readonly byte[] _key;

        public JsonHashUtility(string key)
        {
            _key = Encoding.UTF8.GetBytes(key);
        }

        public string HashMD5(string inputJson, Formatting formatting)
        {
            return Hash(inputJson, new HMACMD5(_key), formatting);
        }

        public string HashSHA256(string inputJson, Formatting formatting)
        {
            return Hash(inputJson, new HMACSHA256(_key), formatting);
        }

        public string HashSHA512(string inputJson, Formatting formatting)
        {
            return Hash(inputJson, new HMACSHA512(_key), formatting);
        }

        static ReservedNamespace Build_XLS_namespace()
        {
            var nsXLS = new ReservedNamespace("xls");
            nsXLS.ReservedFunctions.AddRange(new[] {
                    new ReservedFunction(nsXLS, "compare", new ReservedFunction.ArgumentsToIgnore{
                        AtJArrayIndecies = new int[]{  1 } ,
                        InJObjectProperties = new string[]{ "operator" /*, "left", "right" */ }
                    }),
                    // ATTENTION: all functions that have arguments in properties must be defined !!!
                });
            return nsXLS;
        }

        static IReservedNamespaceDictionary GetReservedNamespaces()
        {
            var result = new ReservedNamespaceDictionary();

            // XLS
            {
                var nsXLS = Build_XLS_namespace();
                result.Add(nsXLS.Namespace, nsXLS);
            }

            // APP
            {
                var nsAPP = new ReservedNamespace("app");
                result.Add(nsAPP.Namespace, nsAPP);
            }

            return result;            
        }

        public string Hash(string inputJson, HashAlgorithm hashAlgorithm, Formatting formatting)
        {
            JToken inputRoot = JRaw.Parse(inputJson);

            //collect aliases
            Dictionary<string, string> dictAliases = new Dictionary<string, string>();

            var outputRoot = HashRecursively(
                    inputRootToken: inputRoot,
                    inputContextToken: inputRoot,
                    skipHashOnLevel: false,
                    hashAlgorithm,
                    dictAliases,
                    reservedNamespaces: GetReservedNamespaces(),
                    reservedFunctionContext: null);

            return outputRoot.ToString(formatting);
        }

        private static JToken HashRecursively(JToken inputRootToken, JToken inputContextToken, bool skipHashOnLevel, HashAlgorithm hashAlgorithm,
            Dictionary<string, string> dictAliases,
            IReservedNamespaceDictionary reservedNamespaces,
            ReservedFunction reservedFunctionContext,
            int? arrayItemIndex = null)
        {
            JToken output;
            bool skipHashOnChildLevel = false;
            ReservedFunction reservedFunctionToChild = null;

            switch (inputContextToken.Type)
            {
                case JTokenType.Null:
                    output = JValue.CreateNull();
                    break;
                case JTokenType.Constructor:
                case JTokenType.Object:
                    output = new JObject();
                    //skipHashOnChildLevel = skipHashOnLevel; // skip hashing JObject properties name.
                                                            // Values may or may NOT hashed !!!
                    reservedFunctionToChild = reservedFunctionContext;
                    break;
                case JTokenType.Array:
                    output = new JArray();
                    reservedFunctionToChild = reservedFunctionContext;
                    break;
                case JTokenType.Property:
                    var originalProperty = (JProperty)inputContextToken;
                    var originalPropertyName = originalProperty.Name;
                    string newPropertyName;

                    if (reservedFunctionContext != null)
                    {
                        newPropertyName = originalPropertyName;
                        skipHashOnChildLevel = IsReservedFunctionArgument(propertyName: originalPropertyName, function: reservedFunctionContext);
                    }
                    else
                    {
                        newPropertyName = TryHashProperty(hashAlgorithm, reservedNamespaces, originalPropertyName, out reservedFunctionToChild);
                        skipHashOnChildLevel = newPropertyName.Equals(originalPropertyName); //skip when entire property was NOT hashed  ( "app:ALIASES" or "XLS:SUM" )
                    }

                    output = new JProperty(newPropertyName, content: null);
                    break;
                // Values represented as STRING 
                case JTokenType.Uri:
                case JTokenType.TimeSpan:
                case JTokenType.Date:
                case JTokenType.Guid:
                case JTokenType.String:
                    {
                        if (skipHashOnLevel ||
                            (reservedFunctionContext != null 
                                && inputContextToken.Parent is JArray 
                                && arrayItemIndex.HasValue 
                                && reservedFunctionContext.Arguments.AtJArrayIndecies.Contains(arrayItemIndex.Value))
                        )
                        {
                            output = inputContextToken.DeepClone();
                        }
                        else
                        {
                            output = HashStringToken(inputContextToken, hashAlgorithm, reservedNamespaces);
                        }
                    }
                    break;
                // Values represented as NON-STRING 
                case JTokenType.Integer:
                case JTokenType.Float:
                case JTokenType.Boolean:
                case JTokenType.Bytes:
                    output = inputContextToken.DeepClone();
                    break;
                case JTokenType.Comment:
                default:
                    throw new NotImplementedException($"Can't hash token of type '{inputContextToken.Type}'");
            }

            if (inputContextToken.HasValues && inputContextToken.Type != JTokenType.Bytes)
            {
                int? arrayChildIndex = inputContextToken.Type == JTokenType.Array ? 0 : default(int?);

                foreach (var childToken in inputContextToken.Children())
                {
                    var outputChild = HashRecursively(
                            inputRootToken: inputRootToken,
                            inputContextToken: childToken,
                            skipHashOnLevel: skipHashOnChildLevel,
                            hashAlgorithm,
                            dictAliases: dictAliases,
                            reservedNamespaces: reservedNamespaces,
                            reservedFunctionToChild,
                            arrayItemIndex: arrayChildIndex
                        );

                    if (arrayChildIndex.HasValue) arrayChildIndex = arrayChildIndex.Value + 1; // increase child item arrayIndex

                    if (output is JProperty jProperty)
                    {
                        jProperty.Value = outputChild;
                    }
                    else if (output is JContainer jContainer)
                    {
                        jContainer.Add(outputChild);
                    }
                    else
                    {
                        throw new NotImplementedException($"Cannot add {outputChild} to {output.Type} token type");
                    }
                }
            }

            return output;
        }

        private static bool IsReservedFunctionArgument(string propertyName, ReservedFunction function)
        {
            return (function != null) && function.Arguments.InJObjectProperties.Contains(propertyName); // without IgnoreCase !!!
        }

        private static string TryHashProperty(HashAlgorithm hashAlgorithm, IReservedNamespaceDictionary reservedNamespaces,
            string originalPropertyName,
            out ReservedFunction reservedFunction)
        {
            string newPropertyName;
            if (HasReservedPrefix(originalPropertyName, reservedNamespaces,
                    reservedPrefix: out string reservedPrefix,
                    level2Suffix: out string level2Suffix,
                    reservedFunction: out reservedFunction)
                )
            {
                if (!String.IsNullOrWhiteSpace(level2Suffix))
                {
                    // "app:functions:GetAverage" -> "app:functions:FD137F90B619DCF6951EF9649F1CB231"
                    newPropertyName = reservedPrefix + hashAlgorithm.ComputeHashAsString(level2Suffix);
                }
                else
                {
                    // no Level2 namespaces
                    // "app:ALIASES" or "XLS:SUM"
                    newPropertyName = originalPropertyName;
                }
            }
            else
            {
                newPropertyName = hashAlgorithm.ComputeHashAsString(originalPropertyName);
            }

            return newPropertyName;
        }

        private static JToken HashStringToken(JToken inputContextToken, HashAlgorithm hashAlgorithm,
            IReservedNamespaceDictionary reservedNamespaces)
        {
            JToken output;

            var originalValue = inputContextToken.Value<string>();
            String newValue = null;

            if (inputContextToken.Type == JTokenType.String)
            {
                if (newValue == null)
                {
                    // check if it's a PostgresSQL JsonPath  "{alias1,path1,path2,1}"
                    var jsonPath = JsonPath4Select.FromPostgresJsonPath(originalValue);

                    if (jsonPath.IsNotValidNewtonsoftJsonPath)
                    {
                        // not valid -> check if it's a NewtonsoftJson JsonPath  "$.['alias1'].['path1'].['path2'][2]" or $['store']['book'][0]['title']
                        jsonPath = JsonPath4Select.FromWellFormedNewtonsoftJsonPath(originalValue);
                    }

                    if (!jsonPath.IsNotValidNewtonsoftJsonPath)
                    {
                        // found valid JsonPath
                        var hashedPath = HashJsonPath(hashAlgorithm, jsonPath, reservedNamespaces);
                        // make wellformed NewtonsoftJsonPath
                        newValue = "$." + hashedPath.ToNewtonsoftJsonPath(); // "$.['a1'] " or "$.[1].['a1']"
                    }
                }
            }

            if (newValue == null) // no value - make hash from original Value
            {
                newValue = hashAlgorithm.ComputeHashAsString(originalValue);
            }

            output = new JValue(newValue);

            return output;
        }

        public static JsonPath4Select HashJsonPath(HashAlgorithm algorithm, IJsonPath4Select jsonPath,
            IReservedNamespaceDictionary reservedNamespaces)
        {
            return new JsonPath4Select(jsonPath.Segments.Select(
                    x => x is String xString
                        ? TryHashProperty(algorithm, reservedNamespaces, originalPropertyName: xString,
                                out _ /* reserved function in JsonPath are not used for further processing */
                            )
                        : x
                ).ToArray()
            );
        }

        /// <summary>
        /// Checks whether property starts with reserved prefix
        /// </summary>
        /// <param name="newPropertyName">property name</param>
        /// <param name="reservedNamespaces">reserved namespaces</param>
        /// <param name="reservedPrefix">if NOT null - then ends with ':'</param>
        /// <param name="level2Suffix">second level namespace suffix. If NOT null - then does NOT start with ':'</param>        
        private static bool HasReservedPrefix(string newPropertyName, IReservedNamespaceDictionary reservedNamespaces,
            out string reservedPrefix,
            out string level2Suffix,
            out ReservedFunction reservedFunction)
        {
            var result = false;
            reservedPrefix = null;
            level2Suffix = null;
            reservedFunction = null;

            foreach (ReservedNamespace rootNamespace in reservedNamespaces.Values)
            {
                var namespace1Level = rootNamespace.Namespace + reservedNamespaces.Separator;
                if (newPropertyName.StartsWith(namespace1Level, StringComparison.InvariantCultureIgnoreCase))
                {
                    reservedPrefix = namespace1Level;
                    var level1Suffix = newPropertyName.Substring(namespace1Level.Length);

                    var namespace2LevelEndIndex = level1Suffix.IndexOf(reservedNamespaces.Separator);

                    if (namespace2LevelEndIndex > 0)
                    {
                        // user defined function or variable ???
                        var namespace2Level = level1Suffix.Substring(0, namespace2LevelEndIndex);
                        reservedPrefix = reservedPrefix + namespace2Level + reservedNamespaces.Separator;

                        if (namespace2LevelEndIndex != level1Suffix.Length - 1) //if separator is NOT last character
                        {
                            level2Suffix = level1Suffix.Substring(namespace2LevelEndIndex + 1);
                        }
                        else
                        {
                            //separator is last character
                            level2Suffix = String.Empty;
                        }
                    }
                    else
                    {
                        // it's predefined function from namespace
                        reservedFunction = rootNamespace.ReservedFunctions.FirstOrDefault(x => x.Function.Equals(level1Suffix, StringComparison.InvariantCultureIgnoreCase));
                    }

                    result = true;
                    break;
                }
            }

            return result;
        }
    }
}