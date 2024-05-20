using AutoMapper;
using Ecommerce.Core.src.Entity;
using Ecommerce.Service.src.DTO;

namespace Ecommerce.Service.src.Shared
{
    public class MapperProfile : Profile
    {
        public MapperProfile()
        {
            #region User Mapper:
            CreateMap<User, UserReadDto>();
            CreateMap<UserCreateDto, User>();
            CreateMap<UserUpdateDto, User>();
            CreateMap<UserCreateDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.UserPassword))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.UserAvatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole))
                 .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate));

            CreateMap<User, UserReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.UserEmail, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.UserAvatar, opt => opt.MapFrom(src => src.Avatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole));

            CreateMap<UserUpdateDto, User>()
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.UserName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.UserEmail))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.UserPassword))
                .ForMember(dest => dest.Avatar, opt => opt.MapFrom(src => src.UserAvatar))
                .ForMember(dest => dest.UserRole, opt => opt.MapFrom(src => src.UserRole));
            #endregion


            #region Product Mapper:
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ProductTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.ProductInventory))
                 .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages));

            CreateMap<Product, ProductReadDto>()
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.ProductInventory, opt => opt.MapFrom(src => src.Inventory))
                .ForMember(dest => dest.ProductImages, opt => opt.MapFrom(src => src.ProductImages))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Category, opt => opt.MapFrom(src => src.Category))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(src => src.CreatedDate))
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(src => src.UpdatedDate));

            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.ProductTitle))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.ProductDescription))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.ProductPrice))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.Inventory, opt => opt.MapFrom(src => src.ProductInventory));

            CreateMap<Product, ProductUpdateDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.ProductInventory, opt => opt.MapFrom(src => src.Inventory))
                .ForMember(dest => dest.ImagesToUpdate, opt => opt.MapFrom(src => src.ProductImages));

            CreateMap<Product, ProductReviewReadDto>()
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.ProductDescription, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.CategoryId));
            #endregion


            #region Image Mapper:
            CreateMap<ProductImage, ProductImageReadDto>()
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData));

            CreateMap<ProductImageCreateDto, ProductImage>()
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData));

            CreateMap<ProductImageUpdateDto, ProductImage>()
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ImageId));
            #endregion

            #region Category Mapper:
            CreateMap<Category, CategoryReadDto>()
            .ForMember(dest => dest.CategoryId, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Name))
            .ForMember(dest => dest.CategoryImage, opt => opt.MapFrom(src => src.Image));

            CreateMap<CategoryCreateDto, Category>()
            .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CategoryImage));

            CreateMap<CategoryUpdateDto, Category>()
             .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.CategoryName))
            .ForMember(dest => dest.Image, opt => opt.MapFrom(src => src.CategoryImage));
            #endregion

            #region Order Mapper:
            CreateMap<Order, OrderReadDto>()
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.ShippingInfo, opt => opt.MapFrom(src => src.ShippingInfo))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status));

            CreateMap<OrderCreateDto, Order>()
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.ShippingInfo, opt => opt.MapFrom(src => src.ShippingInfo))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus));

            CreateMap<OrderUpdateStatusDto, Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.OrderStatus))
                .ForMember(dest => dest.ShippingInfo, opt => opt.MapFrom(src => src.ShippingInfo));

            CreateMap<Order, OrderReadUpdateDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.OrderStatus, opt => opt.MapFrom(src => src.Status))
                .ForMember(dest => dest.OrderProducts, opt => opt.MapFrom(src => src.OrderProducts))
                .ForMember(dest => dest.ShippingInfo, opt => opt.MapFrom(src => src.ShippingInfo));
            #endregion

            #region Order Products Mapper:
            CreateMap<OrderProduct, OrderProductReadDto>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.ProductTitle, opt => opt.MapFrom(src => src.Product.Title))
                .ForMember(dest => dest.ProductPrice, opt => opt.MapFrom(src => src.Product.Price));

            CreateMap<OrderProductCreateDto, OrderProduct>();
            CreateMap<OrderProductUpdateDto, OrderProduct>();
            #endregion

            #region Shipping Info Mapper:
            CreateMap<ShippingInfo, ShippingInfoReadDto>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.ShippingAddress, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.ShippingCity, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ShippingCountry, opt => opt.MapFrom(src => src.Country))
                .ForMember(dest => dest.ShippingPostCode, opt => opt.MapFrom(src => src.PostCode))
                .ForMember(dest => dest.ShippingPhone, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<ShippingInfoCreateDto, ShippingInfo>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ShippingCity))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.ShippingCountry))
                .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.ShippingPostCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ShippingPhone));

            CreateMap<ShippingInfoUpdateDto, ShippingInfo>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.ShippingInfoId))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.ShippingAddress))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.ShippingCity))
                .ForMember(dest => dest.Country, opt => opt.MapFrom(src => src.ShippingCountry))
                .ForMember(dest => dest.PostCode, opt => opt.MapFrom(src => src.ShippingPostCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.ShippingPhone));
            #endregion

            #region Cart Mapper:
            CreateMap<Cart, CartReadDto>()
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<CartCreateDto, Cart>()
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));

            CreateMap<CartUpdateDto, Cart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CartId))
                .ForMember(dest => dest.CartItems, opt => opt.MapFrom(src => src.CartItems));
            #endregion

            #region Cart Items Mapper:
            CreateMap<CartItemReadDto, CartItem>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<CartItem, CartItemReadDto>()
                .ForMember(dest => dest.CartId, opt => opt.MapFrom(src => src.CartId))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<CartItemCreateDto, CartItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));

            CreateMap<CartItemUpdateDto, CartItem>()
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity));
            #endregion

            #region Review mapper
            CreateMap<Review, ReviewReadDto>()
                .ForMember(dest => dest.ReviewRating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.ReviewContent, opt => opt.MapFrom(src => src.Content))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));
            CreateMap<ReviewReadDto, Review>();
            CreateMap<ReviewCreateDto, Review>()
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.ReviewRating))
                .ForMember(dest => dest.Content, opt => opt.MapFrom(src => src.ReviewContent))
                .ForMember(dest => dest.ProductId, opt => opt.MapFrom(src => src.ProductId));
            CreateMap<Review, ReviewCreateDto>();
            #endregion
        }
    }
}

// Will be modified during the time we use them.