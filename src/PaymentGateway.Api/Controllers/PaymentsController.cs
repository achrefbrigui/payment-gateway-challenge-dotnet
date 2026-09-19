using System.Net;

using FluentValidation;

using Microsoft.AspNetCore.Mvc;

using PaymentGateway.Api.Domain.Enums;
using PaymentGateway.Api.Mappers;
using PaymentGateway.Api.Models.Requests;
using PaymentGateway.Api.Models.Responses;
using PaymentGateway.Api.Services;

namespace PaymentGateway.Api.Controllers;

[Route("api/[controller]")]
[ApiController]
public class PaymentsController : Controller
{
    private readonly IPaymentRepository _paymentsRepository;
    private readonly IPaymentService _paymentService;
    private readonly IValidator<PostPaymentRequest> _postPaymentRequestValidator;

    public PaymentsController(IPaymentRepository paymentsRepository,
        IPaymentService paymentService,
        IValidator<PostPaymentRequest> postPaymentRequestValidator)
    {
        _paymentsRepository = paymentsRepository;
        _paymentService = paymentService;
        _postPaymentRequestValidator = postPaymentRequestValidator;
    }

    [HttpGet("{id:guid}")]
    public ActionResult<PostPaymentResponse?> GetPayment(Guid id)
    {
        var payment = _paymentsRepository.Get(id);

        if (payment == null)
        {
            return NotFound();
        }

        return new OkObjectResult(payment.ToGetResponse());
    }

    [HttpPost]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPayment(PostPaymentRequest request)
    {
        try
        {
            var validatorResult = await _postPaymentRequestValidator.ValidateAsync(request);
            if (!validatorResult.IsValid)
            {
                return new BadRequestObjectResult(validatorResult.Errors);
            }
            var payment = await _paymentService.ProcessPayment(request);
            return payment.Status switch
            {
                PaymentStatus.Authorized => Ok(payment),
                PaymentStatus.Declined => Ok(payment),
                PaymentStatus.Rejected => BadRequest(payment),
                _ => StatusCode(StatusCodes.Status500InternalServerError)
            };
        }
        catch (HttpRequestException ex) when (ex.StatusCode == HttpStatusCode.ServiceUnavailable)
        {
            return StatusCode(StatusCodes.Status503ServiceUnavailable);
        }
    }
}