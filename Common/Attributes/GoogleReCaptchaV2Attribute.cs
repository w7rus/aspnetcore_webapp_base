using System;
using System.ComponentModel.DataAnnotations;
using System.Net;
using System.Net.Http;
using Common.Options;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;

namespace Common.Attributes
{
    /// <summary>
    /// Attribute to mark Member of a DTO model to verify that requester had provided valid GoogleReCaptchaV2 token.
    /// </summary>
    public class GoogleReCaptchaV2Attribute : ValidationAttribute
    {
        protected override ValidationResult IsValid(object value, ValidationContext validationContext)
        {
            var errorResult = new Lazy<ValidationResult>(() =>
                new ValidationResult("Google reCAPTCHA validation failed", new[] {validationContext.MemberName}));
            if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
            {
                return errorResult.Value;
            }

            var reCaptchaResponse = value.ToString();
            var options = validationContext.GetService<IOptions<GoogleReCaptchaV2Options>>();
            var reCaptchaSecret = options?.Value.PrivateKey;

            var httpClient = new HttpClient();
            var httpResponse = httpClient
                .GetAsync(
                    $"https://www.google.com/recaptcha/api/siteverify?secret={reCaptchaSecret}&response={reCaptchaResponse}")
                .ConfigureAwait(false).GetAwaiter().GetResult();
            if (httpResponse.StatusCode != HttpStatusCode.OK)
            {
                return errorResult.Value;
            }

            var jsonResponse = httpResponse.Content.ReadAsStringAsync().Result;
            dynamic jsonData = JObject.Parse(jsonResponse);
            return jsonData.success != true.ToString().ToLower() ? errorResult.Value : ValidationResult.Success;
        }
    }
}