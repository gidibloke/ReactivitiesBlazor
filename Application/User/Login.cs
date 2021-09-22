using MediatR;

using System.Threading.Tasks;
using SharedObjects;
using System.Threading;
using Persistence;
using FluentValidation;
using Application.Interfaces;
using Microsoft.AspNetCore.Identity;
using Domain;
using Application.Errors;
using System.Net;

namespace Application.User
{
    public class Login
    {
        public class Query : IRequest<SharedObjects.User> 
        {
            public string Email { get; set; }
            public string Password { get; set; }

        }
        public class QueryValidator : AbstractValidator<Query>
        {
            public QueryValidator()
            {
                RuleFor(x => x.Email).NotEmpty();
                RuleFor(x => x.Password).NotEmpty();
            }
        }


        public class Handler : IRequestHandler<Query, SharedObjects.User>
        {
            private readonly DataContext _context;
            private readonly IJwtGenerator _jwtGenerator;
            private readonly SignInManager<AppUser> _signInManager;
            private readonly UserManager<AppUser> _userManager;

            public Handler(DataContext context, IJwtGenerator jwtGenerator, SignInManager<AppUser> signInManager, UserManager<AppUser> userManager)
            {
                _context = context;
                _jwtGenerator = jwtGenerator;
                _signInManager = signInManager;
                _userManager = userManager;
            }
            public async Task<SharedObjects.User> Handle(Query request, CancellationToken cancellationToken)
            {
                //handler logic goes here
                var user = await _userManager.FindByEmailAsync(request.Email);
                if (user == null)
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { message = "Invalid login attempt"});
                }
                var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
                if (result.Succeeded)
                {
                    return new SharedObjects.User
                    {
                        DisplayName = user.DisplayName,
                        Token = _jwtGenerator.CreateToken(user),
                        Username = user.UserName,
                        Image = null
                    };
                }
                else
                {
                    throw new RestException(HttpStatusCode.Unauthorized, new { message = "Invalid login attempt" });
                }
                
            }
        }
    }
}
