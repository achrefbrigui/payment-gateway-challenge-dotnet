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
    private readonly ILogger<PaymentsController> _logger;

    public PaymentsController(IPaymentRepository paymentsRepository,
        IPaymentService paymentService,
        IValidator<PostPaymentRequest> postPaymentRequestValidator,
        ILogger<PaymentsController> logger)
    {
        _paymentsRepository = paymentsRepository;
        _paymentService = paymentService;
        _postPaymentRequestValidator = postPaymentRequestValidator;
        _logger = logger;
    }

    [HttpGet("{id:guid}")]
    [ProducesResponseType(typeof(PostPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public ActionResult<PostPaymentResponse?> GetPayment(Guid id)
    {
        _logger.LogInformation("Retrieving payment {PaymentId}", id);

        var payment = _paymentsRepository.Get(id);

        if (payment == null)
        {
            _logger.LogInformation("Payment {PaymentId} not found", id);
            return NotFound();
        }

        _logger.LogInformation("Payment {PaymentId} retrieved successfully", id);
        return new OkObjectResult(payment.ToGetResponse());
    }

    [HttpPost]
    [ProducesResponseType(typeof(PostPaymentResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status500InternalServerError)]
    public async Task<ActionResult<PostPaymentResponse>> ProcessPayment(
    PostPaymentRequest request)
    {
        _logger.LogInformation(
            "Processing payment request for {Amount} {Currency}",
            request.Amount, request.Currency);

        var validatorResult = await _postPaymentRequestValidator.ValidateAsync(request);

        if (!validatorResult.IsValid)
        {
            _logger.LogInformation(
                "Payment request failed validation with {ErrorCount} error(s)",
                validatorResult.Errors.Count);

            return new BadRequestObjectResult(validatorResult.Errors);
        }

        var payment = await _paymentService.ProcessPayment(request);

        _logger.LogInformation(
            "Payment {PaymentId} processed with status {PaymentStatus}",
            payment.Id, payment.Status);

        return payment.Status switch
        {
            PaymentStatus.Authorized => Ok(payment),
            PaymentStatus.Declined => Ok(payment),
            PaymentStatus.Rejected => BadRequest(payment),
            _ => StatusCode(StatusCodes.Status500InternalServerError)
        };
    }
}