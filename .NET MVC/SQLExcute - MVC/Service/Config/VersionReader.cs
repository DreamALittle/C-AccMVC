using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Acctrue.CMC.Configuration;

namespace Acctrue.CMC.Web.WebDBInstall.Config
{
    public class VersionReader
    {
        const string VERSION_TAG = "applicationVersion";

        public static readonly Version Zero = new Version(0, 0, 0, 1);

        /// <summary>
        /// 应用程序版本
        /// </summary>
        /// <returns></returns>
        public static Version AppVersion()
        {
            try
            {
                var version = ConfigReader.Read(VERSION_TAG);
                if (string.IsNullOrEmpty(version)) return new Version();
                var regex = new System.Text.RegularExpressions.Regex(@"^(\d+\.)*\d+$");
                if (!regex.IsMatch(version.Trim())) return new Version();
                return new Version(version.Trim());
            }
            catch (Exception e)
            {
                LogTracer.WriteInfo("AppVersionReader", e.ToString());
            }

            return new Version();
        }

        /// <summary>
        /// 当前数据库版本
        /// </summary>
        /// <returns></returns>
        public static string DatabaseVersion(DBObject db)
        {
            try
            {
                using (DBHelper helper = new DBHelper())
                {
                    string commandText = "select configValue from configs where configkey='DBVersion'";
                    var result = helper.ExecuteScalar(db, commandText);
                    if (result == null || result == DBNull.Value) return "1.00";
                    return result.ToString();
                }
            }
            catch (System.Net.NetworkInformation.NetworkInformationException)
            {
                throw;
            }
            catch (System.Net.NetworkInformation.PingException)
            {
                throw;
            }
            catch (System.Net.Sockets.SocketException)
            {
                throw;
            }
            catch (System.Net.WebException)
            {
                throw;
            }
            catch (Exception ex)
            {
                return "1.00";
            }
        }

        static Version ParseVersion(string version)
        {
            var regex = new System.Text.RegularExpressions.Regex(@"^(\d+\.)*\d+$");
            if (!regex.IsMatch(version.Trim())) return new Version();
            return new Version(version.Trim());
        }
    }
}