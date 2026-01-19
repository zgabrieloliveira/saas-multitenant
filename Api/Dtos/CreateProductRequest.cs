using System.ComponentModel.DataAnnotations;

namespace Api.Dtos;

/// <summary>
/// data transfer object for creating a new product.
/// </summary>
/// <param name="Name">the name of the product. must be between 3 and 100 characters.</param>
/// <param name="Description">optional description of the product. max 500 characters.</param>
/// <param name="Price">product price. must be greater than zero.</param>
public record CreateProductRequest(
    [Required(ErrorMessage = "Name has to be defined")] 
    [MinLength(3, ErrorMessage = "Name must have at least 3 characters")]
    string Name,
    
    [MaxLength(500, ErrorMessage =  "Description has to be 500 characters maximum")]
    string Description, 
    
    [Range(0.01, 1000000, ErrorMessage = "Price must be greater than 0")]
    decimal Price
);