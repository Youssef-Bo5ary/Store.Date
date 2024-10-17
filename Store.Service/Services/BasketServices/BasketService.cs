using AutoMapper;
using Store.Repository.Basket;
using Store.Repository.Basket.Models;
using Store.Service.Services.BasketServices.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Store.Service.Services.BasketServices
{
    public class BasketService : IBasketService
    {
        private readonly IBasketRepository _basketRepo;

        private readonly IMapper _mapper;
        public BasketService(IBasketRepository basketRepo,IMapper mapper)
        {
            _basketRepo = basketRepo;
           _mapper = mapper;
        }

        public IMapper Mapper { get; }

        public async Task<bool> DeleteBasketAsync(string basketId)
        =>await _basketRepo.DeleteBasketAsync(basketId);

        public async Task<CustomerBasketDto> GetBasketAsync(string basketId)
        {
            var basket=await _basketRepo.GetBasketAsync(basketId);
            if(basket==null)
                    return new CustomerBasketDto();

            var mappedBasket=_mapper.Map<CustomerBasketDto>(basket);
            return mappedBasket;

        }

        public async Task<CustomerBasketDto> UpdateBasketAsync(CustomerBasketDto input)
        {
            if (input.Id is null)
                input.Id=GenerateRandomBasketId();

            var customerBasket = _mapper.Map<CustomerBasket>(input);
            var updaatedBasket = await _basketRepo.UpdateBasketAsync(customerBasket);
            var mappedUpdatedBasket = _mapper.Map<CustomerBasketDto>(updaatedBasket);
            return mappedUpdatedBasket;
        }
        private string GenerateRandomBasketId()
        {
            Random random = new Random();
            int randomDigits=random.Next(1000, 10000);
            return $"BS-{randomDigits}";
        }
    }
}
