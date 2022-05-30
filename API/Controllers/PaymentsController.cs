using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using API.DTOs;
using API.Errors;
using AutoMapper;
using Core.Entities;
using Core.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Stripe;
using Order = Core.Entities.OrderAggregate.Order;

namespace API.Controllers
{
    public class PaymentsController : BaseApiController
    {
        private readonly IPaymentService _paymentService;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentsController> _logger;
        private const string WhSecret = "whsec_2fa8c8809cd8e5f188174e827c4393a881385ca2cd49b242c109d5623d06e98d";

        public PaymentsController(IPaymentService paymentService,
                                IMapper mapper,
                                ILogger<PaymentsController> logger)
        {
            _mapper = mapper;
            _logger = logger;
            _paymentService = paymentService;
        }

        [Authorize]
        [HttpPost("{basketId}")]
        public async Task<ActionResult<CustomerBasket>> CreateOrUpdatePaymentIntent(string basketId)
        {
            var basket = await _paymentService.CreateOrUpdatePaymentIntent(basketId);
            if (basket == null) return BadRequest(new APIResponse(400, "Problem with your basket"));
            //return Ok(_mapper.Map<CustomerBasket, CustomerBasketDTO>(basket));
            return Ok(basket);
        }

        [HttpPost("webhook")]
        public async Task<ActionResult> StripeWebhook()
        {
            var json = await new StreamReader(HttpContext.Request.Body).ReadToEndAsync();
            var stripeEvent = EventUtility.ConstructEvent(json, Request.Headers["Stripe-Signature"], WhSecret);

            PaymentIntent paymentIntent;
            Order order;

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Succeeded: ", paymentIntent.Id);
                    //TODO: Update order status to success 
                    order = await _paymentService.UpdateOrderPaymentSucceeded(paymentIntent.Id);
                    _logger.LogInformation("Order Updated to PaymentReceived: ", order.Id);
                    break;
                case "payment_intent.payment_failed":
                    paymentIntent = (PaymentIntent)stripeEvent.Data.Object;
                    _logger.LogInformation("Payment Failed: ", paymentIntent.Id);
                    //TODO: Update order status 
                    order = await _paymentService.UpdateOrderPaymentFailed(paymentIntent.Id);
                    _logger.LogInformation("Order Updated to PaymentFailed: ", order.Id);
                    break;
                default:
                    break;
            }

            return new EmptyResult();
        }
    }
}