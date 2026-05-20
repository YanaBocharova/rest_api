using ASPWebApi.Models;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Services.Abstract.Dto;
using Services.Abstract.Interfaces;
using System;
using System.Net.Http;
using System.Text.Json;
using Microsoft.Extensions.Logging;

namespace ASPWebApi.Controllers
{
    [ApiController]
    [Route("api/v1/[controller]")]
    public class AccountsController : ControllerBase
    {
        private readonly IServiceManager serviceManager;
        private readonly IMapper mapper;
        private readonly ILogger<AccountsController> logger;

        public AccountsController(IServiceManager serviceManager, IMapper mapper, ILogger<AccountsController> logger)
        {
            this.serviceManager = serviceManager;
            this.mapper = mapper;
            this.logger = logger;
        }

        // POST api/v1/Accounts/google
        // Accepts: { token: string }
        // Verifies the Google ID token and returns or creates the corresponding account.
        [HttpPost("google", Name = "GoogleSignIn")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<AccountModel>> GoogleSignIn([
            FromBody] GoogleTokenRequest request,
            CancellationToken cancellationToken)
        {
            if (request == null || string.IsNullOrWhiteSpace(request.Token))
            {
                logger.LogWarning("GoogleSignIn: Missing token in request.");
                return BadRequest("Missing token");
            }

            try
            {
                // Verify token with Google
                using var http = new HttpClient();
                var verifyUrl = $"https://oauth2.googleapis.com/tokeninfo?id_token={Uri.EscapeDataString(request.Token)}";
                var verifyResponse = await http.GetAsync(verifyUrl, cancellationToken);

                if (!verifyResponse.IsSuccessStatusCode)
                {
                    logger.LogWarning("GoogleSignIn: Token verification failed with status code {StatusCode}.", verifyResponse.StatusCode);
                    return Unauthorized("Invalid token");
                }

                var payload = await verifyResponse.Content.ReadAsStringAsync(cancellationToken);
                using var doc = JsonDocument.Parse(payload);

                if (!doc.RootElement.TryGetProperty("email", out var emailEl))
                {
                    logger.LogWarning("GoogleSignIn: Token did not contain email.");
                    return BadRequest("Token did not contain email");
                }

                var email = emailEl.GetString();
                if (string.IsNullOrWhiteSpace(email))
                {
                    logger.LogWarning("GoogleSignIn: Email not present in token.");
                    return BadRequest("Email not present in token");
                }

                // Look up existing account
                var existing = await serviceManager.AccountsService.GetAccountByEmail(email, cancellationToken);
                if (existing != null)
                {
                    logger.LogInformation("GoogleSignIn: Existing account found for email {Email}.", email);
                    return Ok(mapper.Map<AccountModel>(existing));
                }

                // Create a new account for this Google user. Password is a random GUID placeholder.
                var newModel = new AccountModel
                {
                    Email = email,
                    Password = Guid.NewGuid().ToString()
                };

                var created = await serviceManager.AccountsService.CreateAccount(
                    mapper.Map<Services.Abstract.Dto.AccountDto>(newModel), cancellationToken);

                logger.LogInformation("GoogleSignIn: New account created for email {Email}.", email);

                return CreatedAtAction(
                    nameof(GetAccount),
                    new { id = created.Id },
                    mapper.Map<AccountModel>(created));
            }
            catch (HttpRequestException ex)
            {
                logger.LogError(ex, "GoogleSignIn: HTTP request failed.");
                return StatusCode(500, "An error occurred while verifying the token.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "GoogleSignIn: An unexpected error occurred.");
                return StatusCode(500, "An unexpected error occurred.");
            }
        }

        public class GoogleTokenRequest
        {
            public string Token { get; set; }
        }

        [HttpGet(Name = "GetAllAccounts")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<AccountModel>>> GetAllAccounts(
            CancellationToken cancellationToken)
        {
            var accounts = await serviceManager
                .AccountsService
                .GetAllAccounts(cancellationToken);

            return Ok(mapper.Map<IEnumerable<AccountModel>>(accounts));
        }

        [HttpGet("{id:int}", Name = "GetAccountById")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountModel>> GetAccount(
            int id,
            CancellationToken cancellationToken)
        {
            var account = await serviceManager
                .AccountsService
                .GetAccountById(id, cancellationToken);

            if (account is null)
                return NotFound();

            return Ok(mapper.Map<AccountModel>(account));
        }

        [HttpPost("signin", Name = "GetAccountByEmailAndPassword")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<AccountModel>> GetAccountSingIn(
            [FromBody] AccountModel newAccount,
            CancellationToken cancellationToken)
        {
            var account = await serviceManager
                .AccountsService
                .GetAccountByEmail(newAccount.Email, newAccount.Password, cancellationToken);

            if (account is null)
                return NotFound();

            return Ok(mapper.Map<AccountModel>(account));
        }


        [HttpPost(Name = "CreateAccount")]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> CreateAccount(
            [FromBody] AccountModel newAccount,
            CancellationToken cancellationToken)
        {
            var dto = mapper.Map<AccountDto>(newAccount);

            var created = await serviceManager
                .AccountsService
                .CreateAccount(dto, cancellationToken);

            return CreatedAtAction(
                nameof(GetAccount),
                new { id = created.Id },
                mapper.Map<AccountModel>(created));
        }

        [HttpDelete("{email}", Name = "DeleteAccountByEmail")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteAccount(
            string email,
            CancellationToken cancellationToken)
        {
            var removed = await serviceManager
                .AccountsService
                .RemoveAccountByEmail(email, cancellationToken);

            if (removed != null)
                return NotFound();

            return NoContent();
        }
    }
}
