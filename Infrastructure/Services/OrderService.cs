using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Entities;
using Core.Entities.OrderAggregate;
using Core.Interfaces;
using Core.Specifications;

namespace Infrastructure.Services
{
    public class OrderService : IOrderService
    {
        // private readonly IGenericRepository<Order> _orderRepo;
        // private readonly IGenericRepository<DeliveryMethod> _deliveryMethodRepo;
        // private readonly IGenericRepository<Product> _productRepo;
        private readonly IBasketRepository _basketRepo;
        private readonly IUnitOfWork _unitOfWork;
        private readonly IPaymentService _paymentService;

        public OrderService(
                            // IGenericRepository<Order> orderRepo,
                            // IGenericRepository<DeliveryMethod> deliveryMethodRepo,
                            // IGenericRepository<Product> productRepo,
                            IBasketRepository basketRepo,
                            IUnitOfWork unitOfWork,
                            IPaymentService paymentService)
        {
            // this._orderRepo = orderRepo;
            // this._deliveryMethodRepo = deliveryMethodRepo;
            // this._productRepo = productRepo;
            this._basketRepo = basketRepo;
            this._unitOfWork = unitOfWork;
            this._paymentService = paymentService;
        }

        public async Task<Order> CreateOrderAsync(string buyerEmail, int deliveryMethodId, string basketId, Address shippingAddress)
        {
            //get basket from basketRepo
            var basket = await _basketRepo.GetBasketAsync(basketId);

            //get items from productRepo
            var listOrderItems = new List<OrderItem>();
            foreach (var item in basket.Items)
            {
                var productItem = await _unitOfWork.Repository<Product>().GetByIdAsync(item.Id);
                var itemOrdered = new ProductItemOrdered(productItem.Id, productItem.Name, productItem.PictureUrl);
                var orderItem = new OrderItem(itemOrdered, productItem.Price, item.Quantity);

                listOrderItems.Add(orderItem);
            }

            //get delivery method from deliveryMethodRepo
            var deliveryMethod = await _unitOfWork.Repository<DeliveryMethod>().GetByIdAsync(deliveryMethodId);

            //calc subtotal
            var subTotal = listOrderItems.Sum(i => i.Price * i.Quantity);

            //check if order exists
            var spec = new OrderByPaymentIntentIdSpecification(basket.PaymentIntentId);
            var existingOrder = await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);

            if (existingOrder != null)
            {
                _unitOfWork.Repository<Order>().Delete(existingOrder);
                await _paymentService.CreateOrUpdatePaymentIntent(basket.PaymentIntentId);
            }

            //create order using orderRepo
            var order = new Order(listOrderItems, buyerEmail, shippingAddress, deliveryMethod, subTotal, basket.PaymentIntentId);

            //ToDo: save to db
            _unitOfWork.Repository<Order>().Add(order);
            var result = await _unitOfWork.Complete();

            if (result <= 0)
                return null;

            // //delete basket
            // await _basketRepo.DeleteBasketAsync(basketId);

            //return the order 
            return order;
        }

        public async Task<IReadOnlyList<DeliveryMethod>> GetDeliveryMethodsAsync()
        {
            return await _unitOfWork.Repository<DeliveryMethod>().ListAllAsync();
        }

        public async Task<Order> GetOrderByIdAsync(int id, string buyerEmail)
        {
            var spec = new OrderswithItemsAndOrderingSpecification(id, buyerEmail);
            return await _unitOfWork.Repository<Order>().GetEntityWithSpec(spec);
        }

        public async Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerEmail)
        {
            var spec = new OrderswithItemsAndOrderingSpecification(buyerEmail);
            return await _unitOfWork.Repository<Order>().ListAsync(spec);
        }
    }
}