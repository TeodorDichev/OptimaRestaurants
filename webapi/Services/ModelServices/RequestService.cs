using Microsoft.AspNetCore.Identity;
using webapi.Data;
using webapi.Models;
using webapi.Services.FileServices;

namespace webapi.Services.ModelServices
{
    public class RequestService
    {
        private readonly JWTService _jwtService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IConfiguration _configuration;
        private readonly OptimaRestaurantContext _context;
        private readonly QrCodesService _qrCodesService;
        private readonly PicturesAndIconsService _pictureService;

        public RequestService(JWTService jwtService,
        UserManager<ApplicationUser> userManager,
        IConfiguration configuration,
        OptimaRestaurantContext context,
        QrCodesService qrCodesService,
        PicturesAndIconsService pictureService)
        {
            _jwtService = jwtService;
            _userManager = userManager;
            _configuration = configuration;
            _context = context;
            _qrCodesService = qrCodesService;
            _pictureService = pictureService;
        }

    }
}
