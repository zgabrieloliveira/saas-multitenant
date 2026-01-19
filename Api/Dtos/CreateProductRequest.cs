using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

public record CreateProductRequest(
    [Required(ErrorMessage = "Name has to be defined")] 
    [MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
    string Name,
    [MaxLength(500, ErrorMessage =  "Description has to be 500 characters maximum")]
    string Description, 
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    decimal Price
);