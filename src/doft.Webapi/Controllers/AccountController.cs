using System;
using System.Collections.Generic;
using System.Linq;
using doft.Application.Commands.Account;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace doft.Webapi.Controllers
    [Route("api/auth")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AccountController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] CreateUserCommand model)
        {
            var result = await _mediator.Send(model);

            if (result == null)
            {
                return BadRequest(new { Message = "User creation failed" });
            }

            return Ok(result);
        }

        [AllowAnonymous]
        [HttpPost("signIn")]
        public async Task<IActionResult> SignIn([FromBody] SignInCommand model)
        {
            var result = await _mediator.Send(model);

            if (result == null)
            {
                return BadRequest(new { Message = "Sign in failed" });
            }
            
            return Ok(result);
        }

    }
}