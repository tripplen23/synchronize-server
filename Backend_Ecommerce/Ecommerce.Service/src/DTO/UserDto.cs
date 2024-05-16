
using Ecommerce.Core.src.Entity;
using Ecommerce.Core.src.ValueObject;

namespace Ecommerce.Service.src.DTO
{

    public class UserReadDto : BaseEntity
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole UserRole { get; set; }
    }

    public class UserCreateDto
    {
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string UserPassword { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole UserRole { get; set; }
        public DateOnly? CreatedDate { get; set; }
        public DateOnly? UpdatedDate { get; set; }
    }

    public class UserUpdateDto
    {
        public string? UserName { get; set; }
        public string? UserEmail { get; set; }
        public string? UserPassword { get; set; }
        public string? UserAvatar { get; set; }
        public UserRole? UserRole { get; set; }
    }
}