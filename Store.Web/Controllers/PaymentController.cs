using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Store.Service.Services.BasketServices.Dtos;
using Store.Service.Services.PaymentServices;
using Stripe;

namespace Store.Web.Controllers
{
    public class PaymentController : BaseController
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        const string endpointSecret = "whsec_98f95a71de73b7e7b9aabdb45e59cc299192c88fe30573d1e4837afa2a7e71a9";

        public PaymentController(IPaymentService paymentService,ILogger<PaymentController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }
        [HttpPost]
        public async Task<ActionResult<CustomerBasketDto>>CreateOrUpdatePaymentIntetnt(CustomerBasketDto input)
            =>Ok(await _paymentService.CreateOrUpdatePayment(input));



        [HttpPost("webhook")]
        public async Task<IActionResult> Index()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            if (string.IsNullOrEmpty(json))
            {
                _logger.LogError("Empty JSON payload");
                return BadRequest("Empty JSON payload");
            }

            if (!Request.Headers.TryGetValue("Stripe-Signature", out var stripeSignature))
            {
                _logger.LogError("Missing Stripe-Signature header");
                return BadRequest("Missing Stripe-Signature header");
            }

            if (string.IsNullOrEmpty(endpointSecret))
            {
                _logger.LogError("Empty endpoint secret");
                throw new InvalidOperationException("Endpoint secret not set.");
            }

            try
            {
                var stripeEvent = EventUtility.ConstructEvent(json, stripeSignature, endpointSecret);
                PaymentIntent paymentIntent;

                switch (stripeEvent.Type)
                {
                    case "payment_intent.payment_failed":
                        paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent == null)
                        {
                            _logger.LogError("PaymentIntent is null");
                            return BadRequest("PaymentIntent is null");
                        }
                        _logger.LogInformation("PaymentFailed : {0}", paymentIntent.Id);
                        var failedOrder = await _paymentService.UppdateOrderPaymentFailed(paymentIntent.Id);
                        _logger.LogInformation("Order Updated To Payment Failed : {0}", failedOrder.Id);
                        break;

                    case "payment_intent.succeeded":
                        paymentIntent = stripeEvent.Data.Object as PaymentIntent;
                        if (paymentIntent == null)
                        {
                            _logger.LogError("PaymentIntent is null");
                            return BadRequest("PaymentIntent is null");
                        }
                        _logger.LogInformation("PaymentSucceeded : {0}", paymentIntent.Id);
                        var succeededOrder = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                        _logger.LogInformation("Order Updated To Payment Succeeded : {0}", succeededOrder.Id);
                        break;

                    case "payment_intent.created":
                        _logger.LogInformation("PaymentCreated");
                        break;

                    default:
                        _logger.LogWarning("Unhandled event type: {0}", stripeEvent.Type);
                        break;
                }

                return Ok();
            }
            catch (StripeException e)
            {
                _logger.LogError(e, "Stripe exception occurred");
                return BadRequest();
            }
        }



        }
    }
