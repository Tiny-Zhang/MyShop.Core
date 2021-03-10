using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using log4net;

namespace MyShop.Common
{
    public class Encryption
    {
        private static readonly ILog log = LogManager.GetLogger(typeof(Encryption));

        /// <summary>
        /// 生成签名  大写
        /// 秘钥+token+随机数+时间戳+参数
        /// </summary>
        /// <param name="apiSignSecret">秘钥</param>
        /// <param name="nonce">随机数</param>
        /// <param name="timestamp">时间戳</param>
        /// <param name="args">参数</param>
        /// <returns></returns>
        public static string SignCreate(string apiSignSecret, string nonce, string timestamp, string args)
        {
            string rule = string.Format("{0}{1}{2}{0}{3}", apiSignSecret, nonce, timestamp, args);
            return MD5(rule).ToUpper();
        }

        /// <summary>
        /// MD5加密
        /// </summary>
        /// <param name="str">原始字符串</param>
        /// <returns>MD5结果</returns>
        public static string MD5(string str)
        {
            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] data = Encoding.UTF8.GetBytes(str);
            byte[] result = md5.ComputeHash(data);
            String ret = "";
            for (int i = 0; i < result.Length; i++)
                ret += result[i].ToString("x").PadLeft(2, '0');
            return ret;
        }

        /// <summary>
        /// 参数拼接
        /// </summary>
        /// <param name="parames"></param>
        /// <returns></returns>
        public static Tuple<string, string> GetQueryString(SortedDictionary<string, string> parames)
        {
            // 第一步：把字典按Key的字母顺序排序
            //IDictionary<string, string> sortedParams = new SortedDictionary<string, string>(parames);
            IEnumerator<KeyValuePair<string, string>> dem = parames.GetEnumerator();

            // 第二步：把所有参数名和参数值串在一起
            StringBuilder query = new StringBuilder("");  //签名字符串
            StringBuilder queryStr = new StringBuilder(""); //url参数
            if (parames == null || parames.Count == 0)
                return new Tuple<string, string>("", "");

            while (dem.MoveNext())
            {
                string key = dem.Current.Key;
                string value = dem.Current.Value;
                if (!string.IsNullOrEmpty(key) && !string.IsNullOrEmpty(value))
                {
                    query.Append(key).Append(value);
                    queryStr.Append("&").Append(key).Append("=").Append(value);
                }
            }
            return new Tuple<string, string>(query.ToString(), queryStr.ToString().Substring(1, queryStr.Length - 1));
        }



        /// <summary>
        /// 生成秘钥Key  16位/32位
        /// </summary>
        /// <param name="length">16位/32位</param>
        /// <returns></returns>
        public static string GetEncrytionKey(int length)
        {
            //定义一个字符串（A-Z，a-z，0-9）即62位,外加 %$#，共65位
            string str = "zxcvbnmlkjhgfdsaqwertyuiopQWERTYUIOPASDFGHJKLZXCVBNM1234567890%$#";
            //由Random生成随机数
            Random random = new Random();
            StringBuilder sb = new StringBuilder();
            //长度为几就循环几次
            for (int i = 0; i < length; ++i)
            {
                //产生0-65的数字
                int number = random.Next(65);
                //将产生的数字通过length次承载到sb中
                sb.Append(str[number]);
            }
            //将承载的字符转换成字符串
            return sb.ToString();
        }

        #region AES

        /// <summary>
        /// AES加密
        /// </summary>
        /// <param name="str"></param>
        /// <param name="key">16/32位</param>
        /// <returns>f527e75a9cf89ecb23a32fa3e8b2c650</returns>
        public static string AesEncrypt(string str, string key)
        {
            try
            {
                if (string.IsNullOrEmpty(str)) return null;
                Byte[] toEncryptArray = Encoding.UTF8.GetBytes(str);

                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateEncryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return BitConverter.ToString(resultArray).Replace("-", "").ToLower();
            }
            catch (Exception ex)
            {
                log.Fatal($"加密:{str}异常:{ex.Message}");
                return "";
            }

        }

        /// <summary>
        /// AES解密
        /// </summary>
        /// <param name="str">f527e75a9cf89ecb23a32fa3e8b2c650</param>
        /// <param name="key">16位</param>
        /// <param name="function">调用源方法名</param>
        /// <returns></returns>          
        public static string AesDecrypt(string str, string key, string function = "")
        {
            try
            {
                if (string.IsNullOrEmpty(str))
                {
                    return null;
                }
                List<string> list = new List<string>();
                for (int i = 0; i < str.Length; i = i + 2)
                {
                    list.Add(str.Substring(i, 2));
                }
                string a4 = (string.Join("-", list.ToArray()));
                String[] arr2 = a4.ToUpper().Split('-');
                byte[] toEncryptArray = new byte[arr2.Length];
                for (int i = 0; i < arr2.Length; i++)
                    toEncryptArray[i] = Convert.ToByte(arr2[i], 16);

                RijndaelManaged rm = new RijndaelManaged
                {
                    Key = Encoding.UTF8.GetBytes(key),
                    Mode = CipherMode.ECB,
                    Padding = PaddingMode.PKCS7
                };

                ICryptoTransform cTransform = rm.CreateDecryptor();
                Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch (Exception ex)
            {
                log.Fatal($"解密:{str}异常:{ex.Message}");
                return "";
            }
        }

        #endregion




    }
}
