using Acctrue.CMC.Model.Request;
using Acctrue.CMC.Factory.Systems;
using Acctrue.CMC.Model.Systems;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using Acctrue.CMC.Model.Response;
using System.Text;
using System.Net.Http;
using Acctrue.CMC.Interface.Common;
using Acctrue.CMC.Web.Controllers.WebApi;

namespace Acctrue.CMC.Web.Controllers.APIControllers
{
    [RoutePrefix("WebApi/AppSettings")]
    public class AppSettingsController : ApiController,IBaseInterface
    {
        [Route("GetToken")]
        [HttpPost]
        public GetTokenResponse GetToken(GetTokenRequest request)
        {
            GetTokenResponse response = new GetTokenResponse();
            response.IsTrue = false;
            response.Status = 0;
            if (request != null)
            {
                try
                {
                    if (string.IsNullOrEmpty(request.AppId) || string.IsNullOrEmpty(request.Secret) || string.IsNullOrEmpty(request.Session_Id))
                    {
                        response.Status = 0;
                        response.Msg = "请求参数有误，请检查!";
                    }
                    else
                    {
                        //获取三方APP信息
                        AppSettingInfo app = SystemFactory.Instance.GetSystemAppConfig(request.AppId, request.Secret);
                        if (app != null)
                        {
                            if (app.AppStatus == AppStatus.Reviewed)
                            {
                                //更新Token,同时更新过期时间)
                                string newToken = SystemFactory.Instance.UpdateTokenInfo(app);

                                response.IsTrue = true;
                                response.Access_Token = newToken;
                                response.Status = 1;
                                response.Msg = "请求成功！";
                                response.Expires_In = 7200;
                                response.CorpCode = app.CorpCode;
                                response.SubCorpCode = app.SubCorpCode;
                                response.CorpName = app.CorpName;
                            }
                            else
                            {
                                response.Msg = "请联系管理员审批系统!";
                            }
                        }
                        else
                        {
                            response.Msg = "该APP未在码中心注册，或密钥有误!请重试或联系管理员";
                        }
                    }
                }
                catch (Exception exception)
                {
                    response.Msg = exception.Message;
                }
            }
            else
            {
                response.Msg = "非法请求，请重试!";
            }

            return response;
        }

        [Route("GetApplications")]
        [HttpPost]
        [CMC(InterfaceName = "获取应用程序列表", Open = true)]
        public GetApplicationResponse GetApplications(GetApplicationRequest request)
        {
            GetApplicationResponse response = new GetApplicationResponse();
            response.IsTrue = false;
            response.Status = 0;

            if (request != null)
            {
                try
                {
                    //验证Token
                    string tokenMsg = "";
                    AppSettingInfo app = ApiHelper.VerifyTokenAndGetApp(request.Token, out tokenMsg);
                    if (app != null)
                    {
                        //AES验证签名
                        if (ApiHelper.AESEncrypt(request.Token, request.TimeStamp, app.Seed) != request.Sign)
                        {
                            response.Msg = "签名验证不通过！";
                            return response;
                        }
                        //验证API访问权限
                        if (ApiHelper.InterfaceRightCheck(app.AppSettingID))
                        {
                            response.Msg = "无访问接口权限，请联系管理员";
                            return response;
                        }
                        List<AppSettingInfoSimple> appList = SystemFactory.Instance.GetAllAppConfig();

                        response.IsTrue = true;
                        response.Status = 1;
                        response.Msg = "请求成功";
                        response.Data = appList;

                    }
                    else
                    {
                        response.Msg = tokenMsg;
                    }
                }
                catch (Exception ex)
                {
                    response.Msg = ex.Message;
                }
            }
            else
            {
                response.Msg = "非法请求，请重试！";
            }

            return response;
        }
    }
}
