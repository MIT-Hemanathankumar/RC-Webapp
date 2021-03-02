using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using PDM.API.Model;
using PDM.Helper;
using PDM.Model;
using PDM.PharmatiseAPI.Contracts.v1;
using PDM.PharmatiseAPI.Options;
using PDM.Services;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PDM.PharmatiseAPI.Controllers.v1
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class DeliveryController : Controller
    {
        private readonly IUserService userService;
        private readonly IDeliveryService deliveryService;
        private readonly IMasterService masterService;
        private readonly JwtSettings jwtSettings;
        private readonly UserManager<IdentityUser> userManager;
        private readonly IConfiguration iConfiguration;
        private readonly ILogger<DeliveryController> logger;
        private readonly IFCMService FCMService;
        public DeliveryController(JwtSettings jwtSettings, UserManager<IdentityUser> userManager, IUserService userService, IDeliveryService deliveryService, IMasterService _masterService,
            IFCMService _FCMService, IConfiguration _iConfiguration, ILogger<DeliveryController> _logger)
        {
            this.jwtSettings = jwtSettings;
            this.userManager = userManager;
            this.userService = userService;
            this.masterService = _masterService;
            this.deliveryService = deliveryService;
            this.iConfiguration = _iConfiguration;
            this.FCMService = _FCMService;
            this.logger = _logger;
        }

        [AllowAnonymous]
        [HttpPost(template: ApiRoutes.Delivery.Login)]
        public async Task<AuthenticationResult> LoginAsync(string username, string password)
        {
            var user = await userManager.FindByNameAsync(username);
            if (user == null)
            {
                return new AuthenticationResult
                {
                    Message = "User does not exist",
                    Status = false
                };
            }
            var userHasValidPwd = await userManager.CheckPasswordAsync(user, password);
            if (!userHasValidPwd)
            {
                return new AuthenticationResult
                {
                    Message = "Login failed, check username / password",
                    Status = false
                };
            }
            return GenerateUserAuthenticationResult(user);
        }
        private AuthenticationResult GenerateUserAuthenticationResult(IdentityUser user)
        {
            var tokenhandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(jwtSettings.Secret);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims: new[]
                {
                    new Claim(type: JwtRegisteredClaimNames.Sub, value: user.Email),
                    new Claim(type: JwtRegisteredClaimNames.Jti, value: Guid.NewGuid().ToString()),
                    new Claim(type: JwtRegisteredClaimNames.Email, value: user.Email),
                    new Claim(type: "id", value: user.Id)
                }),
                Expires = DateTime.Now.AddHours(jwtSettings.Expires),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), algorithm: SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenhandler.CreateToken(tokenDescriptor);
            var dbUser = userService.GetUser(user.Id);
            var userRole = Converters.ConvertString(userManager.GetRolesAsync(user).Result.First());
            if (userRole == "Driver")
            {
                return new AuthenticationResult
                {
                    Status = true,
                    Message = "Login successfully",
                    Token = tokenhandler.WriteToken(token),
                    Email = dbUser.Address.Contacts.Where(w => w.ContactTypeId == (int)PDM.Model.ContactTypes.Email).FirstOrDefault().Value ?? "",
                    Mobile = dbUser.Address.Contacts.Where(w => w.ContactTypeId == (int)PDM.Model.ContactTypes.Mobile).FirstOrDefault().Value ?? "",
                    Name = string.Concat(dbUser.FirstName, " ", dbUser.LastName),
                    UserId = (int)dbUser.UserId,
                    UserType = userRole
                };
            }
            else
            {
                return new AuthenticationResult
                {
                    Status = false,
                    Message = "User is not authorized to access this APP"
                };
            }
        }

        [HttpGet(ApiRoutes.Delivery.delivery)]
        public DeliverylistResult GetDelivery()
        {
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);
            return new DeliverylistResult
            {
                Status = true,
                Message = "success",
                List = deliveryService.GetDeliveries((int)user.RouteId.Value)
            };
        }
        [HttpPost(template: ApiRoutes.Delivery.Complete)]
        public ResultBase DeliveryComplete([FromBody] DeliveryComplete requestDeliveryComplete)
        {
            logger.LogInformation(JsonConvert.SerializeObject(requestDeliveryComplete));
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);
            Delivery modelData = new Delivery
            {
                DeliveryId = requestDeliveryComplete.DeliveryId,
                DeliveryStatus = requestDeliveryComplete.DeliveryStatus,
                DeliveredTo = (requestDeliveryComplete.DeliveredTo.Length > 100) ? requestDeliveryComplete.DeliveredTo.Substring(0, 100) : requestDeliveryComplete.DeliveredTo,
                DeliveryRemarks = requestDeliveryComplete.Remarks,
                DriverId = (int)user.UserId
            };
            bool isSaved = false;
            string message = string.Empty;
            Model.FCMNotificationModel FCMModelData = new Model.FCMNotificationModel();
            var settingSection = iConfiguration.GetSection("Setting");
            if (settingSection != null)
            {
                FCMModelData.ServerAPIKey = settingSection.GetSection("FCMServiceAPIKey").Value;
                FCMModelData.SenderId = settingSection.GetSection("FCMSenderId").Value;
            }
            if (modelData.DeliveryId > 0)
            {
                FCMModelData.UserId = user.UserId;
                FCMModelData.CompanyId = user.CompanyId;
                FCMModelData.BranchId = user.BranchId;
                isSaved = deliveryService.UpdateAPIDelivery(modelData, FCMModelData);
                message = (isSaved) ? "Success" : "Failure";
            }
            else
            {
                message = "Delivery id is empty";
            }
            return new ResultBase
            {
                Status = isSaved,
                Message = message,
            };
        }
        [HttpPost(template: ApiRoutes.Delivery.Acknowledgement)]
        public ResultBase Acknowledgement([FromBody] DeliveryAcknowledgement acknowledgement)
        {
            logger.LogInformation(JsonConvert.SerializeObject(acknowledgement));
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);
            Delivery modelData = new Delivery
            {
                DeliveryId = acknowledgement.DeliveryId,
                BaseSignature = acknowledgement.BaseSignature,
                DriverId = (int)user.UserId
            };
            bool isSaved = false;
            string message = string.Empty;
            if (modelData.DeliveryId > 0)
            {
                isSaved = deliveryService.UpdateAPIDeliveryAcknowledgement(modelData);
                message = (isSaved) ? "Success" : "Failure";
            }
            else
            {
                message = "Delivery id is empty";
            }
            return new ResultBase
            {
                Status = isSaved,
                Message = message,
            };
        }

        [HttpPost(ApiRoutes.Delivery.CreateToken)]
        public ResultBase CreateToken([FromBody] FCMToken tokenData)
        {
            logger.LogInformation(JsonConvert.SerializeObject(tokenData));
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);

            if (Converters.ConvertLong(user.UserId) > 0)
            {
                Model.FCMTokenModel modelData = new Model.FCMTokenModel();
                modelData.UserId = user.UserId;
                modelData.CompanyId = user.CompanyId;
                modelData.BranchId = user.BranchId;
                modelData.FCMToken = tokenData.TokenId;

                var result = FCMService.SaveFCMTokenUser(modelData);
                string message = string.Empty;
                if (result)
                {
                    message = "Success";
                }
                else
                {
                    message = "Failure";
                }
                return new ResultBase
                {
                    Status = result,
                    Message = message,
                };
            }
            else
            {
                return new ResultBase
                {
                    Status = false,
                    Message = "User Id is empty",
                };
            }
        }

        #region Driver API
        [HttpGet(ApiRoutes.Delivery.DriverProfile)]
        public DriverProfileResult GetDriverProfile()
        {
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);
            var resultData = new DriverProfileResult();

            if (user != null)
            {
                string routeName = string.Empty;
                if (user.RouteId.HasValue)
                {
                    routeName = masterService.GetRoute(user.RouteId.Value).RouteName;
                }

                resultData = new DriverProfileResult()
                {
                    DriverProfile = new DriverProfile()
                    {
                        DriverId = user.UserId,
                        FirstName = user.FirstName,
                        MiddleNmae = user.MiddleName,
                        LastName = user.LastName,
                        RouteId = (int)user.RouteId,
                        Route = routeName
                    }
                };
            }

            if (resultData != null)
            {
                resultData.Status = true;
                resultData.Message = "Success";
            }
            else
            {
                resultData.Status = false;
                resultData.Message = "Failure";
            }
            return resultData;
        }

        [HttpGet(ApiRoutes.Delivery.RouteList)]
        public RouteResult GetRoutes()
        {
            var resultData = new RouteResult();
            var user = userService.GetUser(HttpContext.User.Claims.Where(s => s.Type == "id").FirstOrDefault().Value);
            var routeList = masterService.GetAllRouteList().Where(d => d.CompanyId == user.CompanyId && d.BranchId == user.BranchId).ToList();

            resultData.RouteList = routeList;

            if (routeList != null)
            {
                resultData.Status = true;
                resultData.Message = "Success";
            }
            else
            {
                resultData.Status = false;
                resultData.Message = "Failure";
            }
            return resultData;
        }

        [HttpPost(template: ApiRoutes.Delivery.UpdateUserRoute)]
        public ResultBase UpdateDriverRoute([FromBody] DriverRoute requestDriverRoute)
        {
            logger.LogInformation(JsonConvert.SerializeObject(requestDriverRoute));
            var user = userService.GetUser(requestDriverRoute.DriverId);

            bool isSuccess = false;

            if (user != null && user.RouteId != requestDriverRoute.RouteId)
            {
                isSuccess = userService.UpdateUserRoute(requestDriverRoute.DriverId, requestDriverRoute.RouteId, requestDriverRoute.CompanyId, requestDriverRoute.BranchId);
            }

            if (isSuccess)
            {
                return new ResultBase
                {
                    Status = true,
                    Message = "Driver route updated."
                };
            }
            else
            {
                return new ResultBase
                {
                    Status = false,
                    Message = "Unable to update driver route."
                };
            }
        }

        #endregion


    }
}
