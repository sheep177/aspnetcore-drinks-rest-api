using AutoMapper;
using Drinks.API.Entities;
using Drinks.API.Models;

namespace Drinks.API.Profiles;

public class DrinkProfile: Profile
{
    public DrinkProfile()
    {
        CreateMap<Drink, DrinksDto>();
        CreateMap<DrinksForCreationDto, Drink>();
    }
}