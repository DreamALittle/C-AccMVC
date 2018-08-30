using ICSharpCode.SharpZipLib.Zip;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Acctrue.CMC.CodeBuild
{
    //工具类
    public class Tools
    {
        /// <summary>
        /// AES数据加密
        /// </summary>
        /// <param name="encryptString">待加密字符串</param>
        /// <param name="encryptKey">加密密钥（128位密钥的Base64编码形式）</param>
        /// <returns></returns>
        public static string AESEncode(string encryptString, string encryptKey)
        {
            if (string.IsNullOrEmpty(encryptString)) return null;
            Byte[] toEncryptArray = Encoding.UTF8.GetBytes(encryptString);
            //RijndaelManaged rm = new RijndaelManaged
            //{
            //    Key = System.Text.Encoding.UTF8.GetBytes(encryptKey),
            //    Mode = CipherMode.ECB,
            //    Padding = PaddingMode.PKCS7
            //};
            RijndaelManaged rm = new RijndaelManaged();
            rm.Key = Convert.FromBase64String(encryptKey);
            rm.Mode = CipherMode.ECB;
            rm.Padding = PaddingMode.PKCS7;



            ICryptoTransform cTransform = rm.CreateEncryptor();
            Byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);
            return ToHexString(resultArray);
        }
        /// <summary>
        /// HEX编码
        /// </summary>
        /// <param name="bytes">待编码数据</param>
        /// <returns></returns>
        public static string ToHexString(byte[] bytes)
        {
            string byteStr = string.Empty;
            if (bytes != null || bytes.Length > 0)
            {
                foreach (var item in bytes)
                {
                    byteStr += string.Format("{0:X2}", item);
                }
            }
            return byteStr;
        }
        /// <summary>
        /// 解压压缩数据中的第一个压缩内容
        /// </summary>
        /// <param name="zipBytes">压缩数据</param>
        /// <returns>第一个压缩内容文本</returns>
        public static string UnZipGetFirstText(byte[] zipBytes)
        {
            string resultStr = string.Empty;
            using(MemoryStream ms = new MemoryStream(zipBytes))
            {
                UnZipGetFirstText(ms, out resultStr);
            }
            return resultStr;

        }
        /// <summary>   
        /// 解压功能(解压压缩文件到指定目录)   
        /// </summary>   
        /// <param name="fileToUnZip">待解压的文件</param>   
        /// <param name="zipedFolder">指定解压目标目录</param>   
        /// <param name="password">密码</param>   
        /// <returns>解压结果</returns>   
        public static bool UnZipGetFirstText(Stream inputZipStream, out string content)
        {
            bool result = true;
            content = string.Empty;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;

            try
            {
                inputZipStream.Seek(0, SeekOrigin.Begin);
                zipStream = new ZipInputStream(inputZipStream);
                if ((ent = zipStream.GetNextEntry()) != null)
                {

                    using (StreamReader sr = new StreamReader(zipStream))
                    {
                        content = sr.ReadToEnd();
                    }
                    //int size = 2048;
                    //byte[] data = new byte[size];
                    //while (true)
                    //{
                    //    size = zipStream.Read(data, 0, data.Length);
                    //    if (size > 0)
                    //        fs.Write(data, 0, data.Length);
                    //    else
                    //        break;
                    //}
                }
            }
            catch 
            {
                result = false;
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }
        /// <summary>
        /// 将码信息压缩序列化
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static string SerializeZipCode(List<string> codes)
        {
            string allCodeStr = string.Join("\r\n", codes);
            return SerializeZipCode(allCodeStr);
        }
        /// <summary>
        /// 将码信息压缩序列化
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        public static string SerializeZipCode(string codesStr)
        {
            byte[] allCodeBytes = System.Text.Encoding.UTF8.GetBytes(codesStr);
            byte[] zipBytes = Zip(allCodeBytes);

            return Convert.ToBase64String(zipBytes);
        }
        /// <summary>
        /// 压缩
        /// </summary>
        /// <param name="sourceBytes">待压缩数据</param>
        /// <returns></returns>
        public static byte[] Zip(byte[] sourceBytes)
        {
            using (System.IO.MemoryStream ms = new System.IO.MemoryStream())
            {
                using (ICSharpCode.SharpZipLib.Zip.ZipOutputStream zs = new ICSharpCode.SharpZipLib.Zip.ZipOutputStream(ms))
                {
                    ICSharpCode.SharpZipLib.Zip.ZipEntry entry = new ICSharpCode.SharpZipLib.Zip.ZipEntry("Code") { DateTime = DateTime.Now };
                    zs.PutNextEntry(entry);
                    zs.Write(sourceBytes, 0, sourceBytes.Length);
                    zs.Flush();
                }
                return ms.ToArray();
            }
        }

        /// <summary>
        /// 将压缩序列化的信息还原为码信息
        /// </summary>
        /// <param name="codes"></param>
        /// <returns></returns>
        /// 
        public static List<string> DeserializeZipCode(string serializeCodeStr)
        {
            List<string> codes = new List<string>();
            byte[] codeBytes = Convert.FromBase64String(serializeCodeStr);
            byte[] strBytes = UnZip(codeBytes);
            codes = System.Text.Encoding.UTF8.GetString(strBytes).Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries).ToList();
            
            return codes;
        }
        /// <summary>
        /// 解压缩
        /// </summary>
        /// <param name="zipBytes">待解压数据</param>
        /// <returns></returns>
        public static byte[] UnZip(byte[] zipBytes)
        {
            ICSharpCode.SharpZipLib.Zip.ZipInputStream zipStream = null;
            ICSharpCode.SharpZipLib.Zip.ZipEntry ent = null;
            byte[] reslutBytes = null;
            try
            {
                using (System.IO.MemoryStream inputZipStream = new System.IO.MemoryStream(zipBytes))
                {
                    zipStream = new ICSharpCode.SharpZipLib.Zip.ZipInputStream(inputZipStream);
                    if ((ent = zipStream.GetNextEntry()) != null)
                    {
                        reslutBytes = new byte[zipStream.Length];
                        zipStream.Read(reslutBytes, 0, reslutBytes.Length);
                    }
                }
            }
            finally
            {
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return reslutBytes;
        }
        /// <summary>
        /// 压缩文件
        /// </summary>
        /// <param name="codeFileName">待压缩文件路径</param>
        /// <param name="zipFileName">压缩后文件路径</param>
        public static void ZipFile(string codeFileName, string zipFileName)
        {
            if (string.IsNullOrEmpty(codeFileName))
            {
                throw new ArgumentNullException("codeFileName");
            }
            using (ZipOutputStream stream = new ZipOutputStream(File.Create(zipFileName)))
            {
                using (FileStream stream2 = File.OpenRead(codeFileName))
                {
                    ZipEntry entry = new ZipEntry(Path.GetFileName("Code.txt"))
                    {
                        DateTime = DateTime.Now,
                        Size = stream2.Length
                    };
                    stream.PutNextEntry(entry);
                    int count = 0;
                    int num2 = 10000;
                    byte[] buffer = new byte[num2];
                    count = stream2.Read(buffer, 0, buffer.Length);
                    int num3 = 0;
                    while (true)
                    {
                        if (count > 0)
                        {
                            stream.Write(buffer, 0, count);
                            count = stream2.Read(buffer, 0, buffer.Length);
                        }
                        else
                        {
                            break;
                        }
                        num3++;
                    }
                    stream2.Close();
                    stream.Close();
                }
            }

        }
    }
}
